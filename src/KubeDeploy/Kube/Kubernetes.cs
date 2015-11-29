using System;
using KubeNET.Exceptions;
using KubeNET.Kubernetes;
using KubeNET.Swagger.Api;
using KubeNET.Swagger.Client;
using KubeNET.Swagger.Model;
using log4net;

namespace KubeDeploy.Kube
{
    /// <summary>
    /// Sets up the kubernetes scheduler api
    /// </summary>
    public static class Kubernetes
    {
        private static ILog log = LogManager.GetLogger("Kubernetes");
        /// <summary>
        /// Check if we're running under Kubernetes and env variable requirements are met.
        /// </summary>
        /// <returns></returns>
        public static bool IsKubernetes => Host != null && Namespace != null;

        /// <summary>
        /// Kubernetes host
        /// </summary>
        public static string Host => Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");

        /// <summary>
        /// Kubernetes port
        /// </summary>
        public static string Port => Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

        /// <summary>
        /// Namespace to use for bridge
        /// </summary>
        public static string Namespace { get; set; }

        /// <summary>
        /// Api client
        /// </summary>
        private static ApiClient _client = null;

        /// <summary>
        /// API client
        /// </summary>
        public static ApiClient Client => Configuration.DefaultApiClient;

        /// <summary>
        /// API to identify api versions
        /// </summary>
        public static IApiApi APIApi { get; private set; }

        /// <summary>
        /// API to identify api versions
        /// </summary>
        public static IApisApi APIsApi { get; private set; }

        /// <summary>
        /// API to identify api extension versions
        /// </summary>
        public static IExtensionsApi ExtensionsApi { get; private set; }

        /// <summary>
        /// API for all v1 features.
        /// </summary>
        public static IVApi V1Api { get; set; }

        /// <summary>
        /// Version string for v1
        /// </summary>
        public static string V1ApiVersion = "v1";

        /// <summary>
        /// Default delete options for v1.
        /// </summary>
        public static V1DeleteOptions V1DefaultDeleteOptions = new V1DeleteOptions() { ApiVersion = V1ApiVersion };

        /// <summary>
        /// API for all the v1beta1 features.
        /// </summary>
        public static IVbetaApi VBetaApi { get; set; }

        /// <summary>
        /// Version string for v1beta1
        /// </summary>
        public static string VBetaApiVersion = "extensions/v1beta1";

        /// <summary>
        /// Default delete options for v1beta1.
        /// </summary>
        public static V1DeleteOptions V1BetaDefaultDeleteOptions = new V1DeleteOptions() { ApiVersion = V1ApiVersion };

        /// <summary>
        /// API for all the v1beta1 features.
        /// </summary>
        public static IVersionApi VersionApi { get; set; }

        static Kubernetes()
        {
            Namespace = Env.KUBERNETES_NAMESPACE;
            CreateClientIfNotExists();
        }

        /// <summary>
        /// Creates api client if not exists.
        /// </summary>
        /// <returns></returns>
        private static void CreateClientIfNotExists()
        {
            if (_client != null) return;
            if (!IsKubernetes) return;

            ApiClient client = new ApiClient($"https://{Host}:{Port}/");
            Configuration.DefaultApiClient = client;
            try
            {
                KubernetesConfiguration.ConfigureWithPodEnvironment();
            }
            catch (NotKubernetesPodException)
            {
                return;
            }
            _client = client;
            client.AddDefaultHeader("Authorization", "Bearer " + Configuration.Password);

            // Initialize APIs
            APIApi = new ApiApi(client);
            APIsApi = new ApisApi(client);
            ExtensionsApi = new ExtensionsApi(client);
            V1Api = new VApi(client);
            VBetaApi = new VbetaApi(client);
            VersionApi = new VersionApi(client);
        }

        /// <summary>
        /// Check the kubernetes connection
        /// </summary>
        /// <returns></returns>
        public static bool CheckKubernetesConnection()
        {
            log.Debug("Kubernetes configured with url " + Configuration.DefaultApiClient.BasePath + ", token " + Configuration.Password);
            try
            {
                V1Api.ListNamespacedPod(Namespace, null, null, null, null, null, null);
            }
            catch (Exception ex)
            {
                log.Warn("Kubernetes checks failed, ", ex);
                return false;
            }
            log.Debug("Configuration successful.");
            return true;
        }
    }
}
