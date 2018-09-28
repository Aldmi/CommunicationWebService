using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using InputDataModel.Autodictor.Model;
using Shared.Extensions;
using Shared.Types;
using Transport.Base.DataProvidert;

namespace InputDataModel.Autodictor.ByRuleDataProviders
{
    public class ByRulesDataProvider : IExchangeDataProvider<AdInputType, TransportResponse>
    {
        #region field

        private CancellationTokenSource _cts;

        private readonly ByRulesProviderOption _providerOption;
        private readonly List<Rule> _rules;

        private Rule _currentRule;
        private string _stringRequest;

        #endregion




        #region ctor

        public ByRulesDataProvider(ProviderOption providerOption)
        {
            _providerOption = providerOption.ByRulesProviderOption;
            if(_providerOption == null)
                throw new ArgumentNullException(providerOption.Name);

           _rules= _providerOption.Rules.Select(option=> new Rule(option)).ToList();
           _cts = new CancellationTokenSource();
        }

        #endregion




        #region prop

        public string ProviderName { get; set; }
        public InDataWrapper<AdInputType> InputData { get; set; }

        public TransportResponse OutputData { get; set; }
        public int CountGetDataByte { get; }
        public int CountSetDataByte { get; }



        public bool IsOutDataValid { get; set; }
        public Subject<TransportResponse> OutputDataChangeRx { get; }



        public int TimeRespone => _currentRule.Option.ResponseOption.TimeRespone;


        #endregion




        #region IExchangeDataProviderImplementation

        public byte[] GetDataByte()
        {
            var format = _currentRule.Option.Format;
            //Преобразовываем КОНЕЧНУЮ строку в массив байт
            byte[]  resultBuffer;
            if (format == "HEX")
            {
                resultBuffer = new byte[100] ; //DEBUG
                //Распарсить строку в масив байт как она есть. 0203АА96 ...
            }
            else
            {
                resultBuffer = Encoding.GetEncoding(format).GetBytes(_stringRequest).ToArray();
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
            //_currentRule.Option.ResponseOption.Body
            if (data?[0] == 0x10)
            {
                IsOutDataValid = true;
            }
            else
            {
                IsOutDataValid = false;
            }

            //TODO: рефакторинг
            OutputData = new TransportResponse
            {
                ResponseData = data.ToString(),
                Encoding = _currentRule.Option.ResponseOption.Body,   
                IsOutDataValid = IsOutDataValid            
            };

            return IsOutDataValid;   
        }

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
            return _stringRequest;
        }

        public bool SetString(Stream stream)
        {
            throw new NotImplementedException();
        }


        public Subject<IExchangeDataProvider<AdInputType, TransportResponse>> RaiseSendDataRx { get; } = new Subject<IExchangeDataProvider<AdInputType, TransportResponse>>();

        #endregion




        #region Methode

        public async Task StartExchangePipline(InDataWrapper<AdInputType> inData)
        {           
            foreach (var rule in _rules)
            {
                var chekedItems = inData.Datas.Where(data => rule.CheckItem(data)).ToList(); //TODO: Проверить на ограничение max item
                if (chekedItems.Count == 0) continue;

                _currentRule = rule;
                foreach (var butch in chekedItems.Batch(rule.BatchSize))
                {
                    _stringRequest = _currentRule.CreateStringRequest(butch); //TODO:передвать startIndex, для этого батча (для очета смещ=щения строки по Y).
                    InputData = new InDataWrapper<AdInputType> {Datas = butch.ToList()};
                    RaiseSendDataRx.OnNext(this);               
                }
            }
         
            //Конвеер обработки входных данных завершен  
            await Task.CompletedTask; 
        }

        #endregion
    }


    public class Rule
    {
        public readonly RuleOption Option;

        public Rule(RuleOption option)
        {
            Option = option;
        }


        public int BatchSize => Option.BatchSize;

        /// <summary>
        /// Проверяет элемент под ограничения правила.
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public bool CheckItem(AdInputType inputType)
        {
            return true;
        }

        /// <summary>
        /// Создать строку Запроса (используя форматную строку) из одного батча данных.
        /// </summary>
        /// <returns></returns>
        public string CreateStringRequest(IEnumerable<AdInputType> inputTypes)
        {
            //throw new NotImplementedException("ddsfdsf");//DEBUG
            return "formatString";
        }
    }
}