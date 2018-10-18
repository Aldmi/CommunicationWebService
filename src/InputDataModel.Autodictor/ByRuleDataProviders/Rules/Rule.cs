using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;

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
            ViewRules= option.ViewRules.Select(opt=> new ViewRule(opt)).ToList();
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

            //ЗАМЕНА  DateTime.Now.AddMinute(...)-------------------
           //string pattern = @"DateTime\.Now\.AddMinute\(([^()]*)\)";
           //var where = Option.
           // var result = Regex.Replace(where, pattern, x =>
           // {
           //     var val = x.Groups[1].Value;
           //     if (int.TryParse(val, out var min))
           //     {
           //         var date = now.AddMinutes(min);
           //         return $"DateTime({date.Year}, {date.Month}, {date.Day}, {date.Hour}, {date.Minute}, 0)";
           //     }
           //     return x.Value;
           // });


            var chekedItems = inData.Where(CheckItem);   
            return chekedItems;
        }


        public IEnumerable<AdInputType> OrderItems(IEnumerable<AdInputType> inData)
        {


            return inData;
        }


        public IEnumerable<AdInputType> TakeItems(IEnumerable<AdInputType> inData)
        {


            return inData;
        }


        /// <summary>
        /// Проверяет элемент под ограничения правила.
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        private bool CheckItem(AdInputType inputType)
        {
            //TODO: проверить Contrains для этого элемента

            return true;
        }


        /// <summary>
        /// Проверка обрабатывает ли это правило команда
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool CheckCommand(Command4Device command)
        {
            var ruleName = $"Command_{command.ToString()}";  //Command_On, Command_Off, Command_Restart, Command_Clear
            return !ruleName.Equals("Command_None");
        }

        #endregion


        #region OrderBy

        

        #endregion

        #region StringRequest

        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из одного батча данных.
        /// </summary>
        /// <returns></returns>
        public string CreateStringRequest(IEnumerable<AdInputType> inputTypes, int startRow)
        {
            //throw new NotImplementedException("CreateStringRequest ExceptionTest");//DEBUG
            return "formatString";
        }


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