using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;
using Newtonsoft.Json;

namespace InputDataModel.Autodictor.DataProviders
{
    public class BaseDataProvider
    {
        /// <summary>
        /// Подходит ли обработчик для обработки команды.
        /// </summary>
        /// <param name="command">имя команды</param>
        /// <param name="handlerName">имя обработчика</param>
        /// <returns></returns>
        public bool IsCommandHandler(Command4Device command, string handlerName)
        {
            var commandName = $"Command_{command.ToString()}";  //Command_On, Command_Off, Command_Restart, Command_Clear
            return commandName.Equals(handlerName);
        }


        /// <summary>
        /// Подходит ли обработчик для обработки данных.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data4Handle"></param>
        /// <returns></returns>
        public bool IsDataHandler(Command4Device command, IEnumerable<AdInputType> data4Handle)
        {
            return (command == Command4Device.None) && (data4Handle != null) && (data4Handle.Any());

        }


        /// <summary>
        /// Фильтровать элементы по Contrains этого правила.
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="whereFilter">фильтрация данных</param>
        /// <param name="orderBy">упорядочевание данных по имени св-ва</param>
        /// <param name="takeItems">кол-во элементов которые нужно взять из коллекции или дополнить до этого кол-ва</param>
        /// <param name="defaultItemJson">дефолтное значение AdInputType, дополняется до TakeIteme этим значением</param>
        /// <returns></returns>
        public IEnumerable<AdInputType> FilteredAndOrderedAndTakesItems(IEnumerable<AdInputType> inData, string whereFilter, string orderBy, int takeItems, string defaultItemJson)
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

                var defaultItem= GetDefaultAdInputType(defaultItemJson);
                var takedItems= Enumerable.Repeat(defaultItem, takeItems).ToArray();
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


        private AdInputType GetDefaultAdInputType(string defaultItemJson)
        {
            var adInputType = JsonConvert.DeserializeObject<AdInputType>(defaultItemJson);
            return adInputType;
        }
    }
}