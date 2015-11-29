using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace KubeDeploy.CI
{
    /// <summary>
    /// Travis CI
    /// </summary>
    public class Travis
    {
        /// <summary>
        /// Rest client
        /// </summary>
        public RestClient Client { get; set; }

        /// <summary>
        /// Travis checker
        /// </summary>
        /// <param name="repository">Repository, example yasp-dota/yasp</param>
        public Travis(string repository)
        {
            Client = new RestClient("https://api.travis-ci.org/" + repository + "/");
        }

        /// <summary>
        /// Get a list of builds
        /// </summary>
        /// <returns></returns>
        public TravisBuild[] ListBuilds()
        {
            RestRequest req = new RestRequest("builds", Method.GET);
            var result = Client.Execute(req);
            return JArray.Parse(result.Content).ToObject<TravisBuild[]>();
        }
    }
}
