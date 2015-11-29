using Newtonsoft.Json;

namespace KubeDeploy.CI
{
    public class TravisBuild
    {
        /// <summary>
        /// The ID
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Repository ID
        /// </summary>
        [JsonProperty("repository_id")]
        public int RepositoryId { get; set; }

        /// <summary>
        /// Build number
        /// </summary>
        [JsonProperty("number")]
        public string Number { get; set; }

        /// <summary>
        /// String state
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        [JsonProperty("result")]
        public int? Result { get; set; }

        /// <summary>
        /// Started at date
        /// </summary>
        [JsonProperty("started_at")]
        public string StartedAt { get; set; }

        /// <summary>
        /// Finished at
        /// </summary>
        [JsonProperty("finished_at")]
        public string FinishedAt { get; set; }

        /// <summary>
        /// How long did the build take
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Commit tag
        /// </summary>
        [JsonProperty("commit")]
        public string Commit { get; set; }

        /// <summary>
        /// Branch
        /// </summary>
        [JsonProperty("branch")]
        public string Branch { get; set; }

        /// <summary>
        /// Commit message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Event type that triggered the build
        /// </summary>
        [JsonProperty("event_type")]
        public string EventType { get; set; }
    }
}
