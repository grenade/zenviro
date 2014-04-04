using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Zenviro.Bushido;

namespace Zenviro.Dojo.Controllers
{
    [RoutePrefix("api/app")]
    public class AppController : ApiController
    {
        [Route("")]
        public IEnumerable<AppModel> GetApps()
        {
            return DataAccess.GetApps();
        }

        [Route("{search}")]
        public IEnumerable<AppModel> GetApps(string search)
        {
            return DataAccess.GetApps(search);
        }

        [Route("web")]
        public IEnumerable<AppModel> GetWebApps()
        {
            return DataAccess.GetApps()
                .Where(x => x.Role == "web")
                .OrderBy(x => x.Name);
        }

        [Route("svc")]
        public IEnumerable<AppModel> GetSvcApps()
        {
            return DataAccess.GetApps()
                .Where(x => x.Role == "svc")
                .OrderBy(x => x.Name);
        }
    }
}
