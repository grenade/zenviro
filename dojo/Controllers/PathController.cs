using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Zenviro.Bushido;

namespace Zenviro.Dojo.Controllers
{
    [RoutePrefix("api/path")]
    public class PathController : ApiController
    {
        [Route(""), HttpGet]
        public IEnumerable<SearchPathModel> GetPaths()
        {
            return DataAccess.GetPaths();
        }

        [Route("{share}"), HttpGet]
        public SearchPathModel GetPath(string share)
        {
            var find = share.Replace("%5C", @"\");
            var paths = DataAccess.GetPaths().ToArray();
            return paths.FirstOrDefault(x => x.Share.Equals(find, StringComparison.InvariantCultureIgnoreCase));
        }

        [Route(""), HttpPut, HttpPost]
        public IHttpActionResult SavePath(SearchPathModel path)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            DataAccess.SavePath(path);
            return StatusCode(HttpStatusCode.Accepted);
        }

        [Route(""), HttpDelete]
        public IHttpActionResult DeletePath(SearchPathModel path)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            DataAccess.DeletePath(path);
            return StatusCode(HttpStatusCode.Accepted);
        }
    }

    [RoutePrefix("")]
    public class DefaultController : ApiController
    {
        [Route(""), HttpGet]
        public RedirectResult Index()
        {
            return Redirect(Request.RequestUri.AbsoluteUri + "index.html");
        }
    }
}