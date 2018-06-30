using System.Collections.Generic;
using Autofac;
using Communication.SerialPort.Abstract;
using Exchange.Base;
using Microsoft.AspNetCore.Mvc;
using Worker.Background.Abstarct;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IEnumerable<IExhangeBehavior> _excBehaviors;
        private readonly ILifetimeScope _scope;
        private readonly ISerailPort _spService;



        //public ValuesController(ISerailPort spService)
        //{
        //    _spService = spService;
        //}

        public ValuesController(IEnumerable<ISerailPort> spServices, IEnumerable<IBackgroundService> backgroundServices)
        {

        }

        //public ValuesController(IEnumerable<IExhangeBehavior> excBehaviors)
        //{
        //    _excBehaviors = excBehaviors;



        //}




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
