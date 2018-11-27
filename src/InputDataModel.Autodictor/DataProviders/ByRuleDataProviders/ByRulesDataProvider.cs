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
using Shared.Helpers;

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
        public int CountSetDataByte => _currentRequest.ResponseOption.Lenght;                                   

        #endregion




        #region RxEvent

        public Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>> RaiseSendDataRx { get; } = new Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>>();

        #endregion




        #region IExchangeDataProviderImplementation

        public byte[] GetDataByte()
        {
            var stringRequset = _currentRequest.StringRequest;
            var format = _currentRequest.RequestOption.Format;
            StatusString.AppendLine($"GetDataByte.StringRequest= \"{stringRequset}\". Lenght= \"{stringRequset.Length}\"");
            //Преобразовываем КОНЕЧНУЮ строку в массив байт
            var resultBuffer= stringRequset.ConvertString2ByteArray(format);      
            StatusString.AppendLine($"GetDataByte.ByteRequest= \"{ resultBuffer.ArrayByteToString("X2")}\" Lenght= \"{resultBuffer.Length}\"");
            return resultBuffer;
        }

        /// <summary>
        /// Проверить ответ, Присвоить выходные данные.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetDataByte(byte[] data)
        {
            var format = _currentRequest.ResponseOption.Format;
            if (data == null)
            {
                IsOutDataValid = false;
                OutputData = new ResponseDataItem<AdInputType>
                {
                    ResponseData = null,
                    Encoding = format,
                    IsOutDataValid = IsOutDataValid
                };
                return false;
            }       
            var stringResponse = data.ArrayByteToString(format);
            IsOutDataValid = (stringResponse == _currentRequest.ResponseOption.Body);
            OutputData = new ResponseDataItem<AdInputType>
            {   
                ResponseData = stringResponse,
                Encoding = format,   
                IsOutDataValid = IsOutDataValid            
            };
            StatusString.AppendLine($"SetDataByte.StringResponse= \"{stringResponse}\" Length= \"{data.Length}\"");
            return IsOutDataValid;   
        }

        #endregion



        #region Methode

        public async Task<int> StartExchangePipeline(InDataWrapper<AdInputType> inData)
        {
            //DEBUG
            //TODO: тут можно не дополнять,
            if (inData == null)
            {
                inData= new InDataWrapper<AdInputType>
                {
                    Datas = new List<AdInputType>()
                };
            }

            var countTryingSendData = 0; //Счетчик попыток отправит подготовленные данные.
            foreach (var rule in _rules)
            {
                StatusString.Clear();
                StatusString.AppendLine($"RuleName= \"{rule.Option.Name}\"");
                //КОМАНДА-------------------------------------------------------------
                if (IsCommandHandler(inData.Command, rule.Option.Name))
                {
                    StatusString.AppendLine($"Command= \"{inData.Command}\"");
                    var commandViewRule = rule.ViewRules.FirstOrDefault();
                    _currentRequest = commandViewRule?.GetCommandRequestString();
                    InputData = new InDataWrapper<AdInputType> { Command = inData.Command };             
                    RaiseSendDataRx.OnNext(this);
                    countTryingSendData++;
                    continue;
                }
                //ДАННЫЕ--------------------------------------------------------------
                var takesItems = FilteredAndOrderedAndTakesItems(inData.Datas, rule.Option.WhereFilter, rule.Option.OrderBy, rule.Option.TakeItems, rule.Option.DefaultItemJson)?.ToList();
                if (IsDataHandler(inData.Command, takesItems))
                {
                    foreach (var viewRule in rule.ViewRules)
                    {
                        foreach (var request in viewRule.GetDataRequestString(takesItems))
                        {
                            if (request == null) //правило отображения не подходит под ДАННЫЕ
                                continue;

                            _currentRequest = request;
                            InputData = new InDataWrapper<AdInputType> { Datas = _currentRequest.BatchedData.ToList() };
                            StatusString.AppendLine($"viewRule.Id = \"{viewRule.Option.Id}\".  CountItem4Sending = \"{InputData.Datas.Count}\"");
                            RaiseSendDataRx.OnNext(this);
                            countTryingSendData++;
                        }
                    }
                }
            }
            //Конвеер обработки входных данных завершен    
            StatusString.Clear();
            await Task.CompletedTask;
            return countTryingSendData;
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
    }
}