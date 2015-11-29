using System;

namespace KubeDeploy
{
    public static class Env
    {
        /// <summary>
        /// Kubernetes namespace.
        /// </summary>
        public static string KUBERNETES_NAMESPACE;

        /// <summary>
        /// Webhook secret
        /// </summary>
        public static string WEBHOOK_SECRET;

        /// <summary>
        /// Comma separated list of replication controllers to update
        /// </summary>
        public static string REPLICATION_CONTROLLERS;

        /// <summary>
        /// The docker image, e.g. yasp/yasp
        /// </summary>
        public static string DOCKER_IMAGE;

        /// <summary>
        /// The travis repository, e/g yasp-dota/yasp
        /// </summary>
        public static string TRAVIS_REPOSITORY;

        /// <summary>
        /// The deploy branch
        /// </summary>
        public static string DEPLOY_BRANCH;

        static Env()
        {
            // Exit with missing environment variable
            KUBERNETES_NAMESPACE = Environment.GetEnvironmentVariable("BRIDGE_KUBERNETES_NAMESPACE");
            if (KUBERNETES_NAMESPACE == null)
                Environment.Exit(255);

            WEBHOOK_SECRET = Environment.GetEnvironmentVariable("WEBHOOK_SECRET");
            if (WEBHOOK_SECRET == null)
                Environment.Exit(255);

            REPLICATION_CONTROLLERS = Environment.GetEnvironmentVariable("REPLICATION_CONTROLLERS");
            if (REPLICATION_CONTROLLERS == null)
                Environment.Exit(255);

            DOCKER_IMAGE = Environment.GetEnvironmentVariable("DOCKER_IMAGE");
            if (DOCKER_IMAGE == null)
                Environment.Exit(255);

            TRAVIS_REPOSITORY = Environment.GetEnvironmentVariable("TRAVIS_REPOSITORY");
            if (TRAVIS_REPOSITORY == null)
                Environment.Exit(255);

            DEPLOY_BRANCH = Environment.GetEnvironmentVariable("DEPLOY_BRANCH");
            if (DEPLOY_BRANCH == null)
                Environment.Exit(255);
        }
    }
}
