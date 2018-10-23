using System;
using System.Collections.Generic;
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
            var startRow = Option.StartPosition;          //TODO: {row} можно вычислить
            var requestBody = Option.RequestOption.Body;
            //throw new NotImplementedException("CreateStringRequest ExceptionTest");//DEBUG
            return $"formatString startRow= {startRow}"; 
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