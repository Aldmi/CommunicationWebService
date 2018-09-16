using System.Threading.Tasks;
using Autofac.Features.Indexed;
using BL.Services.InputData;
using InputDataModel.Autodictor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        #endregion




        #region ctor

        public InputDataController(IConfiguration config, IIndex<string, ISimpleBackground> background, InputDataApplyService<AdInputType> inputDataApplyService)
        {
            _inputDataApplyService = inputDataApplyService;
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

        #endregion
    }
}