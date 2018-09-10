using System;
using System.Threading;
using System.Threading.Tasks;
using Worker.Background.Abstarct;

namespace Worker.Background.Concrete.HostingBackground
{
    public class HostingBackgroundGetInputData : HostingBackgroundBase, IGetInputDataBackground
    {


        #region ctor

        public HostingBackgroundGetInputData(bool autoStart) : base(autoStart)
        {
        }

        #endregion




        protected override Task ProcessAsync(CancellationToken stoppingToken)
        {
            //TODO: Добавить обработку фонового процесса
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