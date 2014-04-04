using System;
using log4net;
using Microsoft.Owin.Hosting;
using Zenviro.Bushido;

namespace Zenviro.Dojo
{
    public class Owin
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Owin));

        #region singleton

        static readonly object Lock = new object();
        static Owin _instance;
        public static Owin Instance
        {
            get
            {
                lock (Lock)
                    return _instance ?? (_instance = new Owin());
            }
        }

        #endregion

        static readonly string Uri = string.Format("http://+:{0}/", AppConfig.NancyPort);
        private IDisposable _server;

        public void Start()
        {
            Log.Info(string.Format("owin server at: {0}, starting", Uri));
            _server = WebApp.Start<Startup>(new StartOptions(Uri)
            {
                ServerFactory = "Microsoft.Owin.Host.HttpListener"
            });
        }

        public void Stop()
        {
            Log.Info(string.Format("owin server at: {0}, stopping", Uri));
            _server.Dispose();
        }
    }
}
