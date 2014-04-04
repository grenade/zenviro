using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using Zenviro.Bushido;

namespace Zenviro.Dojo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(typeof(OwinRequestLogger));
            app.UseStaticFiles(new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(Path.Combine(AppConfig.DataDir, "web")),
                RequestPath = new PathString()
            });
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute("Path", "api/path/{share}");
            //config.Routes.MapHttpRoute(
            //    name: "SearchPaths",
            //    routeTemplate: "api/path/{share}",
            //    defaults: new { share = RouteParameter.Optional }
            //); 
            app.UseWebApi(config);
        }
    }

    class OwinRequestLogger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OwinRequestLogger));
        private readonly Func<IDictionary<string, object>, Task> _next;

        public OwinRequestLogger(Func<IDictionary<string, object>, Task> next)
        {
            if (next == null)
                throw new ArgumentNullException("next");
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var method = GetValueFromEnvironment(environment, "owin.RequestMethod");
            var path = GetValueFromEnvironment(environment, "owin.RequestPath");

            Log.Debug(string.Format("Entry\t{0}\t{1}", method, path));
            var stopWatch = Stopwatch.StartNew();
            return _next(environment).ContinueWith(t =>
            {
                Log.Debug(string.Format("Exit\t{0}\t{1}\t{2}\t{3}\t{4}",
                    method,
                    path,
                    stopWatch.ElapsedMilliseconds,
                    GetValueFromEnvironment(environment, "owin.ResponseStatusCode"),
                    GetValueFromEnvironment(environment, "owin.ResponseReasonPhrase")));
                return t;
            });
        }

        private static string GetValueFromEnvironment(IDictionary<string, object> environment, string key)
        {
            object value;
            environment.TryGetValue(key, out value);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
