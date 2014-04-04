using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Zenviro.Bushido;

namespace Zenviro.Dojo.Controllers
{
    [RoutePrefix("api/app-names")]
    public class AppNameController : ApiController
    {
        [Route("")]
        public IEnumerable<object> GetAppNames()
        {
            return DataAccess.GetApps()
                .Select(x => new { x.Name, x.Role })
                .Distinct();
        }

        [Route("{search}")]
        public IEnumerable<object> GetAppNames(string search)
        {
            return DataAccess.GetApps(search)
                .Select(x => new { x.Name, x.Role })
                .Distinct();
        }
    }
}