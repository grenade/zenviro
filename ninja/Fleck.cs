using System.Collections.Generic;
using System.Linq;
using Fleck;
using log4net;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using Zenviro.Bushido;

namespace Zenviro.Ninja
{
    public class Fleck
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Fleck));

        private const int RecentMessageBufferSize = 1000;
        #region singleton

        static readonly object Lock = new object();
        static Fleck _instance;
        public static Fleck Instance
        {
            get
            {
                lock (Lock)
                    return _instance ?? (_instance = new Fleck());
            }
        }

        #endregion

        static readonly string Uri = string.Format("ws://localhost:{0}", AppConfig.FleckPort);

        private Fleck()
        {
            _sockets = new List<IWebSocketConnection>();
            _server = new WebSocketServer(Uri);
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    _sockets.Add(socket);
                    var recentMessages = _queue.ToArray();
                    foreach (var message in recentMessages)
                        socket.Send(message);
                };
                socket.OnClose = () => _sockets.Remove(socket);
                socket.OnMessage = Broadcast;
            });
        }

        public void Init()
        {
            Log.Info(string.Format("fleck server initialised at: {0}", Uri));
        }

        public void Run()
        {
            Log.Info(string.Format("fleck server running at: {0}", Uri));
        }

        private readonly Queue<string> _queue = new Queue<string>(RecentMessageBufferSize);

        public void Broadcast(string message)
        {
            var tidiedMessage = message.Replace("+<>c__DisplayClass6", string.Empty);
            _queue.Enqueue(tidiedMessage);
            _sockets.ToList().ForEach(s => s.Send(tidiedMessage));
        }

        public void Stop()
        {
            Log.Info(string.Format("fleck server at: {0}, stopping", Uri));
            _server.Dispose();
        }

        private readonly List<IWebSocketConnection> _sockets;
        private readonly WebSocketServer _server;
    }

    public class FleckAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            Fleck.Instance.Broadcast(JsonConvert.SerializeObject(new
            {
                severity = GetSeverity(loggingEvent.Level),
                source = loggingEvent.LocationInformation.ClassName,
                message = loggingEvent.RenderedMessage,
                timestamp = loggingEvent.TimeStamp
            }));
        }

        private static string GetSeverity(Level level)
        {
            switch (level.Name)
            {
                case "DEBUG":
                    return "Debug";
                case "WARN":
                    return "Warning";
                case "ERROR":
                    return "Error";
                default:
                    return "Information";
            }
        }
    }
}
