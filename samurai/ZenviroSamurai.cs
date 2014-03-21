using System.ServiceProcess;

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
        }

        protected override void OnStop()
        {
        }
    }
}
