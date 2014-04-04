using System;
using System.ServiceProcess;
using log4net;
using Zenviro.Bushido;
using Zenviro.Ninja;

namespace Zenviro.Samurai
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine("zenviro samurai is running...");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("press any key to stop.");
                Console.ResetColor();
                Fleck.Instance.Init();
                Fleck.Instance.Run();
                AppConfig.InitDataDir();
                Monitor.Instance.Init();
                Monitor.Instance.Run();
                Monitor.Instance.Stop();
                Fleck.Instance.Stop();
            }
            else
            {
                try
                {
                    ServiceBase.Run(new ZenviroSamurai());
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }
        }
    }
}
