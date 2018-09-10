using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Types;

namespace Worker.Background.Concrete.HostingBackground
{
    public class HostingBackgroundGetInputData : HostingBackgroundBase
    {


        public HostingBackgroundGetInputData(KeyTransport key, bool autoStart) : base(key, autoStart)
        {
        }




        protected override Task ProcessAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public override void AddCycleAction(Func<CancellationToken, Task> action)
        {
            throw new NotImplementedException();
        }

        public override void RemoveCycleFunc(Func<CancellationToken, Task> action)
        {
            throw new NotImplementedException();
        }

        public override void AddOneTimeAction(Func<CancellationToken, Task> action)
        {
            throw new NotImplementedException();
        }
    }
}