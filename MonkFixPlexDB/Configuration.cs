namespace MonkFixPlexDB
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Configuration
    {
        [JsonProperty("plexDatabasePath", NullValueHandling = NullValueHandling.Ignore)]
        public string PlexDatabasePath { get; set; }

        [JsonProperty("plexAutoScanURL", NullValueHandling = NullValueHandling.Ignore)]
        public Uri PlexAutoScanUrl { get; set; }

        [JsonProperty("mediaPaths", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediaPath> MediaPaths { get; set; }

        [JsonProperty("plexUser", NullValueHandling = NullValueHandling.Ignore)]
        public string PlexUser { get; set; }

        [JsonProperty("plexPass", NullValueHandling = NullValueHandling.Ignore)]
        public string PlexPass { get; set; }

        [JsonProperty("plexToken", NullValueHandling = NullValueHandling.Ignore)]
        public string PlexToken { get; set; }

        [JsonProperty("plexProtocol", NullValueHandling = NullValueHandling.Ignore)]
        public string PlexProtocol { get; set; }

        [JsonProperty("plexPort", NullValueHandling = NullValueHandling.Ignore)]
        public long? PlexPort { get; set; }

        [JsonProperty("plexHost", NullValueHandling = NullValueHandling.Ignore)]
        public string PlexHost { get; set; }

        [JsonProperty("timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int Timeout { get; set; }

        [JsonProperty("emptyTrash", NullValueHandling = NullValueHandling.Ignore)]
        public bool EmptyTrash { get; set; }

        [JsonProperty("preferredAgent", NullValueHandling = NullValueHandling.Ignore)]
        public string PreferredAgent { get; set; }

        [JsonProperty("sectionsToMonitor", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> SectionsToMonitor { get; set; }

        [JsonProperty("sectionsToProcess", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> SectionsToProcess { get; set; }

        [JsonProperty("mediaTypes", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> MediaTypes { get; set; }
    }

    public partial class MediaPath
    {
        [JsonProperty("section", NullValueHandling = NullValueHandling.Ignore)]
        public long? Section { get; set; }

        [JsonProperty("realPath", NullValueHandling = NullValueHandling.Ignore)]
        public string RealPath { get; set; }

        [JsonProperty("dockerPath", NullValueHandling = NullValueHandling.Ignore)]
        public string DockerPath { get; set; }
    }
}
