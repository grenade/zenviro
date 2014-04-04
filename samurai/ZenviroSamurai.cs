using System.ServiceProcess;
using System.Threading.Tasks;
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
            Task.Factory.StartNew(() => Fleck.Instance.Run());
            //Task.Factory.StartNew(() => Dojo.Owin.Instance.Start());
            Task.Factory.StartNew(AppConfig.InitDataDir);
            Monitor.Instance.Init();
            Task.Factory.StartNew(() => Monitor.Instance.Run());
        }

        protected override void OnStop()
        {
            //Dojo.Owin.Instance.Stop();
            Monitor.Instance.Stop();
            Fleck.Instance.Stop();
        }
    }
}
