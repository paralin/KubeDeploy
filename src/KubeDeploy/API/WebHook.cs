using System.Threading.Tasks;
using KubeDeploy.Deployment;
using log4net;
using Nancy;

namespace KubeDeploy.API
{
    /// <summary>
    /// Webhook
    /// </summary>
    public class WebHook : Nancy.NancyModule
    {
        private ILog log = LogManager.GetLogger("WebHook");
        public WebHook() : base("/api/webhook")
        {
            Get["/success/{secret}/{buildid:int}"] = WebhookTrigger;
        }

        private dynamic WebhookTrigger(dynamic o)
        {
            string secret = o.secret;
            int buildid = o.buildid;

            log.Debug("Received hook with secret " + secret + " for build ID " + buildid);
            if (secret != Env.WEBHOOK_SECRET)
            {
                log.Warn("Secret " + secret +" is invalid, ignoring.");
                return HttpStatusCode.Unauthorized;
            }

            Task.Run(() =>
            {
                DeploymentStore.StartTravisDeployment(buildid);
            });

            return HttpStatusCode.OK;
        }
    }
}
