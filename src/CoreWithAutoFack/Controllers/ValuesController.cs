using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Exchange.Base;
using Microsoft.AspNetCore.Mvc;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IEnumerable<IExchange> _excBehaviors;
        private readonly IEnumerable<IBackgroundService> _backgroundServices;
        private readonly ILifetimeScope _scope;
        private readonly ISerailPort _spService;



        //public ValuesController(ISerailPort spService)
        //{
        //    _spService = spService;
        //}

        //public ValuesController(IEnumerable<ISerailPort> spServices, IEnumerable<IBackgroundService> backgroundServices)
        //{

        //}

        public ValuesController(IEnumerable<IExchange> excBehaviors, IEnumerable<IBackgroundService> backgroundServices)
        {
            _excBehaviors = excBehaviors;
            _backgroundServices = backgroundServices;
        }




        //public ValuesController(ILifetimeScope scope)
        //{
        //    _scope = scope;

        //    var com=_scope.ResolveNamed<ISpService>("COM1");
        //    _scope.Disposer.Dispose();
        //}





        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
           var background= _backgroundServices.FirstOrDefault(back => back.KeyBackground.Key == "COM1");
           if (background == null)
              return "NULL";

            switch (id)
            {
                case 1:
                    background.StopAsync(CancellationToken.None);
                    break;

                case 2:
                    background.StartAsync(CancellationToken.None);
                    break;
            }

           var isStarted= background.IsStarted;

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
