using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Zenviro.Bushido;

namespace Zenviro.Dojo.Controllers
{
    [RoutePrefix("api/env-code")]
    public class EnvCodeController : ApiController
    {
        [Route("")]
        public IEnumerable<string> GetEnvs()
        {
            return DataAccess.GetEnvs().Select(x => x.Code);
        }
    }
}