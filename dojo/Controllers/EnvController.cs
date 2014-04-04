using System.Collections.Generic;
using System.Web.Http;
using Zenviro.Bushido;

namespace Zenviro.Dojo.Controllers
{
    [RoutePrefix("api/env")]
    public class EnvController : ApiController
    {
        [Route("{code}")]
        public ApplicationEnvironmentModel GetEnv(string code)
        {
            return DataAccess.GetEnv(code);
        }

        [Route("")]
        public IEnumerable<ApplicationEnvironmentModel> GetEnvs()
        {
            return DataAccess.GetEnvs();
        }
    }
}