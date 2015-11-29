using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KubeDeploy.CI;
using log4net;

namespace KubeDeploy.Deployment
{
    /// <summary>
    /// Stores all the deployments
    /// </summary>
    public static class DeploymentStore
    {
        private static ILog log = LogManager.GetLogger("DeploymentStore");
        public static List<RCDeployer> Deployments;
        public static Travis Travis;

        static DeploymentStore()
        {
            Deployments = new List<RCDeployer>();
            Travis = new Travis(Env.TRAVIS_REPOSITORY);
        }

        public static void Initialize()
        { 
            string[] rcs = Env.REPLICATION_CONTROLLERS.Trim().Split(',');
            foreach (var rct in rcs)
            {
                var rc = rct.Trim();
                if (rc.Length == 0)
                    continue;
                Deployments.Add(new RCDeployer(rc));
            }
        }

        /// <summary>
        /// Start a deployment based on a Travis build ID.
        /// </summary>
        /// <param name="buildid"></param>
        public static async Task StartTravisDeployment(int buildid)
        {
            try
            {
                log.Debug("Checking Travis for build ID " + buildid);
                var builds = Travis.ListBuilds();
                var build = builds.First(m => m.Id == buildid);
                if (build.Branch.ToLower() != Env.DEPLOY_BRANCH.ToLower())
                {
                    log.Warn("Build branch " + build.Branch + " isn't deploy branch " + Env.DEPLOY_BRANCH + ", ignoring.");
                    return;
                }
                if (!build.Result.HasValue || build.Result.Value == 0)
                {
                    log.Warn("Build result doesn't indicate success, ignoring.");
                    return;
                }
                log.Debug("Deploying commit " + build.Commit + "...");
                foreach (var deployment in Deployments)
                {
                    await deployment.Deploy(build.Commit);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error starting travis deployment", ex);
            }
        }
    }
}
