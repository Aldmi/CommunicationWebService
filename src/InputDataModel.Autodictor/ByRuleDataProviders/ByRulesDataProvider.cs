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
using InputDataModel.Autodictor.ByRuleDataProviders.Rules;
using InputDataModel.Autodictor.Model;
using Shared.Extensions;

namespace InputDataModel.Autodictor.ByRuleDataProviders
{
    public class ByRulesDataProvider : IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>
    {
        #region field

        private readonly List<Rule> _rules;    // Набор правил, для обработки данных.
        private Rule _currentRule;             //Текущее правило (по нему создается _stringRequest)
        private string _stringRequest;        //Строковое представление запроса, которое преобразуется в нужную форму для транспорта.

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

        public int TimeRespone => _currentRule.Option.ViewRules[0].ResponseOption.TimeRespone;        //Время на ответ
        public int CountGetDataByte { get; }                                            //TODO: брать с _currentRule.Option
        public int CountSetDataByte { get; } = 12;                                          //TODO: брать с _currentRule.Option

        #endregion




        #region RxEvent

        public Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>> RaiseSendDataRx { get; } = new Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>>();

        #endregion




        #region IExchangeDataProviderImplementation

        public byte[] GetDataByte()
        {
            StatusString.AppendLine($"GetDataByte. StringRequest= {_stringRequest}");

            var format = _currentRule.Option.ViewRules[0].RequestOption.Format;
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
            StatusString.AppendLine($"SetDataByte. Length= {data.Length}");

            var format = _currentRule.Option.ViewRules[0].ResponseOption.Format;
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
                Encoding = _currentRule.Option.ViewRules[0].ResponseOption.Format,   
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
            return _stringRequest;
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
                if (rule.CheckCommand(inData.Command))
                {
                    _currentRule = rule;
                    StatusString.AppendLine($"Command= {inData.Command}");
                    _stringRequest = _currentRule.CreateStringRequest(inData.Command);
                    InputData = new InDataWrapper<AdInputType> { Command = inData.Command };             
                    RaiseSendDataRx.OnNext(this);
                    continue;
                }
                //ДАННЫЕ--------------------------------------------------------------
                var filterItems = rule.FilteredAndOrderedAndTakesItems(inData.Datas).ToList();  //TODO: OrderBy, maxItem - обрезали/Paging - нарезали (с заполнением пустых строк)
                if (filterItems.Count == 0) continue;

                _currentRule = rule;
                foreach (var viewRule in rule.ViewRules)
                {
                    
                }

                //_currentRule = rule;
                //var numberOfBatch = 0;
                //foreach (var batch in filterItems.Batch(rule.BatchSize))
                //{
                //    InputData = new InDataWrapper<AdInputType> { Datas = batch.ToList() };
                //    StatusString.AppendLine($"NumberOfBatch= {numberOfBatch}  CountItem = {InputData.Datas.Count}");
                //    _stringRequest = _currentRule.CreateStringRequest(batch, numberOfBatch);
                //    RaiseSendDataRx.OnNext(this);
                //    numberOfBatch++;
                //}
            }
            //Конвеер обработки входных данных завершен    
            StatusString.Clear();
            await Task.CompletedTask; 
        }

        #endregion
    }
}