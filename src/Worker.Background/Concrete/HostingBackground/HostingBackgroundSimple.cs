using System;
using System.Threading;
using System.Threading.Tasks;
using Worker.Background.Abstarct;

namespace Worker.Background.Concrete.HostingBackground
{
    public class HostingBackgroundSimple : HostingBackgroundBase, ISimpleBackground
    {
        #region prop

        public string Key { get; }
        public Func<CancellationToken, Task> ActionBg { get; private set; }
     
        #endregion




        #region ctor

        public HostingBackgroundSimple(string key, bool autoStart) : base(autoStart)
        {
            Key = key;
        }

        #endregion




        #region Methode

        public override void AddOneTimeAction(Func<CancellationToken, Task> action)
        {
            if (action != null)
                ActionBg = action;
        }


        protected override async Task ProcessAsync(CancellationToken stoppingToken)
        {
            await ActionBg(stoppingToken);
        }

        #endregion
    }
}