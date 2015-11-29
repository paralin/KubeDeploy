using KubeNET.Swagger.Model;
using log4net;

namespace KubeDeploy.Deployment
{
    /// <summary>
    /// A replication controller with a parsed version ID
    /// </summary>
    public class DeploymentReplicationController
    {
        private static ILog _validateLog = LogManager.GetLogger("DeploymentRCValidator");
        /// <summary>
        /// The existing replication controller
        /// </summary>
        public V1ReplicationController Controller { get; private set; }

        /// <summary>
        /// The version of this rc.
        /// </summary>
        public uint Version { get; private set; }

        public DeploymentReplicationController(V1ReplicationController controller, uint versionNum)
        {
            Controller = controller;
            Version = versionNum;
        }

        public static DeploymentReplicationController ValidateAndCreate(V1ReplicationController cont)
        {
            _validateLog.Debug("Validating rc " + cont.Metadata.Name + "...");
            // The replication controller name should be _rcName-version
            var parts = cont.Metadata.Name.Split('-');
            uint version;
            if (!uint.TryParse(parts[parts.Length - 1], out version))
            {
                _validateLog.Warn("Replication controller " + cont.Metadata.Name + " name not formatted correctly.");
                return null;
            }
            return new DeploymentReplicationController(cont, version);
        }
    }
}
