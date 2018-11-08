using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Model;
using NCalc;
using Shared.Extensions;

namespace InputDataModel.Autodictor.DataProviders.ByRuleDataProviders.Rules
{
    /// <summary>
    /// Правило отображения порции даных
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
            var startSection = "STX{AddressDevice:X2}{Nbyte:X2}";

            var body = "%StationArr=???{NumberOfCharacters:X2}??? !!!\"{StationArrival}\"!!!" +
                       "%TypeName=???{TypeName}???" +
                       "%NumberOfTrain=???{NumberOfTrain}???" +
                       "%PathNumber=???{PathNumber:D5}???" +
                       "%01000018{(rowNumber*11-11):X3}" +
                       "%StationDep=???{NumberOfCharacters:X2}??? !!!\"{StationDeparture}\"!!!" +
                       "%StatC=???{NumberOfCharacters:X2}??? !!!\"{StationsCut}\"!!!" +
                       "%DelayT= !!!{DelayTime}!!!" + 
                       "%ExpectedT= !!!{ExpectedTime:t}!!!";

            var endSection = " {CRCXor:X2}ETX";
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

            //КОНКАТЕНИРОВАТЬ СТРОКИ В СУММАРНУЮ СТРОКУ-------------------------------------------------------
            //resSumStr содержит только ЗАВИСИМЫЕ данные: {AddressDevice} {NByte} {NumberOfCharacters {CRC}}
            var resSumStr = startSection + resBodyStr + endSection;

            //ВСТАВИТЬ ЗАВИСИМЫЕ ДАННЫЕ ({AddressDevice} {NByte} {NumberOfCharacters {CRC})-------------------------------------------------
            var resDependencyStr = MakeDependentInserts(resSumStr);

            return resDependencyStr; 
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
            var lang = uit.Lang;
            if (body.Contains("}"))                                                           //если указанны переменные подстановки
            {
                var subStr = body.Split('}');
                StringBuilder resStr = new StringBuilder();
                int parseVal;
                foreach (var s in subStr)
                {
                    var replaseStr = (s.Contains("{")) ? (s + "}") : s;
                    var mathStr = Regex.Match(replaseStr, @"{(.*)}").Groups[1].Value;
                    var subvar = mathStr.Split(':').First();

                    switch (subvar)
                    {
                        case "TypeName":
                            var ruTypeTrain = uit.TrainType;
                            var formatStr = string.Format(replaseStr.Replace("TypeName", "0"), ruTypeTrain);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.NumberOfTrain):
                            if (mathStr.Contains(":")) //если указан формат числа
                            {
                                formatStr = int.TryParse(uit.NumberOfTrain, out parseVal) ?
                                    string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), parseVal) :
                                    string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), " ");
                            }
                            else
                            {
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), string.IsNullOrEmpty(uit.NumberOfTrain) ? " " : uit.NumberOfTrain);
                            }
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.PathNumber):
                            if (mathStr.Contains(":")) //если указан формат числа
                            {
                                formatStr = int.TryParse(uit.PathNumber, out parseVal) ?
                                    string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), parseVal) :
                                    string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), " ");
                            }
                            else
                            {
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), string.IsNullOrEmpty(uit.PathNumber) ? " " : uit.PathNumber);
                            }
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.Event):
                            formatStr = string.Format(replaseStr.Replace(nameof(uit.Event), "0"), string.IsNullOrEmpty(uit.Event.NameRu) ? " " : uit.Event);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.Addition):
                             formatStr = string.Format(replaseStr.Replace(nameof(uit.Addition), "0"), !string.IsNullOrEmpty(uit.Addition) ? uit.Addition : " ");
                             resStr.Append(formatStr);
                            break;


                        case "StationsCut":
                            //var stationsCut = " ";
                            //switch (uit.EventOld)
                            //{
                            //    case "ПРИБ.":
                            //        stationsCut = (uit.StationArrival != null) ? uit.StationArrival.NameRu : " ";
                            //        break;

                            //    case "ОТПР.":
                            //        stationsCut = (uit.StationDeparture != null) ? uit.StationDeparture.NameRu : " ";
                            //        break;

                            //    case "СТОЯНКА":
                            //        stationsCut = (uit.StationArrival != null && uit.StationDeparture != null) ? $"{uit.StationArrival.NameRu}-{uit.StationDeparture.NameRu}" : " ";
                            //        break;
                            //}
                            //var formatStr = string.Format(replaseStr.Replace("StationsCut", "0"), stationsCut);
                            //resStr.Append(formatStr);
                            break;

                        case nameof(uit.StationArrival):
                            var stationArrival = uit.StationArrival?.NameRu ?? " ";
                            formatStr = string.Format(replaseStr.Replace(nameof(uit.StationArrival), "0"), stationArrival);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.StationDeparture):
                            var stationDeparture = uit.StationDeparture?.NameRu ?? " ";
                            formatStr = string.Format(replaseStr.Replace(nameof(uit.StationDeparture), "0"), stationDeparture);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.Note):
                            formatStr = string.Format(replaseStr.Replace(nameof(uit.Note), "0"), string.IsNullOrEmpty(uit.Note) ? " " : uit.Note);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.DaysFollowingAlias):
                            formatStr = string.Format(replaseStr.Replace(nameof(uit.DaysFollowingAlias), "0"), string.IsNullOrEmpty(uit.DaysFollowingAlias) ? " " : uit.DaysFollowingAlias);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.DaysFollowing):
                            formatStr = string.Format(replaseStr.Replace(nameof(uit.DaysFollowing), "0"), string.IsNullOrEmpty(uit.DaysFollowing) ? " " : uit.DaysFollowing);
                            resStr.Append(formatStr);
                            break;

                        case nameof(uit.DelayTime):
                            if (uit.DelayTime == null || uit.DelayTime.Value.TimeOfDay == TimeSpan.Zero)
                            {
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), " ");
                                resStr.Append(formatStr);
                                continue;
                            }

                            if (mathStr.Contains(":")) //если указзанн формат времени
                            {
                                var dateFormat = s.Split(':')[1]; //без закр. скобки
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : uit.DelayTime.Value.ToString(dateFormat));
                                resStr.Append(formatStr);
                            }
                            else                         //вывод в минутах
                            {
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : ((uit.DelayTime.Value.Hour * 60) + (uit.DelayTime.Value.Minute)).ToString());
                                resStr.Append(formatStr);
                            }
                            break;

                        case nameof(uit.ExpectedTime):
                            if (mathStr.Contains(":")) //если указзанн формат времени
                            {
                                var dateFormat = s.Split(':')[1]; //без закр. скобки
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(dateFormat));
                                resStr.Append(formatStr);
                            }
                            else
                            {
                                formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(CultureInfo.InvariantCulture));
                                resStr.Append(formatStr);
                            }
                            break;

                        //case nameof(uit.Time):
                        //    if (mathStr.Contains(":")) //если указанн формат времени
                        //    {
                        //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                        //        if (dateFormat.Contains("Sec"))   //формат задан в секундах
                        //        {
                        //            var intFormat = dateFormat.Substring(3, 2);
                        //            var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                        //            formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                        //        }
                        //        else
                        //        {
                        //            formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (uit.Time == DateTime.MinValue) ? " " : uit.Time.ToString(dateFormat));
                        //        }           
                        //    }
                        //    else
                        //    {
                        //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (uit.Time == DateTime.MinValue) ? " " : uit.Time.ToString(CultureInfo.InvariantCulture));
                        //    }
                        //    resStr.Append(formatStr);
                        //    break;

                        case "4":
                            break;

                        case "6":
                            break;

                        case "7":
                            break;

                        case "8":
                            break;

                        case "9":
                            break;

                        default:
                            if (subvar.Contains("rowNumber"))
                            {
                                if (currentRow.HasValue)
                                {
                                    formatStr = CalculateMathematicFormat(replaseStr, currentRow.Value);
                                    resStr.Append(formatStr);
                                }
                            }
                            break;
                    }



                    //if (subvar == nameof(uit.AddressDevice))
                    //{
                    //    if (mathStr.Contains(":")) //если указанн формат числа
                    //    {
                    //        if (int.TryParse(uit.AddressDevice, out parseVal))
                    //        {
                    //            var formatStr = string.Format(replaseStr.Replace(nameof(uit.AddressDevice), "0"), parseVal);
                    //            resStr.Append(formatStr);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.AddressDevice), "0"), uit.AddressDevice);
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}

                    //if (subvar == "TypeName")
                    //{
                    //    var ruTypeTrain = uit.TypeTrain;
                    //    var formatStr = string.Format(replaseStr.Replace("TypeName", "0"), ruTypeTrain);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}

                    //if (subvar == "TypeAlias")
                    //{
                    //    var ruTypeTrain = uit.TypeTrain.Substring(0, 4);
                    //    var formatStr = string.Format(replaseStr.Replace("TypeAlias", "0"), ruTypeTrain);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}

                    //if (subvar == nameof(uit.NumberOfTrain))
                    //{
                    //    string formatStr;
                    //    if (mathStr.Contains(":")) //если указан формат числа
                    //    {
                    //        formatStr = int.TryParse(uit.NumberOfTrain, out parseVal) ?
                    //            string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), parseVal) :
                    //            string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), " ");
                    //    }
                    //    else
                    //    {
                    //        formatStr = string.Format(replaseStr.Replace(nameof(uit.NumberOfTrain), "0"), string.IsNullOrEmpty(uit.NumberOfTrain) ? " " : uit.NumberOfTrain);
                    //    }
                    //    resStr.Append(formatStr);
                    //}

                    //if (subvar == nameof(uit.PathNumber))
                    //{
                    //    string formatStr;
                    //    if (mathStr.Contains(":")) //если указан формат числа
                    //    {
                    //        formatStr = int.TryParse(uit.PathNumber, out parseVal) ?
                    //            string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), parseVal) :
                    //            string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), " ");
                    //    }
                    //    else
                    //    {
                    //        formatStr = string.Format(replaseStr.Replace(nameof(uit.PathNumber), "0"), string.IsNullOrEmpty(uit.PathNumber) ? " " : uit.PathNumber);
                    //    }
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}

                    //if (subvar == nameof(uit.Event))
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.Event), "0"), string.IsNullOrEmpty(uit.Event) ? " " : uit.Event);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.Addition))
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.Addition), "0"), !string.IsNullOrEmpty(uit.Addition) ? uit.Addition : " ");
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == "StationsCut")
                    //{
                    //    var stationsCut = " ";
                    //    switch (uit.EventOld)
                    //    {
                    //        case "ПРИБ.":
                    //            stationsCut = (uit.StationArrival != null) ? uit.StationArrival.NameRu : " ";
                    //            break;

                    //        case "ОТПР.":
                    //            stationsCut = (uit.StationDeparture != null) ? uit.StationDeparture.NameRu : " ";
                    //            break;

                    //        case "СТОЯНКА":
                    //            stationsCut = (uit.StationArrival != null && uit.StationDeparture != null) ? $"{uit.StationArrival.NameRu}-{uit.StationDeparture.NameRu}" : " ";
                    //            break;
                    //    }
                    //    var formatStr = string.Format(replaseStr.Replace("StationsCut", "0"), stationsCut);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.Stations))
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.Stations), "0"), string.IsNullOrEmpty(uit.Stations) ? " " : uit.Stations);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.StationArrival))
                    //{
                    //    var stationArrival = uit.StationArrival?.NameRu ?? " ";
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.StationArrival), "0"), stationArrival);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.StationDeparture))
                    //{
                    //    var stationDeparture = uit.StationDeparture?.NameRu ?? " ";
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.StationDeparture), "0"), stationDeparture);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.Note))
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.Note), "0"), string.IsNullOrEmpty(uit.Note) ? " " : uit.Note);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}

                    //if (subvar == nameof(uit.DaysFollowingAlias))
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.DaysFollowingAlias), "0"), string.IsNullOrEmpty(uit.DaysFollowingAlias) ? " " : uit.DaysFollowingAlias);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}

                    //if (subvar == nameof(uit.DaysFollowing))
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace(nameof(uit.DaysFollowing), "0"), string.IsNullOrEmpty(uit.DaysFollowing) ? " " : uit.DaysFollowing);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}

                    //if (subvar == nameof(uit.DelayTime))
                    //{
                    //    if (uit.DelayTime == null || uit.DelayTime.Value.TimeOfDay == TimeSpan.Zero)
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), " ");
                    //        resStr.Append(formatStr);
                    //        continue;
                    //    }

                    //    if (mathStr.Contains(":")) //если указзанн формат времени
                    //    {
                    //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : uit.DelayTime.Value.ToString(dateFormat));
                    //        resStr.Append(formatStr);
                    //    }
                    //    else                         //вывод в минутах
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.DelayTime), "0"), (uit.DelayTime == DateTime.MinValue) ? " " : ((uit.DelayTime.Value.Hour * 60) + (uit.DelayTime.Value.Minute)).ToString());
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.ExpectedTime))
                    //{
                    //    if (mathStr.Contains(":")) //если указзанн формат времени
                    //    {
                    //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(dateFormat));
                    //        resStr.Append(formatStr);
                    //    }
                    //    else
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.ExpectedTime), "0"), (uit.ExpectedTime == DateTime.MinValue) ? " " : uit.ExpectedTime.ToString(CultureInfo.InvariantCulture));
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}


                    //if (subvar == nameof(uit.Time))
                    //{
                    //    if (mathStr.Contains(":")) //если указанн формат времени
                    //    {
                    //        string formatStr;
                    //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                    //        if (dateFormat.Contains("Sec"))   //формат задан в секундах
                    //        {
                    //            var intFormat = dateFormat.Substring(3, 2);
                    //            var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                    //            formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                    //        }
                    //        else
                    //        {
                    //            formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (uit.Time == DateTime.MinValue) ? " " : uit.Time.ToString(dateFormat));
                    //        }

                    //        resStr.Append(formatStr);
                    //    }
                    //    else
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace(nameof(uit.Time), "0"), (uit.Time == DateTime.MinValue) ? " " : uit.Time.ToString(CultureInfo.InvariantCulture));
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}


                    //if (subvar == "TDepart")
                    //{
                    //    DateTime timeDepart = DateTime.MinValue;
                    //    switch (uit.EventOld)
                    //    {
                    //        case "СТОЯНКА":
                    //            timeDepart = (uit.TransitTime != null && uit.TransitTime.ContainsKey("отпр")) ? uit.TransitTime["отпр"] : DateTime.MinValue;
                    //            break;

                    //        case "ОТПР.":
                    //            timeDepart = uit.Time;
                    //            break;
                    //    }

                    //    if (mathStr.Contains(":")) //если указанн формат времени
                    //    {
                    //        string formatStr;
                    //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                    //        if (dateFormat.Contains("Sec"))   //формат задан в секундах
                    //        {
                    //            var intFormat = dateFormat.Substring(3, 2);
                    //            var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                    //            formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                    //        }
                    //        else
                    //        {
                    //            formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (timeDepart == DateTime.MinValue) ? " " : timeDepart.ToString(dateFormat));
                    //        }

                    //        resStr.Append(formatStr);
                    //    }
                    //    else
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace("TDepart", "0"), (timeDepart == DateTime.MinValue) ? " " : timeDepart.ToString(CultureInfo.InvariantCulture));
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}


                    //if (subvar == "TArrival")
                    //{
                    //    DateTime timeArrival = DateTime.MinValue;
                    //    switch (uit.EventOld)
                    //    {
                    //        case "СТОЯНКА":
                    //            timeArrival = (uit.TransitTime != null && uit.TransitTime.ContainsKey("приб")) ? uit.TransitTime["приб"] : DateTime.MinValue;
                    //            break;

                    //        case "ПРИБ.":
                    //            timeArrival = uit.Time;
                    //            break;
                    //    }

                    //    if (mathStr.Contains(":")) //если указанн формат времени
                    //    {
                    //        string formatStr;
                    //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                    //        if (dateFormat.Contains("Sec"))   //формат задан в секундах
                    //        {
                    //            var intFormat = dateFormat.Substring(3, 2);
                    //            var intValue = (uit.Time.Hour * 3600 + uit.Time.Minute * 60);
                    //            formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (intValue == 0) ? " " : intValue.ToString(intFormat));
                    //        }
                    //        else
                    //        {
                    //            formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (timeArrival == DateTime.MinValue) ? " " : timeArrival.ToString(dateFormat));
                    //        }

                    //        resStr.Append(formatStr);
                    //    }
                    //    else
                    //    {
                    //        var formatStr = string.Format(replaseStr.Replace("TArrival", "0"), (timeArrival == DateTime.MinValue) ? " " : timeArrival.ToString(CultureInfo.InvariantCulture));
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}


                    //if (subvar == "Hour")
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace("Hour", "0"), DateTime.Now.Hour);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == "Minute")
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace("Minute", "0"), DateTime.Now.Minute);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == "Second")
                    //{
                    //    var formatStr = string.Format(replaseStr.Replace("Second", "0"), DateTime.Now.Second);
                    //    resStr.Append(formatStr);
                    //    continue;
                    //}


                    //if (subvar == "SyncTInSec")
                    //{
                    //    var secTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                    //    string formatStr;
                    //    if (mathStr.Contains(":")) //если указан формат времени
                    //    {
                    //        var dateFormat = s.Split(':')[1]; //без закр. скобки
                    //        formatStr = string.Format(replaseStr.Replace("SyncTInSec", "0"), (secTime == 0) ? " " : secTime.ToString(dateFormat));
                    //        resStr.Append(formatStr);
                    //    }
                    //    else
                    //    {
                    //        formatStr = string.Format(replaseStr.Replace("SyncTInSec", "0"), (secTime == 0) ? " " : secTime.ToString(CultureInfo.InvariantCulture));
                    //        resStr.Append(formatStr);
                    //    }
                    //    continue;
                    //}


                    //if (subvar.Contains("rowNumber"))
                    //{
                    //    if (currentRow.HasValue)
                    //    {
                    //        var formatStr = CalculateMathematicFormat(replaseStr, currentRow.Value);
                    //        resStr.Append(formatStr);
                    //        continue;
                    //    }
                    //}


                    ////Добавим в неизменном виде спецификаторы байтовой информации.
                    //resStr.Append(replaseStr);
                }

                return resStr.ToString();
            }

            return body;
        }




        /// <summary>
        /// Первоначальная вставка ЗАВИСИМЫХ переменных
        ///  {AddressDevice} {NByte} {NumberOfCharacters} {CRC}
        /// </summary>
        private string MakeDependentInserts(string str)
        {
            /*
              1. Вставить AddressDevice
              2. Вычислить NumberOfCharacters и вставить.
              3. Вычислить NByte (кол-во байт между {NByte} и {CRC}) и вставить.
              4. Вычислить CRC и вставвить
              5. Получилась сумарная строка в которой могли остаться КОНСТАНТНЫЕ СИМВОЛЫ STX, ETX, они заменяются уже при преобразовании строки
             */

            return String.Empty;
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