using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Model;
using Shared.Extensions;


namespace InputDataModel.Autodictor.ByRuleDataProviders.Rules
{
    /// <summary>
    /// Правило отображения порции даных
    /// </summary>
    public class ViewRule
    {
        #region fields

        public readonly ViewRuleOption Option;

        #endregion



        #region ctor

        public ViewRule(ViewRuleOption option)
        {
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

            var startRow = Option.StartPosition;          //TODO: {row} можно вычислить

            var startSection = "STX{AddressDevice:X2}{Nbyte:X2}";

            var body = "%StationArr=???{NumberOfCharacters:X2}??? !!!\"{StationArrival}\"!!!" +
                       "%StationDep=???{NumberOfCharacters:X2}??? !!!\"{StationDeparture}\"!!!" +
                       "%StatC=???{NumberOfCharacters:X2}??? !!!\"{StationsCut}\"!!!" +
                       "%DelayT= !!!{DelayTime}!!!" + 
                       "%ExpectedT= !!!{ExpectedTime:t}!!!";

            var endSection = " {CRCXor:X2}ETX";


            //ЗАПОЛНИТЬ НАЧАЛО ЗАПРОСА------------------------------------------------------------------------
            var addressDevice = 1;
            var resStartStr = MakeStartSectionIndependentInserts(startSection, addressDevice);

            //ЗАПОЛНИТЬ ТЕЛО ЗАПРОСА-------------------------------------------------------------------------
            var resBodyStr = new StringBuilder();
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var currentRow = startRow + i;
                var res = MakeBodySectionIndependentInserts(body, item, currentRow);
                resBodyStr.Append(res);
            }

            //ЗАПОЛНИТЬ КОНЕЦ ЗАПРОСА-----------------------------------------------------------------------
            var resEndStr = MakeEndSectionIndependentInserts(endSection);

            //КОНКАТЕНИРОВАТЬ СТРОКИ В СУМАРНУЮ СТРОКУ-------------------------------------------------------
            var resSumStr = resStartStr + resBodyStr + resEndStr;


            //ВСТАВИТЬ ЗАВИСИМЫЕ ДАННЫЕ (NumberOfCharacters)-------------------------------------------------
            var resDependencyStr = MakeDependentInserts(resSumStr);

            //ВЫЧИСЛИТЬ CRC----------------------------------------------------------------------------------






   

            return $"formatString startRow= {startRow}"; 
        }


        /// <summary>
        /// Первоначальная вставка НЕЗАВИСИМЫХ переменных в секцию START
        /// </summary>
        private string MakeStartSectionIndependentInserts(string startSection, int addressDevice)
        {

            return String.Empty;
        }


        /// <summary>
        /// Первоначальная вставка НЕЗАВИСИМЫХ переменных в секцию START
        /// </summary>
        private string MakeEndSectionIndependentInserts(string startSection)
        {

            return String.Empty;
        }


        /// <summary>
        /// Первоначальная вставка НЕЗАВИСИМЫХ переменных
        /// </summary>
        private string MakeBodySectionIndependentInserts(string body, AdInputType uit, int? currentRow)
        {
            if (body.Contains("}"))                                                           //если указанны переменные подстановки
            {
                var subStr = body.Split('}');
                StringBuilder resStr = new StringBuilder();
                int parseVal;
                foreach (var s in subStr)
                {
                    var replaseStr = (s.Contains("{")) ? (s + "}") : s;
                    var mathStr = Regex.Match(replaseStr, @"{(.*)}").Groups[1].Value;
                    var subvar= mathStr.Split(':').First();

                    if (subvar == nameof(uit.AddressDevice))
                    { 
                        if (mathStr.Contains(":")) //если указанн формат числа
                        {
                            if (int.TryParse(uit.AddressDevice, out parseVal))
                            {
                                var formatStr = string.Format(replaseStr.Replace(nameof(uit.AddressDevice), "0"), parseVal);
                                resStr.Append(formatStr);
                            }
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.AddressDevice), "0"), uit.AddressDevice);
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == "TypeName")
                    {
                        var ruTypeTrain = uit.TypeTrain;
                        var formatStr = string.Format(replaseStr.Replace("TypeName", "0"), ruTypeTrain);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == "TypeAlias")
                    {
                        var ruTypeTrain = uit.TypeTrain.Substring(0, 4);
                        var formatStr = string.Format(replaseStr.Replace("TypeAlias", "0"), ruTypeTrain);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == nameof(uit.NumberOfTrain))
                    {
                        if (mathStr.Contains(":")) //если указан формат числа
                        {
                            if (int.TryParse(uit.NumberOfTrain, out parseVal))
                            {
                                var formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), parseVal);
                                resStr.Append(formatStr);
                            }
                            else
                            {
                                var formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), " ");
                                resStr.Append(formatStr);
                            }
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), string.IsNullOrEmpty(uit.NumberOfTrain) ? " " : uit.NumberOfTrain);
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == nameof(uit.PathNumber))
                    {
                        if (mathStr.Contains(":")) //если указан формат числа
                        {
                            if (int.TryParse(uit.PathNumber, out parseVal))
                            {
                                var formatStr = string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), parseVal);
                                resStr.Append(formatStr);
                            }
                            else
                            {
                                var formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), " ");
                                resStr.Append(formatStr);
                            }
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), string.IsNullOrEmpty(uit.PathNumber) ? " " : uit.PathNumber);
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == nameof(uit.EventOld))
                    {
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.EventOld), "0"), string.IsNullOrEmpty(uit.EventOld) ? " " : uit.EventOld);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == nameof(uit.Addition))
                    {
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.Addition), "0"), !string.IsNullOrEmpty(uit.Addition) ? uit.Addition : " ");
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == "StationsCut")
                    {
                        var stationsCut = " ";
                        switch (uit.EventOld)
                        {
                            case "ПРИБ.":
                                stationsCut = (uit.StationArrival != null) ? uit.StationArrival.NameRu : " ";
                                break;

                            case "ОТПР.":
                                stationsCut = (uit.StationDeparture != null) ? uit.StationDeparture.NameRu : " ";
                                break;

                            case "СТОЯНКА":
                                stationsCut = (uit.StationArrival != null && uit.StationDeparture != null) ? $"{uit.StationArrival.NameRu}-{uit.StationDeparture.NameRu}" : " ";
                                break;
                        }
                        var formatStr = string.Format(replaseStr.Replace("StationsCut", "0"), stationsCut);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == nameof(uit.Stations))
                    {
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.Stations), "0"), string.IsNullOrEmpty(uit.Stations) ? " " : uit.Stations);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == nameof(uit.StationArrival))
                    {
                        var stationArrival = uit.StationArrival?.NameRu ?? " ";
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.StationArrival), "0"), stationArrival);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == nameof(uit.StationDeparture))
                    {
                        var stationDeparture = uit.StationDeparture?.NameRu ?? " ";
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.StationDeparture), "0"), stationDeparture);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == nameof(uit.Note))
                    {
                        var formatStr= string.Format(replaseStr.Replace(nameof(uit.Note), "0"), string.IsNullOrEmpty(uit.Note) ? " " : uit.Note);
                        resStr.Append(formatStr);
                        continue;
                    }

                    if (subvar == nameof(uit.DaysFollowingAlias))
                    {
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.DaysFollowingAlias), "0"), string.IsNullOrEmpty(uit.DaysFollowingAlias) ? " " : uit.DaysFollowingAlias);
                        resStr.Append(formatStr);
                        continue;
                    }

                    if (subvar == nameof(uit.DaysFollowing))
                    {
                        var formatStr = string.Format(replaseStr.Replace(nameof(uit.DaysFollowing), "0"), string.IsNullOrEmpty(uit.DaysFollowing) ? " " : uit.DaysFollowing);
                        resStr.Append(formatStr);
                        continue;
                    }

                    if (subvar == nameof(uit.DelayTime))
                    {
                        if (uit.DelayTime == null || uit.DelayTime.Value.TimeOfDay == TimeSpan.Zero)
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), " ");
                            resStr.Append(formatStr);
                            continue;
                        }

                        if (mathStr.Contains(":")) //если указзанн формат времени
                        {
                            var dateFormat = s.Split(':')[1]; //без закр. скобки
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : uit.DelayTime.Value.ToString(dateFormat));
                            resStr.Append(formatStr);
                        }
                        else                         //вывод в минутах
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : ((uit.DelayTime.Value.Hour * 60) + (uit.DelayTime.Value.Minute)).ToString());
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == nameof(uit.ExpectedTime))
                    {
                        if (mathStr.Contains(":")) //если указзанн формат времени
                        {
                            var dateFormat = s.Split(':')[1]; //без закр. скобки
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(dateFormat));
                            resStr.Append(formatStr);
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(CultureInfo.InvariantCulture));
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == nameof(uit.Time))
                    {
                        if (mathStr.Contains(":")) //если указанн формат времени
                        {
                            string formatStr;
                            var dateFormat = s.Split(':')[1]; //без закр. скобки
                            if (dateFormat.Contains("Sec"))   //формат задан в секундах
                            {
                                var intFormat = dateFormat.Substring(3, 2);
                                var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                            }
                            else
                            {
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (uit.Time == DateTime.MinValue) ? " " : uit.Time.ToString(dateFormat));
                            }

                            resStr.Append(formatStr);
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (uit.Time == DateTime.MinValue) ? " " : uit.Time.ToString(CultureInfo.InvariantCulture));
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == "TDepart")
                    {
                        DateTime timeDepart = DateTime.MinValue;
                        switch (uit.EventOld)
                        {
                            case "СТОЯНКА":
                                timeDepart = (uit.TransitTime != null && uit.TransitTime.ContainsKey("отпр")) ? uit.TransitTime["отпр"] : DateTime.MinValue;
                                break;

                            case "ОТПР.":
                                timeDepart = uit.Time;
                                break;
                        }

                        if (mathStr.Contains(":")) //если указанн формат времени
                        {
                            string formatStr;
                            var dateFormat = s.Split(':')[1]; //без закр. скобки
                            if (dateFormat.Contains("Sec"))   //формат задан в секундах
                            {
                                var intFormat = dateFormat.Substring(3, 2);
                                var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                                formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                            }
                            else
                            {
                                formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (timeDepart == DateTime.MinValue) ? " " : timeDepart.ToString(dateFormat));
                            }

                            resStr.Append(formatStr);
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (timeDepart == DateTime.MinValue) ? " " : timeDepart.ToString(CultureInfo.InvariantCulture));
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == "TArrival")
                    {
                        DateTime timeArrival = DateTime.MinValue;
                        switch (uit.EventOld)
                        {
                            case "СТОЯНКА":
                                timeArrival = (uit.TransitTime != null && uit.TransitTime.ContainsKey("приб")) ? uit.TransitTime["приб"] : DateTime.MinValue;
                                break;

                            case "ПРИБ.":
                                timeArrival = uit.Time;
                                break;
                        }

                        if (mathStr.Contains(":")) //если указанн формат времени
                        {
                            string formatStr;
                            var dateFormat = s.Split(':')[1]; //без закр. скобки
                            if (dateFormat.Contains("Sec"))   //формат задан в секундах
                            {
                                var intFormat = dateFormat.Substring(3, 2);
                                var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                                formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                            }
                            else
                            {
                                formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (timeArrival == DateTime.MinValue) ? " " : timeArrival.ToString(dateFormat));
                            }

                            resStr.Append(formatStr);
                        }
                        else
                        {
                            var formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (timeArrival == DateTime.MinValue) ? " " : timeArrival.ToString(CultureInfo.InvariantCulture));
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar == "Hour")
                    {
                        var formatStr = string.Format(replaseStr.Replace("Hour", "0"), DateTime.Now.Hour);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == "Minute")
                    {
                        var formatStr = string.Format(replaseStr.Replace("Minute", "0"), DateTime.Now.Minute);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == "Second")
                    {
                        var formatStr = string.Format(replaseStr.Replace("Second", "0"), DateTime.Now.Second);
                        resStr.Append(formatStr);
                        continue;
                    }


                    if (subvar == "SyncTInSec")
                    {
                        var secTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                        string formatStr;
                        if (mathStr.Contains(":")) //если указан формат времени
                        {
                            var dateFormat = s.Split(':')[1]; //без закр. скобки
                            formatStr = string.Format(replaseStr.Replace("SyncTInSec", "0"), (secTime == 0) ? " " : secTime.ToString(dateFormat));
                            resStr.Append(formatStr);
                        }
                        else
                        {
                            formatStr = string.Format(replaseStr.Replace("SyncTInSec", "0"), (secTime == 0) ? " " : secTime.ToString(CultureInfo.InvariantCulture));
                            resStr.Append(formatStr);
                        }
                        continue;
                    }


                    if (subvar.Contains("rowNumber"))
                    {
                        if (currentRow.HasValue)
                        {
                            var formatStr = CalculateMathematicFormat(replaseStr, currentRow.Value);
                            resStr.Append(formatStr);
                            continue;
                        }
                    }


                    //Добавим в неизменном виде спецификаторы байтовой информации.
                    resStr.Append(replaseStr);
                }

                return resStr.ToString();
            }

            return body;
        }




        /// <summary>
        /// Первоначальная вставка ЗАВИСИМЫХ переменных
        /// </summary>
        private string MakeDependentInserts(string str)
        {
            return String.Empty;
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