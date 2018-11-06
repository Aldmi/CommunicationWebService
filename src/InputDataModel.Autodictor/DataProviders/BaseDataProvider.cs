﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using InputDataModel.Autodictor.Model;

namespace InputDataModel.Autodictor.DataProviders
{
    public class BaseDataProvider
    {
        /// <summary>
        /// Фильтровать элементы по Contrains этого правила.
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="whereFilter">фильтрация данных</param>
        /// <param name="orderBy">упорядочевание данных по имени св-ва</param>
        /// <param name="takeItems">кол-во элементов которые нужно взять из коллекции или дополнить до этого кол-ва</param>
        /// <returns></returns>
        public IEnumerable<AdInputType> FilteredAndOrderedAndTakesItems(IEnumerable<AdInputType> inData, string whereFilter, string orderBy, int takeItems)
        {
            if(inData == null)
                return new List<AdInputType>();

            var now = DateTime.Now;
            try
            {
                //ЗАМЕНА  DateTime.Now.AddMinute(...)---------------------------
                var pattern = @"DateTime\.Now\.AddMinute\(([^()]*)\)";
                var where = whereFilter;
                where = Regex.Replace(where, pattern, x =>
                {
                    var val = x.Groups[1].Value;
                    if (int.TryParse(val, out var min))
                    {
                        var date = now.AddMinutes(min);
                        return $"DateTime({date.Year}, {date.Month}, {date.Day}, {date.Hour}, {date.Minute}, 0)";
                    }
                    return x.Value;
                });
                //ЗАМЕНА  DateTime.Now----------------------------------------
                pattern = @"DateTime.Now";
                where = Regex.Replace(where, pattern, x =>
                {
                    var date = now;
                    return $"DateTime({date.Year}, {date.Month}, {date.Day}, {date.Hour}, {date.Minute}, 0)";
                });
                //ПРИМЕНИТЬ ФИЛЬТР И УПОРЯДОЧЕВАНИЕ
                var filtred = inData.AsQueryable().Where(where).OrderBy(orderBy).ToList();
                //ВЗЯТЬ TakeItems ИЛИ ДОПОЛНИТЬ ДО TakeItems.
                var takedItems= new AdInputType[takeItems];
                var endPosition= (takeItems < filtred.Count) ? takeItems : filtred.Count;
                filtred.CopyTo(0, takedItems, 0, endPosition);
                return takedItems;
            }
            catch (Exception ex)
            {
                //LOG
                return null;
            }
        }
    }
}