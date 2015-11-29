using System;
using System.Net;
using System.Threading;
using KubeDeploy.Deployment;
using KubeDeploy.Kube;
using log4net;
using log4net.Config;
using Microsoft.Owin.Hosting;
using Owin;

namespace KubeDeploy
{
    public class Program
    {
        private static ILog log = LogManager.GetLogger("Program");
        private static volatile bool running = true;
        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            Nancy.Json.JsonSettings.MaxJsonLength = int.MaxValue;

            log.Info("Initializing...");

            // Disable ssl verification
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            if (!Kubernetes.IsKubernetes)
            {
                log.Warn("KubeDeploy cannot run without Kubernetes.");
                return;
            }

            if (!Kubernetes.CheckKubernetesConnection())
            {
                log.Warn("Kubernetes connection check failed.");
                return;
            }

            DeploymentStore.Initialize();

            var exitEvent = new ManualResetEvent(false);
            log.Debug("Initializing web host...");
            using (WebApp.Start<Startup>("http://+:8080"))
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                };
                exitEvent.WaitOne();
            }
            log.Debug("Exiting...");
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}
