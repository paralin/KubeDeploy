using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KubeDeploy.Kube;
using KubeNET.Swagger.Model;
using log4net;
using Newtonsoft.Json.Linq;

namespace KubeDeploy.Deployment
{
    /// <summary>
    /// Replication controller deployer
    /// </summary>
    public class RCDeployer
    {
        private ILog log;

        /// <summary>
        /// Replication controller deployment name
        /// </summary>
        private string _rcName;

        /// <summary>
        /// Previous versions, sorted newest to oldest.
        /// </summary>
        private DeploymentReplicationController[] _previousVersions;

        /// <summary>
        /// Last version number
        /// </summary>
        private uint _lastVersion;

        /// <summary>
        /// Create a Replication Controller Deployer
        /// </summary>
        /// <param name="replicationControllerName">RC name</param>
        public RCDeployer(string replicationControllerName)
        {
            log = LogManager.GetLogger("RcDeployer " + replicationControllerName);
            _rcName = replicationControllerName;
        }

        /// <summary>
        /// Attempt to perform a deployment to a new tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task Deploy(string tag)
        {
            log.Debug("Performing deployment to tag " + tag + "...");
            try
            {
                await UpdateLastRcs();
                if (_previousVersions.Length == 0)
                {
                    log.Warn("There are no previous replication controllers for this deployment.");
                    return;
                }
                uint newVersion = _previousVersions[0].Version + 1u;
                log.Debug("The new version ID will be " + newVersion + ".");
                bool doRollingUpdate = true;
                int replicas = 0;
                if (!_previousVersions[0].Controller.Spec.Replicas.HasValue || _previousVersions[0].Controller.Spec.Replicas == 0)
                {
                    log.Debug("The last version has no replicas, creating rc without rolling update.");
                    doRollingUpdate = false;
                    replicas = _previousVersions[0].Controller.Spec.Replicas.Value;
                }
                // Pretty stupid way to deep clone it
                var newRc = JObject.Parse(JObject.FromObject(_previousVersions[0]).ToString()).ToObject<V1ReplicationController>();
                newRc.Metadata.Name = _rcName + "-" + newVersion;
                newRc.Metadata.CreationTimestamp = null;
                newRc.Metadata.Namespace = Kubernetes.Namespace;
                newRc.Metadata.ResourceVersion = null;
                newRc.Metadata.SelfLink = null;
                // Start at zero
                newRc.Spec.Replicas = 0;
                newRc.Spec.Selector["version"] = tag;
                newRc.Spec.Template.Metadata.Labels["version"] = tag;
                newRc.Spec.Template.Spec.Containers[0].Image = Env.DOCKER_IMAGE + ":" + tag;
                log.Debug("Creating new replication controller " + newRc.Metadata.Name + "...");
                await Kubernetes.V1Api.CreateNamespacedReplicationControllerAsync(newRc, Kubernetes.Namespace, null);
                if (doRollingUpdate)
                {
                    JObject patch = new JObject
                    {
                        ["spec"] = new JObject()
                        {
                            {"replicas", 0}
                        }
                    };
                    log.Debug("Scaling " + _previousVersions[0].Controller.Metadata.Name +" to 0 replicas...");
                    await
                        Kubernetes.V1Api.PatchNamespacedReplicationControllerAsync((UnversionedPatch) patch, Kubernetes.Namespace,
                            _previousVersions[0].Controller.Metadata.Name, null);
                    patch["spec"]["replicas"] = replicas;
                    log.Debug("Scaling " + newRc.Metadata.Name +" to " + replicas +" replicas...");
                    await
                        Kubernetes.V1Api.PatchNamespacedReplicationControllerAsync((UnversionedPatch) patch, Kubernetes.Namespace,
                            newRc.Metadata.Name, null);
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to perform deployment", ex);
            }
        }

        /// <summary>
        /// Try to update the latest deployment controller(s).
        /// </summary>
        private async Task UpdateLastRcs()
        {
            var controllers =
                await Kubernetes.V1Api.ListNamespacedReplicationControllerAsync(Kubernetes.Namespace, null, "deployment=" + _rcName, null, null,
                    null, null);
            _previousVersions = controllers.Items.Select(DeploymentReplicationController.ValidateAndCreate).Where(rc => rc != null).OrderByDescending(m => m.Version).ToArray();
        }
    }
}
