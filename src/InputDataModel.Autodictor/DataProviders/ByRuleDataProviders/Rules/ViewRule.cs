using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Entities;
using InputDataModel.Autodictor.Model;
using NCalc;
using Shared.CrcCalculate;
using Shared.Extensions;
using Shared.Helpers;

namespace InputDataModel.Autodictor.DataProviders.ByRuleDataProviders.Rules
{
    /// <summary>
    /// Правило отображения порции даных
    /// STX - \u0002
    /// RTX - \u0003
    /// </summary>
    public class ViewRule
    {
        #region fields

        private readonly string _addressDevice;
        public readonly ViewRuleOption Option;

        #endregion



        #region ctor

        public ViewRule(string addressDevice, ViewRuleOption option)
        {
            _addressDevice = addressDevice;
            Option = option;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Создать строку запроса ПОД ДАННЫЕ, подставив в форматную строку запроса значения переменных из списка items.
        /// </summary>
        /// <param name="items">элементы прошедшие фильтрацию для правила</param>
        /// <returns>строку запроса и батч данных в обертке </returns>
        public IEnumerable<ViewRuleRequestModelWrapper> GetDataRequestString(List<AdInputType> items)
        {
            var viewedItems = GetViewedItems(items);
            if (viewedItems == null)
            {
                yield return null;
            }
            else
            {
                foreach (var batch in viewedItems.Batch(Option.BatchSize))
                {
                    var resStr = CreateStringRequest(batch);
                    yield return new ViewRuleRequestModelWrapper
                    {
                        BatchedData = batch,
                        StringRequest = resStr,
                        RequestOption = Option.RequestOption,
                        ResponseOption = Option.ResponseOption
                    };
                }
            }
        }


        /// <summary>
        /// Создать строку запроса ПОД КОМАНДУ.
        /// Body содержит готовый запрос для команды.
        /// </summary>
        /// <returns></returns>
        public ViewRuleRequestModelWrapper GetCommandRequestString()
        {
            var header = Option.RequestOption.Header;
            var body = Option.RequestOption.Body;
            var footer = Option.RequestOption.Footer;

            //КОНКАТЕНИРОВАТЬ СТРОКИ В СУММАРНУЮ СТРОКУ-------------------------------------------------------------------------------------
            //resSumStr содержит только ЗАВИСИМЫЕ данные: {AddressDevice} {NByte} {CRC}}
            var resSumStr = header + body + footer;

            //ВСТАВИТЬ ЗАВИСИМЫЕ ДАННЫЕ ({AddressDevice} {NByte} {CRC})-------------------------------------------------
            var resDependencyStr = MakeDependentInserts(resSumStr);

            return new ViewRuleRequestModelWrapper
            {
                BatchedData = null,
                StringRequest = resDependencyStr,
                RequestOption = Option.RequestOption,
                ResponseOption = Option.ResponseOption
            };
        }


        /// <summary>
        /// Вернуть элементы из диапазона укзанного в правиле отображения
        /// Если границы диапазона не прпавильны вернуть null
        /// </summary>
        private IEnumerable<AdInputType> GetViewedItems(List<AdInputType> items)
        {
            try
            {
                return items.GetRange(Option.StartPosition, Option.Count);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из одного батча данных.
        /// </summary>
        /// <returns></returns>
        private string CreateStringRequest(IEnumerable<AdInputType> batch)
        {
            var items = batch.ToList();

            //DEBUG----------------------------------------------------
            var header = Option.RequestOption.Header;

            //var body = "%StationArr= {NumberOfCharacters:X2} \\\"{StationArrival}\\\"" +
            //           "%TypeName= {TypeName}" +
            //           "%NumberOfTrain= {NumberOfTrain}" +
            //           "%PathNumber= {NumberOfCharacters:X2} \\\"{PathNumber:D5}\\\"" +
            //           "%Stations= {Stations} " +
            //           "%TArrival= {TArrival:t}" +
            //           "%rowNumb= {(rowNumber*11-11):X3}" +
            //           "%StationDep= {NumberOfCharacters:X2} \\\"{StationDeparture}\\\"" +
            //           "%StatC= {NumberOfCharacters:X2} \\\"{StationsCut}\\\"" +
            //           "%DelayT= {DelayTime}" +
            //           "%ExpectedT= {ExpectedTime:t}";

            //var body = "%StationArr= {NumberOfCharacters:X2} \\\"{StationArrival}\\\"" +
            //           "%TypeName= {TypeName}" +
            //           "%NumberOfTrain= {NumberOfTrain}" +
            //           "%PathNumber= {PathNumber:D5}" +
            //           "%Stations= {Stations} " +
            //           "%TArrival= {TArrival:t}" +
            //           "%rowNumb= {((rowNumber*11-11)+5):X3}" +
            //           "%StationDep= {NumberOfCharacters:X2} \\\"{StationDeparture}\\\"" +
            //           "%StatC= {NumberOfCharacters:X2} \\\"{StationsCut}\\\"";

            var body = Option.RequestOption.Body;
            var footer = Option.RequestOption.Footer;//" {CRCXor:X2}\u0003";
            //DEBUG----------------------------------------------------

            //ЗАПОЛНИТЬ ТЕЛО ЗАПРОСА (вставить НЕЗАВИСИМЫЕ данные)-------------------------------------------------------------------------
            var resBodyStr = new StringBuilder();
            var startRow = Option.StartPosition;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var currentRow = startRow + i + 1;
                var res = MakeBodySectionIndependentInserts(body, item, currentRow);
                resBodyStr.Append(res);
            }

            //ВСТАВИТЬ ЗАВИСИМЫЕ ДАННЫЕ В ТЕЛО ЗАПРОСА--------------------------------------------------------------------------------------
            var resBodyDependentStr= MakeBodyDependentInserts(resBodyStr.ToString());

            //ОГРАНИЧИТЬ ДЛИННУ ТЕЛА ЗАПРОСА------------------------------------------------------------------------------------------------
            var limitBodyStr= LimitBodySectionLenght(new StringBuilder(resBodyDependentStr));

            //КОНКАТЕНИРОВАТЬ СТРОКИ В СУММАРНУЮ СТРОКУ-------------------------------------------------------------------------------------
            //resSumStr содержит только ЗАВИСИМЫЕ данные: {AddressDevice} {NByte} {CRC}}
            var resSumStr = header + limitBodyStr + footer;

            //ВСТАВИТЬ ЗАВИСИМЫЕ ДАННЫЕ ({AddressDevice} {NByte} {CRC})-------------------------------------------------
            var resDependencyStr = MakeDependentInserts(resSumStr);

            return resDependencyStr;
        }


        /// <summary>
        /// Первоначальная вставка НЕЗАВИСИМЫХ переменных
        /// </summary>
        private string MakeBodySectionIndependentInserts(string body, AdInputType uit, int currentRow)
        {
            var lang = uit.Lang;
            //ЗАПОЛНИТЬ СЛОВАРЬ ВСЕМИ ВОЗМОЖНЫМИ ВАРИАНТАМИ ВСТАВОК
            var typeTrain = uit.TrainType?.GetName(lang);
            var typeAlias = uit.TrainType?.GetNameAlias(lang);
            var eventTrain = uit.Event?.GetName(lang);
            var addition = uit.Addition?.GetName(lang);
            var stations = CreateStationsStr(uit, lang);
            var stationsCut = CreateStationsCutStr(uit, lang);
            var note = uit.Note?.GetName(lang);
            var daysFollowing = uit.DaysFollowing?.GetName(lang);
            var daysFollowingAlias = uit.DaysFollowing?.GetNameAlias(lang);
            var dict = new Dictionary<string, object>
            {
                ["TypeName"] = string.IsNullOrEmpty(typeTrain) ? " " : typeTrain,
                ["TypeAlias"] = string.IsNullOrEmpty(typeAlias) ? " " : typeAlias,
                [nameof(uit.NumberOfTrain)] = string.IsNullOrEmpty(uit.NumberOfTrain) ? " " : uit.NumberOfTrain,
                [nameof(uit.PathNumber)] = string.IsNullOrEmpty(uit.PathNumber) ? " " : uit.PathNumber,
                [nameof(uit.Event)] = string.IsNullOrEmpty(eventTrain) ? " " : eventTrain,
                [nameof(uit.Addition)] = string.IsNullOrEmpty(addition) ? " " : addition,
                ["Stations"] = string.IsNullOrEmpty(stations) ? " " : stations,
                ["StationsCut"] = string.IsNullOrEmpty(stationsCut) ? " " : stationsCut,
                [nameof(uit.StationArrival)] = uit.StationArrival?.GetName(lang) ?? " ",
                [nameof(uit.StationDeparture)] = uit.StationDeparture?.GetName(lang) ?? " ",
                [nameof(uit.Note)] = string.IsNullOrEmpty(note) ? " " : note,
                ["DaysFollowing"] = string.IsNullOrEmpty(daysFollowing) ? " " : daysFollowing,
                ["DaysFollowingAlias"] = string.IsNullOrEmpty(daysFollowingAlias) ? " " : daysFollowingAlias,
                [nameof(uit.DelayTime)] = uit.DelayTime ?? DateTime.MinValue,
                [nameof(uit.ExpectedTime)] = uit.ExpectedTime,
                ["TArrival"] = uit.ArrivalTime ?? DateTime.MinValue,
                ["TDepart"] = uit.DepartureTime ?? DateTime.MinValue,
                ["Hour"] = DateTime.Now.Hour,
                ["Minute"] = DateTime.Now.Minute,
                ["Second"] = DateTime.Now.Second,
                ["SyncTInSec"] = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second,
                ["rowNumber"] = currentRow
            };
            //ВСТАВИТЬ ПЕРЕМЕННЫЕ ИЗ СЛОВАРЯ В body
            var resStr = HelpersString.StringTemplateInsert(body, dict);
            return resStr;
        }


        /// <summary>
        /// Вставить зависимые (вычисляемые) данные в ТЕЛО запроса 
        /// {NumberOfCharacters}
        /// </summary>
        /// <param name="str">входная строка тела в которой только ЗАВИСИМЫЕ данные</param>
        /// <returns></returns>
        private string MakeBodyDependentInserts(string str)
        {
            if (str.Contains("}"))                                                           //если указанны переменные подстановки
            {
                var subStr = str.Split('}');
                StringBuilder resStr = new StringBuilder();
                for (var index = 0; index < subStr.Length; index++)
                {
                    var s = subStr[index];
                    var replaseStr = (s.Contains("{")) ? (s + "}") : s;
                    //1. Подсчет кол-ва символов
                    if (replaseStr.Contains("NumberOfCharacters"))
                    {
                        var targetStr = (subStr.Length > (index + 1)) ? subStr[index + 1] : string.Empty;
                        if (Regex.Match(targetStr, "\\\"(.*)\"").Success) //
                        {
                            var matchString = Regex.Match(targetStr, "\\\"(.*)\\\"").Groups[1].Value;
                            if (!string.IsNullOrEmpty(matchString))
                            {
                                var lenght = matchString.TrimEnd('\\').Length;
                                var dateFormat = Regex.Match(replaseStr, "\\{NumberOfCharacters:(.*)\\}").Groups[1].Value;
                                var formatStr = !string.IsNullOrEmpty(dateFormat) ?
                                    string.Format(replaseStr.Replace("NumberOfCharacters", "0"), lenght.ToString(dateFormat)) :
                                    string.Format(replaseStr.Replace("NumberOfCharacters", "0"), lenght);
                                resStr.Append(formatStr);
                            }
                        }
                        continue;
                    }
                    ////2. Вставка хххх
                    //if (replaseStr.Contains("хххх"))
                    //{
                    //    continue;
                    //}

                    //Добавим в неизменном виде спецификаторы байтовой информации.
                    resStr.Append(replaseStr);
                }
                return resStr.ToString().Replace("\\\"", string.Empty); //заменить \"
            }
            return str;
        }


        /// <summary>
        /// Ограничить длинну строки
        /// </summary>
        private StringBuilder LimitBodySectionLenght(StringBuilder resBodyStr)
        {
            if (resBodyStr.Length < Option.RequestOption.MaxBodyLenght)
                return resBodyStr;

            var startIndex = Option.RequestOption.MaxBodyLenght;
            var lenght = resBodyStr.Length - Option.RequestOption.MaxBodyLenght;
            resBodyStr.Remove(startIndex, lenght);
            return resBodyStr;
        }


        /// <summary>
        /// Первоначальная вставка ЗАВИСИМЫХ переменных
        ///  {AddressDevice} {NByte} {NumberOfCharacters} {CRC}
        /// </summary>
        private string MakeDependentInserts(string str)
        {
            /*
              1. Вставит AddressDevice и Вычислить NumberOfCharacters и вставить.
              2. Вычислить NByte (кол-во байт между {NByte} и {CRC}) и вставить.
              3. Вычислить CRC и вставить
            */
            str = MakeAddressDevice(str);
            str = MakeNByte(str);
            str = MakeCrc(str);
            return str;
        }


        private string CreateStationsCutStr(AdInputType uit, Lang lang)
        {
            var eventNum = uit.Event?.Num;
            if (!eventNum.HasValue)
                return string.Empty;

            var stArrival = uit.StationArrival?.GetName(lang);
            var stDepart = uit.StationDeparture?.GetName(lang);
            var stations = string.Empty;
            switch (eventNum.Value)
            {
                case 0: //"ПРИБ"
                    stations = stArrival;
                    break;
                case 1:  //"ОТПР"
                    stations = stDepart;
                    break;
                case 2:   //"СТОЯНКА"
                    stations = $"{stArrival}-{stDepart}";
                    break;
            }
            return stations;
        }


        private string CreateStationsStr(AdInputType uit, Lang lang)
        {
            var stArrival = uit.StationArrival?.GetName(lang);
            var stDepart = uit.StationDeparture?.GetName(lang);
            var stations = string.Empty;
            if (!string.IsNullOrEmpty(stArrival) && !string.IsNullOrEmpty(stDepart))
            {
                stations = $"{stArrival}-{stDepart}";
            }
            return stations;
        }


        /// <summary>
        /// Математическое вычисление формулы с участием переменной rowNumber
        /// </summary>
        private string CalculateMathematicFormat(string str, int row)
        {
            var matchString = Regex.Match(str, "\\{\\((.*)\\)\\:(.*)\\}").Groups[1].Value;
            var expr = new Expression(matchString)
            {
                Parameters = { ["rowNumber"] = row }
            };
            var func = expr.ToLambda<int>();
            var arithmeticResult = func();
            var reultStr = str.Replace("(" + matchString + ")", "0");
            reultStr = string.Format(reultStr, arithmeticResult);
            return reultStr;
        }


        /// <summary>
        /// Заменить все переменные NumberOfCharacters.
        /// Вычислить N символов след. за NumberOfCharacters в кавычках
        /// </summary>
        private string MakeAddressDevice(string str)
        {
            var dict = new Dictionary<string, object>
            {
                ["AddressDevice"] =  int.TryParse(_addressDevice, out var address) ? address : 0
            };
            //ВСТАВИТЬ ПЕРЕМЕННЫЕ ИЗ СЛОВАРЯ В body
            var resStr = HelpersString.StringTemplateInsert(str, dict);
            return resStr;
        }


        private string MakeNByte(string str)
        {
            var requestFillBodyWithoutConstantCharacters = str.Replace("STX", string.Empty).Replace("ETX", string.Empty);

            //ВЫЧИСЛЯЕМ NByte---------------------------------------------------------------------------
            int lenght = 0;
            string matchString = null;
            if (Regex.Match(requestFillBodyWithoutConstantCharacters, "{Nbyte(.*)}(.*){CRC(.*)}").Success) //вычислили длинну строки между Nbyte и CRC
            {
                matchString = Regex.Match(requestFillBodyWithoutConstantCharacters, "{Nbyte(.*)}(.*){CRC(.*)}").Groups[2].Value;
                lenght = matchString.Length;
            }
            else if (Regex.Match(requestFillBodyWithoutConstantCharacters, "{Nbyte(.*)}(.*)").Success)//вычислили длинну строки от Nbyte до конца строки
            {
                matchString = Regex.Match(requestFillBodyWithoutConstantCharacters, "{Nbyte(.*)}(.*)").Groups[1].Value;
                lenght = matchString.Length;
            }

            ////ОГРАНИЧНИЕ ДЛИННЫ ПОСЫЛКИ------------------------------------------------------------------
            //var limetedStr = requestFillBodyWithoutConstantCharacters;
            //if (RequestRule.MaxLenght.HasValue && lenght >= RequestRule.MaxLenght)
            //{
            //    var removeCount = lenght - RequestRule.MaxLenght.Value;
            //    limetedStr = matchString.Remove(RequestRule.MaxLenght.Value, removeCount);
            //    lenght = limetedStr.Length;
            //    requestFillBodyWithoutConstantCharacters =
            //        requestFillBodyWithoutConstantCharacters.Replace(matchString, limetedStr);
            //}

            //ЗАПОНЯЕМ ВСЕ СЕКЦИИ ДО CRC
            var subStr = requestFillBodyWithoutConstantCharacters.Split('}');
            StringBuilder resStr = new StringBuilder();
            foreach (var s in subStr)
            {
                var replaseStr = (s.Contains("{")) ? (s + "}") : s;
                if (replaseStr.Contains("Nbyte"))
                {
                    var formatStr = string.Format(replaseStr.Replace("Nbyte", "0"), lenght);
                    resStr.Append(formatStr);
                }
                else
                {
                    resStr.Append(replaseStr);
                }
            }
            return resStr.ToString();
        }


        private string MakeCrc(string str)
        {
            var format = Option.RequestOption.Format;
            var matchString = Regex.Match(str, "(.*){CRC(.*)}").Groups[1].Value;
            matchString = matchString.Replace("\u0002", string.Empty).Replace("\u0003", string.Empty);
            var xorBytes = matchString.ConvertString2ByteArray(format);
            //вычислить CRC по правилам XOR
            if (str.Contains("CRCXor"))
            {
                byte xor = CrcCalc.CalcXor(xorBytes);
                str = string.Format(str.Replace("CRCXor", "0"), xor);
            }
            return str;
        }

        #endregion
    }


    /// <summary>
    /// Единица запроса обработанная ViewRule
    /// </summary>
    public class ViewRuleRequestModelWrapper
    {
        public IEnumerable<AdInputType> BatchedData { get; set; }    //набор входных данных на базе которых созданна StringRequest
        public string StringRequest { get; set; }                   //строка запроса, созданная по правилам RequestOption
        public RequestOption RequestOption { get; set; }            //Запрос
        public ResponseOption ResponseOption { get; set; }          //Ответ
    }
}