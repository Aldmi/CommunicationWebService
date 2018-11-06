using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using InputDataModel.Autodictor.DataProviders.ByRuleDataProviders.Rules;
using InputDataModel.Autodictor.Model;
using Shared.Extensions;

namespace InputDataModel.Autodictor.DataProviders.ByRuleDataProviders
{
    public class ByRulesDataProvider : BaseDataProvider, IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>
    {
        #region field

        private readonly List<Rule> _rules;                   // Набор правил, для обработки данных.
        private ViewRuleRequestModelWrapper _currentRequest;  // Созданный запрос, после подготовки данных. 

        #endregion




        #region ctor

        public ByRulesDataProvider(ProviderOption providerOption)
        {
            var option = providerOption.ByRulesProviderOption;
            if(option == null)
               throw new ArgumentNullException(providerOption.Name);

            ProviderName = providerOption.Name;
           _rules = option.Rules.Select(opt=> new Rule(opt)).ToList();
        }

        #endregion




        #region prop

        public string ProviderName { get; }
        public StringBuilder StatusString { get;  } = new StringBuilder();
        public InDataWrapper<AdInputType> InputData { get; set; }
        public ResponseDataItem<AdInputType> OutputData { get; set; }
        public bool IsOutDataValid { get; set; }

        public int TimeRespone => _currentRequest.ResponseOption.TimeRespone;        //Время на ответ
        public int CountGetDataByte { get; }                                         //TODO: брать с _currentRule.Option
        public int CountSetDataByte { get; } = 12;                                   //TODO: брать с _currentRule.Option

        #endregion




        #region RxEvent

        public Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>> RaiseSendDataRx { get; } = new Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>>();

        #endregion




        #region IExchangeDataProviderImplementation

        public byte[] GetDataByte()
        {
            StatusString.AppendLine($"GetDataByte. StringRequest= {_currentRequest.StringRequest}");

            var format = _currentRequest.RequestOption.Format;
            //Преобразовываем КОНЕЧНУЮ строку в массив байт
            byte[]  resultBuffer;
            if (format == "HEX")
            {
                resultBuffer = new byte[100] ; //DEBUG
                //Распарсить строку в масив байт как она есть. 0203АА96 ...
            }
            else
            {
                resultBuffer = Encoding.GetEncoding(format).GetBytes(_currentRequest.StringRequest).ToArray();
            }
            return resultBuffer;
        }


        /// <summary>
        /// Проверить ответ, Присвоить выходные данные.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetDataByte(byte[] data)
        {
            StatusString.AppendLine($"SetDataByte. Length= {data.Length}");

            var format = _currentRequest.ResponseOption.Format;
            //_currentRule.Option.ResponseOption.Body
            if (data?[0] == 0x10)
            {
                IsOutDataValid = true;
            }
            else
            {
                IsOutDataValid = false;
            }

            OutputData = new ResponseDataItem<AdInputType>
            {   
                ResponseData = data.ArrayByteToString(format),
                Encoding = _currentRequest.ResponseOption.Format,   
                IsOutDataValid = IsOutDataValid            
            };

            return IsOutDataValid;   
        }

        #endregion




        #region NotImplemented
        public Stream GetStream()
        {
            throw new NotImplementedException();
        }

        public bool SetStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public string GetString()
        {
            return _currentRequest.StringRequest;
        }

        public bool SetString(Stream stream)
        {
            throw new NotImplementedException();
        }

        #endregion




        #region Methode

        public async Task StartExchangePipeline(InDataWrapper<AdInputType> inData)
        {           
            foreach (var rule in _rules)
            {
                StatusString.Clear();
                StatusString.AppendLine($"RuleName= {rule.Option.Name}");
                //КОМАНДА-------------------------------------------------------------
                if (rule.IsCommand(inData.Command))
                {
                    //_currentRule = rule;
                    StatusString.AppendLine($"Command= {inData.Command}");
                    //_stringRequest = _currentRule.CreateStringRequest(inData.Command);
                    InputData = new InDataWrapper<AdInputType> { Command = inData.Command };             
                    RaiseSendDataRx.OnNext(this);
                    continue;
                }
                //ДАННЫЕ--------------------------------------------------------------
                var filterItems = FilteredAndOrderedAndTakesItems(inData.Datas, rule.Option.WhereFilter, rule.Option.OrderBy, rule.Option.TakeItems)?.ToList();
                if (filterItems == null || filterItems.Count == 0)
                    continue;

                foreach (var viewRule in rule.ViewRules)
                {
                    foreach (var request in viewRule.GetRequestString(filterItems))
                    {
                        if(request == null) //правило отображения не подходит под ДАННЫЕ
                          continue;

                        _currentRequest = request;
                        InputData = new InDataWrapper<AdInputType> { Datas = _currentRequest.BatchedData.ToList() };
                        StatusString.AppendLine($"CountItem4Sending = {InputData.Datas.Count}");
                        RaiseSendDataRx.OnNext(this);
                    }
                }
            }
            //Конвеер обработки входных данных завершен    
            StatusString.Clear();
            await Task.CompletedTask; 
        }

        #endregion
    }
}