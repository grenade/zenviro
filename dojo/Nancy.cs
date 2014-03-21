using System;
using log4net;
using Nancy.Hosting.Self;
using Zenviro.Bushido;

namespace Zenviro.Dojo
{
    public class Nancy
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Nancy));

        #region singleton

        static readonly object Lock = new object();
        static Nancy _instance;
        public static Nancy Instance
        {
            get
            {
                lock (Lock)
                    return _instance ?? (_instance = new Nancy());
            }
        }

        #endregion

        private static readonly string Url = string.Format("http://localhost:{0}", AppConfig.NancyPort);
        readonly NancyHost _host = new NancyHost(new Uri(Url));

        public void Start()
        {
            Log.Info(string.Format("nancy web host starting at: {0}...", Url));
            _host.Start();
        }

        public void Stop()
        {
            Log.Info(string.Format("nancy web host at: {0}, stopping...", Url));
            _host.Stop();
        }
    }
}
