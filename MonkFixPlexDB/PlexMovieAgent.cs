namespace MonkFixPlexDB
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PlexMovieAgent
    {
        [JsonProperty("MediaContainer", NullValueHandling = NullValueHandling.Ignore)]
        public PlexMovieAgentMediaContainer MediaContainer { get; set; }
    }

    public partial class PlexMovieAgentMediaContainer
    {
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        [JsonProperty("identifier", NullValueHandling = NullValueHandling.Ignore)]
        public string Identifier { get; set; }

        [JsonProperty("mediaTagPrefix", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaTagPrefix { get; set; }

        [JsonProperty("mediaTagVersion", NullValueHandling = NullValueHandling.Ignore)]
        public long? MediaTagVersion { get; set; }

        [JsonProperty("SearchResult", NullValueHandling = NullValueHandling.Ignore)]
        public List<PlexMovieAgentSearchResult> SearchResult { get; set; }
    }

    public partial class PlexMovieAgentSearchResult
    {
        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Thumb { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public string Guid { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
        public long? Year { get; set; }

        [JsonProperty("summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty("lifespanEnded", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LifespanEnded { get; set; }
    }
}
