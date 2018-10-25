using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;
using System.Linq.Dynamic.Core;

namespace InputDataModel.Autodictor.ByRuleDataProviders.Rules
{
    public class Rule
    {
        #region fields

        public readonly RuleOption Option;

        #endregion



        #region ctor

        public Rule(RuleOption option)
        {
            Option = option;
            ViewRules= option.ViewRules.Select(opt=> new ViewRule(Option.AddressDevice, opt)).ToList();
        }

        #endregion



        #region prop

        public IEnumerable<ViewRule> ViewRules { get; set; }

        #endregion




        #region Filter

        /// <summary>
        /// Фильтровать элементы по Contrains этого правила.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public IEnumerable<AdInputType> FilteredAndOrderedAndTakesItems(IEnumerable<AdInputType> inData)
        {
            if(inData == null)
                return new List<AdInputType>();

            var now = DateTime.Now;
            try
            {
                //ЗАМЕНА  DateTime.Now.AddMinute(...)---------------------------
                var pattern = @"DateTime\.Now\.AddMinute\(([^()]*)\)";
                var where = Option.WhereFilter;
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
                var filtred = inData.AsQueryable().Where(where).OrderBy(Option.OrderBy).ToList();
                //ВЗЯТЬ TakeItems ИЛИ ДОПОЛНИТЬ ДО TakeItems.
                var takedItems= new AdInputType[Option.TakeItems];
                var endPosition= (Option.TakeItems < filtred.Count) ? Option.TakeItems : filtred.Count;
                filtred.CopyTo(0, takedItems, 0, endPosition);
                return takedItems;
            }
            catch (Exception ex)
            {
                //LOG
                return null;
            }
        }



        /// <summary>
        /// Проверка обрабатывает ли это правило команда
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool IsCommand(Command4Device command)
        {
            var ruleName = $"Command_{command.ToString()}";  //Command_On, Command_Off, Command_Restart, Command_Clear
            return !ruleName.Equals("Command_None");
        }

        #endregion



        #region StringRequest





        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из команды.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string CreateStringRequest(Command4Device command)
        {
            return "Body"; //Option.RequestOption.Body;
        }

        #endregion
    }
}