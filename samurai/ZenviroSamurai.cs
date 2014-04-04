using System.ServiceProcess;
using Zenviro.Bushido;
using Zenviro.Ninja;

namespace Zenviro.Samurai
{
    public partial class ZenviroSamurai : ServiceBase
    {
        public ZenviroSamurai()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Fleck.Instance.Init();
            Fleck.Instance.Run();
            //Dojo.Owin.Instance.Start();
            AppConfig.InitDataDir();

            Monitor.Instance.Init();
            Monitor.Instance.Run();
        }

        protected override void OnStop()
        {
            //Dojo.Owin.Instance.Stop();
            Monitor.Instance.Stop();
            Fleck.Instance.Stop();
        }
    }
}
