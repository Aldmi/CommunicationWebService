using System;
using System.Collections.Generic;
using Autofac;
using AutoMapper;
using BL.Services.Mediators;
using BL.Services.Storages;
using Exchange.Base;
using InputDataModel.Autodictor.Entities;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;
using Microsoft.AspNetCore.Mvc;
using Transport.SerialPort.Abstract;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using Worker.Background.Abstarct;

namespace WebServer.Controllers
{
    /// <summary>
    /// Все singleton сервисы передаваемые через DI, в контроллеры, должны быть ПОТОКОБЕЗОПАСНЫЕ.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ValuesController: Controller
    {
        private readonly MediatorForStorages<AdInputType> _mediatorForStorages;
        private readonly TransportStorageService _spSrStorageService;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IExchange<AdInputType>> _excBehaviors;
        private readonly IEnumerable<ITransportBackground> _backgroundServices;
        private readonly ILifetimeScope _scope;
        private readonly ISerailPort _spService;



        //public ValuesController(TransportStorageService spSrStorageService, IMapper mapper)
        //{
        //    _spSrStorageService = spSrStorageService;
        //    _mapper = mapper;
        //}


        //public ValuesController(MediatorForStorages<AdInputType> mediatorForStorages)
        //{
        //    _mediatorForStorages = mediatorForStorages;
        //}

        //public ValuesController(IEnumerable<ISerailPort> spServices, IEnumerable<IBackgroundService> backgroundServices)
        //{

        //}

        //public ValuesController(IEnumerable<IExchange> excBehaviors, IEnumerable<IBackground> backgroundServices)
        //{
        //    _excBehaviors = excBehaviors;
        //    _backgroundServices = backgroundServices;
        //}




        //public ValuesController(ILifetimeScope scope)
        //{
        //    _scope = scope;

        //    var com=_scope.ResolveNamed<ISpService>("COM1");
        //    _scope.Disposer.Dispose();
        //}




        // GET api/values
        [HttpGet]
        
        // public IEnumerable<InputData<AdInputType>> Get()
        public AdInputType Get()
        {
            var adInputType= new AdInputType
            {
                PathNumber = "5",
            };

            return adInputType;


            //var user = new User() {Name = "dsfdsdfsfsdgdfgh"};
            //var userDto=  _mapper.Map<UserDto>(user);

            //var exchangesDto= new List<ExchangeOptionDto>
            //{
            //    new ExchangeOptionDto
            //    {
            //        Id = 1, 
            //        KeyTransport = new KeyTransportDto {Key = "COM1", TransportType = "Sp"},
            //        Key = "Exch_1",
            //        AutoStartCycleFunc = true,
            //        Provider = new ProviderOptionDto
            //        {
            //            Name = "VidorBase",
            //            ManualProviderOptionDto = new ManualProviderOptionDto
            //            {
            //                TimeRespone = 1000,
            //                Address = "1"
            //            }
            //        }
            //    },
            //    new ExchangeOptionDto
            //    {
            //        Id = 1, 
            //        KeyTransport = new KeyTransportDto {Key = "COM1", TransportType = "Sp"},
            //        Key = "Exch_2",
            //        AutoStartCycleFunc = true,
            //        Provider = new ProviderOptionDto
            //        {
            //            Name = "ByRules",
            //            ByRulesProviderOptionDto = new ByRulesProviderOptionDto
            //            {
            //                RulesDto = new List<RuleDto>
            //                {
            //                    new RuleDto
            //                    {
            //                        Name = "Rule_1",
            //                        Format = "utf8",
            //                        Request = new RequestDto
            //                        {
            //                            Body = "sdgsfsgdfgdfdgfg",
            //                            MaxLenght = 500
            //                        },
            //                        Response = new ResponseDto
            //                        {
            //                            Body = "ttttttttttttt",
            //                            TimeRespone = 500,
            //                            MaxLenght = 500
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //};


            //TODO: заменить на РЕАЛЬНый тестовый запрос
            //var inData = new List<InputData<AdInputType>> //коллекция данных для уст-ва.
            //    {
            //        new InputData<AdInputType>
            //        {
            //            DeviceName = "Device_1",
            //            ExchangeName = "Exchange_1",
            //            Data = new List<AdInputType>
            //            {
            //                new AdInputType
            //                {
            //                    Id = 1,
            //                    Event = "ПРИБ",
            //                    NumberOfTrain = "562",
            //                    PathNumber = "2",
            //                    StationArrival = new Station
            //                    {
            //                        CodeEsr = 521,
            //                        CodeExpress = 100,
            //                        NameRu = "Москва",
            //                    },
            //                    DaysFollowing = "ЕЖ",
            //                    TrainType = TrainType.Passenger,
            //                    VagonDirection = VagonDirection.FromTheHead,
            //                    ExpectedTime = DateTime.Now.AddHours(10),
            //                    DelayTime = DateTime.Now.AddHours(8),
            //                    StopTime = TimeSpan.FromHours(10)
            //                },
            //                new AdInputType
            //                {
            //                    Id = 2,
            //                    Event = "СТОЯНКА",
            //                    NumberOfTrain = "685",
            //                    PathNumber = "2",
            //                    StationArrival = new Station
            //                    {
            //                        CodeEsr = 521,
            //                        CodeExpress = 100,
            //                        NameRu = "Рязань",
            //                    },
            //                    StationDeparture = new Station
            //                    {
            //                        CodeEsr = 530,
            //                        CodeExpress = 101,
            //                        NameRu = "Питер",
            //                    },
            //                    TrainType = TrainType.Suburban,
            //                    VagonDirection = VagonDirection.FromTheTail,
            //                    ExpectedTime = DateTime.Now.AddHours(36),
            //                    DelayTime = DateTime.Now.AddHours(8),
            //                    StopTime = TimeSpan.FromHours(10)
            //                }
            //            }
            //        }
            //    };

            //var inData = new List<InputData<AdInputType>>();
            //return inData;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IEnumerable<string> Get(int id)
        {
            return  new List<string>{"str1", "str2"};
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]ExchangeOptionDto value)
        {
            var exch = value;

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
