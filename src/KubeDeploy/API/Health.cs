using System.Threading.Tasks;
using KubeDeploy.Deployment;
using log4net;
using Nancy;

namespace KubeDeploy.API
{
    /// <summary>
    /// Health
    /// </summary>
    public class Health : Nancy.NancyModule
    {
        private ILog log = LogManager.GetLogger("Health");
        public Health()
        {
            Get["/"] = OkResponse;
            Get["/healthz"] = OkResponse;
        }

        private dynamic OkResponse(dynamic o)
        {
            return "ok";
        }
    }
}
