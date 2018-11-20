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
        /// Создать строку запроса, подставив в форматную строку запроса значения переменных из списка items.
        /// </summary>
        /// <param name="items">элементы прошедшие фильтрацию для правила</param>
        /// <returns>строку запроса и батч данных в обертке </returns>
        public IEnumerable<ViewRuleRequestModelWrapper> GetRequestString(List<AdInputType> items)
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
            var startSection = "\u0002{AddressDevice:X2}{Nbyte:D2}";

            var body = "%StationArr= {NumberOfCharacters:X2} \\\"{StationArrival}\\\"" +
                       "%TypeName= {TypeName}" +
                       "%NumberOfTrain= {NumberOfTrain}" +
                       "%PathNumber= {NumberOfCharacters:X2} \\\"{PathNumber:D5}\\\"" +
                       "%Stations= {Stations} " +
                       "%TArrival= {TArrival:t}" +
                       "%rowNumb= {(rowNumber*11-11):X3}" +
                       "%StationDep= {NumberOfCharacters:X2} \\\"{StationDeparture}\\\"" +
                       "%StatC= {NumberOfCharacters:X2} \\\"{StationsCut}\\\"" +
                       "%DelayT= {DelayTime}" +
                       "%ExpectedT= {ExpectedTime:t}";

            //var body = "%StationArr= {NumberOfCharacters:X2} \\\"{StationArrival}\\\"" +
            //           "%TypeName= {TypeName}" +
            //           "%NumberOfTrain= {NumberOfTrain}" +
            //           "%PathNumber= {PathNumber:D5}" +
            //           "%Stations= {Stations} " +
            //           "%TArrival= {TArrival:t}" +
            //           "%rowNumb= {((rowNumber*11-11)+5):X3}" +
            //           "%StationDep= {NumberOfCharacters:X2} \\\"{StationDeparture}\\\"" +
            //           "%StatC= {NumberOfCharacters:X2} \\\"{StationsCut}\\\"";


            //var body = "%StationArr={NumberOfCharacters:D2} \\\"{StationArrival}\\\" NumberOfTr={NumberOfTrain}";
            var endSection = " {CRCXor:X2}\u0003";
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
            //resSumStr содержит только ЗАВИСИМЫЕ данные: {AddressDevice} {NByte} {NumberOfCharacters {CRC}}
            var resSumStr = startSection + limitBodyStr + endSection;

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
        /// Первоначальная вставка НЕЗАВИСИМЫХ переменных V2
        /// </summary> 
        //private string MakeBodySectionIndependentInserts(string body, AdInputType uit, int? currentRow)
        //{
        //    var lang = uit.Lang;
        //    if (body.Contains("}"))                                                           //если указанны переменные подстановки
        //    {
        //        var subStr = body.Split('}');
        //        var resStr = new StringBuilder();
        //        foreach (var s in subStr)
        //        {
        //            var replaseStr = (s.Contains("{")) ? (s + "}") : s;
        //            var mathStr = Regex.Match(replaseStr, @"{(.*)}").Groups[1].Value;
        //            var subvar = mathStr.Split(':').First();
        //            int parseVal;
        //            string formatStr;
        //            switch (subvar)
        //            {
        //                case "TypeName":
        //                    var typeTrain = uit.TrainType.GetName(lang);
        //                    formatStr = string.Format(replaseStr.Replace("TypeName", "0"), typeTrain);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.NumberOfTrain):
        //                    if (mathStr.Contains(":")) //если указан формат числа
        //                    {
        //                        formatStr = int.TryParse(uit.NumberOfTrain, out parseVal) ?
        //                            string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), parseVal) :
        //                            string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), " ");
        //                    }
        //                    else
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), string.IsNullOrEmpty(uit.NumberOfTrain) ? " " : uit.NumberOfTrain);
        //                    }
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.PathNumber):
        //                    if (mathStr.Contains(":")) //если указан формат числа
        //                    {
        //                        formatStr = int.TryParse(uit.PathNumber, out parseVal) ?
        //                            string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), parseVal) :
        //                            string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), " ");
        //                    }
        //                    else
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), string.IsNullOrEmpty(uit.PathNumber) ? " " : uit.PathNumber);
        //                    }
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.Event):
        //                    var eventTrain = uit.Event?.GetName(lang);
        //                    formatStr = string.Format(replaseStr.Replace(nameof(uit.Event), "0"), string.IsNullOrEmpty(eventTrain) ? " " : eventTrain);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.Addition):
        //                    var addition = uit.Addition?.GetName(lang);
        //                    formatStr = string.Format(replaseStr.Replace(nameof(uit.Addition), "0"), !string.IsNullOrEmpty(addition) ? addition : " ");
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "Stations":
        //                    var stations = CreateStationsStr(uit, lang);
        //                    formatStr = string.Format(replaseStr.Replace("Stations", "0"), string.IsNullOrEmpty(stations) ? " " : stations);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "StationsCut":
        //                    var stationsCut = CreateStationsCutStr(uit, lang);
        //                    formatStr = string.Format(replaseStr.Replace("StationsCut", "0"), string.IsNullOrEmpty(stationsCut) ? " " : stationsCut);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "TypeAlias":
        //                    var typeAlias = uit.TrainType?.GetNameAlias(lang);
        //                    formatStr = string.Format(replaseStr.Replace("TypeAlias", "0"), string.IsNullOrEmpty(typeAlias) ? " " : typeAlias);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.StationArrival):
        //                    var stationArrival = uit.StationArrival?.GetName(lang) ?? " ";
        //                    formatStr = string.Format(replaseStr.Replace(nameof(uit.StationArrival), "0"), stationArrival);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.StationDeparture):
        //                    var stationDeparture = uit.StationDeparture?.GetName(lang) ?? " ";
        //                    formatStr = string.Format(replaseStr.Replace(nameof(uit.StationDeparture), "0"), stationDeparture);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.Note):
        //                    var note = uit.Note?.GetName(lang);
        //                    formatStr = string.Format(replaseStr.Replace(nameof(uit.Note), "0"), string.IsNullOrEmpty(note) ? " " : note);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "DaysFollowing":
        //                    var daysFollowing = uit.DaysFollowing?.GetName(lang);
        //                    formatStr = string.Format(replaseStr.Replace("DaysFollowing", "0"), string.IsNullOrEmpty(daysFollowing) ? " " : daysFollowing);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "DaysFollowingAlias":
        //                    var daysFollowingAlias = uit.DaysFollowing?.GetNameAlias(lang);
        //                    formatStr = string.Format(replaseStr.Replace("DaysFollowingAlias", "0"), string.IsNullOrEmpty(daysFollowingAlias) ? " " : daysFollowingAlias);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case nameof(uit.DelayTime):
        //                    if (uit.DelayTime == null || uit.DelayTime.Value.TimeOfDay == TimeSpan.Zero)
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), " ");
        //                        resStr.Append(formatStr);
        //                        continue;
        //                    }
        //                    if (mathStr.Contains(":")) //если указзанн формат времени
        //                    {
        //                        var dateFormat = s.Split(':')[1]; //без закр. скобки
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : uit.DelayTime.Value.ToString(dateFormat));
        //                        resStr.Append(formatStr);
        //                    }
        //                    else                         //вывод в минутах
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : ((uit.DelayTime.Value.Hour * 60) + (uit.DelayTime.Value.Minute)).ToString());
        //                        resStr.Append(formatStr);
        //                    }
        //                    break;

        //                case nameof(uit.ExpectedTime):
        //                    if (mathStr.Contains(":")) //если указзанн формат времени
        //                    {
        //                        var dateFormat = s.Split(':')[1]; //без закр. скобки
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(dateFormat));
        //                        resStr.Append(formatStr);
        //                    }
        //                    else
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(CultureInfo.InvariantCulture));
        //                        resStr.Append(formatStr);
        //                    }
        //                    break;

        //                case "TArrival":
        //                    if (!uit.ArrivalTime.HasValue)
        //                        break;
        //                    var timeArrival = uit.ArrivalTime.Value;
        //                    if (mathStr.Contains(":")) //если указанн формат времени
        //                    {
        //                        var dateFormat = s.Split(':')[1]; //без закр. скобки
        //                        if (dateFormat.Contains("Sec"))   //формат задан в секундах
        //                        {
        //                            var intFormat = dateFormat.Substring(3, 2);
        //                            var intValue = (timeArrival.Hour * 3600 + timeArrival.Minute * 60);
        //                            formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
        //                        }
        //                        else
        //                        {
        //                            formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (timeArrival == DateTime.MinValue) ? " " : timeArrival.ToString(dateFormat));
        //                        }
        //                        resStr.Append(formatStr);
        //                    }
        //                    else
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (timeArrival == DateTime.MinValue) ? " " : timeArrival.ToString(CultureInfo.InvariantCulture));
        //                        resStr.Append(formatStr);
        //                    }
        //                    break;

        //                case "TDepart":
        //                    if (!uit.DepartureTime.HasValue)
        //                        break;
        //                    var timeDepart = uit.DepartureTime.Value;
        //                    if (mathStr.Contains(":")) //если указанн формат времени
        //                    {
        //                        var dateFormat = s.Split(':')[1]; //без закр. скобки
        //                        if (dateFormat.Contains("Sec"))   //формат задан в секундах
        //                        {
        //                            var intFormat = dateFormat.Substring(3, 2);
        //                            var intValue = (timeDepart.Hour * 3600 + timeDepart.Minute * 60);
        //                            formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
        //                        }
        //                        else
        //                        {
        //                            formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (timeDepart == DateTime.MinValue) ? " " : timeDepart.ToString(dateFormat));
        //                        }
        //                        resStr.Append(formatStr);
        //                    }
        //                    else
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (timeDepart == DateTime.MinValue) ? " " : timeDepart.ToString(CultureInfo.InvariantCulture));
        //                        resStr.Append(formatStr);
        //                    }
        //                    break;

        //                case "Hour":
        //                    formatStr = string.Format(replaseStr.Replace("Hour", "0"), DateTime.Now.Hour);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "Minute":
        //                    formatStr = string.Format(replaseStr.Replace("Minute", "0"), DateTime.Now.Minute);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "Second":
        //                    formatStr = string.Format(replaseStr.Replace("Second", "0"), DateTime.Now.Second);
        //                    resStr.Append(formatStr);
        //                    break;

        //                case "SyncTInSec":
        //                    var secTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        //                    if (mathStr.Contains(":")) //если указан формат времени
        //                    {
        //                        var dateFormat = s.Split(':')[1]; //без закр. скобки
        //                        formatStr = string.Format(replaseStr.Replace("SyncTInSec", "0"), (secTime == 0) ? " " : secTime.ToString(dateFormat));
        //                        resStr.Append(formatStr);
        //                    }
        //                    else
        //                    {
        //                        formatStr = string.Format(replaseStr.Replace("SyncTInSec", "0"), (secTime == 0) ? " " : secTime.ToString(CultureInfo.InvariantCulture));
        //                        resStr.Append(formatStr);
        //                    }
        //                    break;

        //                default:
        //                    if (subvar.Contains("rowNumber"))
        //                    {
        //                        if (currentRow.HasValue)
        //                        {
        //                            formatStr = CalculateMathematicFormat(replaseStr, currentRow.Value);
        //                            resStr.Append(formatStr);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Добавим в неизменном виде спецификаторы байтовой информации.
        //                        resStr.Append(replaseStr);
        //                    }
        //                    break;
        //            }
        //        }
        //        return resStr.ToString();
        //    }
        //    return body;
        //}


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
                ["AddressDevice"] = _addressDevice,
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