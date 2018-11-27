using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using AutoMapper;
using BL.Services.InputData;
using InputDataModel.Autodictor.Model;
using InputDataModel.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using WebServer.DTO.XML;
using Worker.Background.Abstarct;

namespace WebServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class InputDataController : Controller
    {
        #region fields

        private readonly ISimpleBackground _background;
        private readonly InputDataApplyService<AdInputType> _inputDataApplyService;
        private readonly ILogger _logger;

        #endregion




        #region ctor

        public InputDataController(IConfiguration config,
                                   IIndex<string, ISimpleBackground> background,
                                   InputDataApplyService<AdInputType> inputDataApplyService,
                                   ILogger logger)
                                  
        {
            _inputDataApplyService = inputDataApplyService;
            _logger = logger;
            var backgroundName= config["MessageBrokerConsumer4InData:Name"];
           _background = background[backgroundName];
        }

        #endregion




        #region Methode

        // GET api/InputData/GetBackgroundState
        [HttpGet("GetBackgroundState")]
        public async Task<IActionResult> GetBackgroundState()
        {          
            var bgState = _background.IsStarted ? "Started" : "Stoped";
            await Task.CompletedTask;
            return new JsonResult(bgState);
        }

        //TODO: добавить POST - "SendData4Devices", PUT - "StartBg", PUT - "StopBg".  


        /// <summary>
        /// Запустить бекграунд слушателя выходных сообшений от messageBroker
        /// </summary>
        // GET api/InputData/StartListener
        [HttpPut("StartListener")]
        public async Task<IActionResult> StartListener()
        {
            try
            {
                if (_background.IsStarted)
                {
                    ModelState.AddModelError("StartListener", "Listener already started !!!");
                    return BadRequest(ModelState);
                }

                await _background.StartAsync(CancellationToken.None);
                return Ok("Listener starting");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в InputDataController/StartListener");
                throw;
            }
        }


        /// <summary>
        /// Запустить бекграунд слушателя выходных сообшений от messageBroker
        /// </summary>
        [HttpPut("StopListener")]
        public async Task<IActionResult> StopListener()
        {
            try
            {
                if (!_background.IsStarted)
                {
                    ModelState.AddModelError("StartListener", "Listener already staopped !!!");
                    return BadRequest(ModelState);
                }

                await _background.StopAsync(CancellationToken.None);
                return Ok("Listener Stoping");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в InputDataController/StopListener");
                throw;
            }
        }



        [HttpPost("SendData4Devices")]
        public async Task<IActionResult> SendData4Devices([FromBody] IEnumerable<InputData<AdInputType>> inputDatas)
        {
            try
            {
                var errors= await _inputDataApplyService.ApplyInputData(inputDatas);
                if (errors.Any())
                {
                    var errorCompose = new StringBuilder("Ошибка при отправке даных: ");
                    foreach (var err in errors)
                    {
                        errorCompose.AppendLine(err);
                    }
                    _logger.Error($"{errorCompose}");
                    ModelState.AddModelError("SendData4Devices", errorCompose.ToString());
                    return BadRequest(ModelState);
                }
                    
               return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка в InputDataController/SendData4Devices");
                throw;
            }
        }


        [HttpPost("SendDataXml4Devices")]
        [Produces("application/xml")]
        public async Task<IActionResult> SendDataXml4Devices([FromBody] AdInputType4XmlDtoContainer adInputType4XmlList)
        {
            //try
            //{
            //    var first = adInputType4XmlList.Trains.FirstOrDefault();
            //    var res = _mapper.Map<List<AdInputType>>(trainsList.Trains);
            //}
            //catch (Exception e)
            //{

            //}

            return Ok();


            //try
            //{
            //    var errors= await _inputDataApplyService.ApplyInputData(inputDatas);
            //    if (errors.Any())
            //    {
            //        var errorCompose = new StringBuilder("Error in sending data: ");
            //        foreach (var err in errors)
            //        {
            //            errorCompose.AppendLine(err);
            //        }     
            //        ModelState.AddModelError("SendData4Devices", errorCompose.ToString());
            //        return BadRequest(ModelState);
            //    }

            //    return Ok();
            //}
            //catch (Exception ex)
            //{
            //_logger.Error(ex, "Ошибка в InputDataController/SendDataXml4Devices");
            //    throw;
            //}
        }

        #endregion
    }
}