using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Zenviro.Bushido;

namespace Zenviro.Dojo.Controllers
{
    [RoutePrefix("api/host-names")]
    public class HostNameController : ApiController
    {
        [Route("")]
        public IEnumerable<string> GetHostNames()
        {
            return DataAccess.GetHosts().Select(x => x.ToString()).Distinct();
        }
    }
}