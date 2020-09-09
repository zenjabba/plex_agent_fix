using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MonkFixPlexDB
{

    public partial class PlexMediaContainer
    {
        public MediaContainerClass MediaContainer{ get; set; }
    }

    public partial class MediaContainerClass
    {
        public long Size { get; set; }
        public bool AllowSync { get; set; }
        public string AugmentationKey { get; set; }
        public string Identifier { get; set; }
        public long LibrarySectionId { get; set; }
        public string LibrarySectionTitle { get; set; }
        public Guid LibrarySectionUuid { get; set; }
        public string MediaTagPrefix { get; set; }
        public long MediaTagVersion { get; set; }
        public List<MediaContainerMetadatum> Metadata { get; set; }
    }

    public partial class MediaContainerMetadatum
    {
        public long RatingKey { get; set; }
        public string Key { get; set; }
        public string guid { get; set; }
        public string Studio { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string TitleSort { get; set; }
        public string LibrarySectionTitle { get; set; }
        public long LibrarySectionId { get; set; }
        public string LibrarySectionKey { get; set; }
        public string ContentRating { get; set; }
        public string Summary { get; set; }
        public double Rating { get; set; }
        public double AudienceRating { get; set; }
        public long Year { get; set; }
        public string Tagline { get; set; }
        public string Thumb { get; set; }
        public string Art { get; set; }
        public long Duration { get; set; }
        public DateTimeOffset OriginallyAvailableAt { get; set; }
        public long AddedAt { get; set; }
        public long UpdatedAt { get; set; }
        public string AudienceRatingImage { get; set; }
        public string PrimaryExtraKey { get; set; }
        public string RatingImage { get; set; }
        public List<FluffyMedia> Media { get; set; }
        public List<Country> Genre { get; set; }
        public List<Country> Director { get; set; }
        public List<Country> Writer { get; set; }
        public List<Country> Producer { get; set; }
        public List<Country> Country { get; set; }
        public List<Role> Role { get; set; }
        public List<Country> Similar { get; set; }
        [JsonProperty("Guid")]
        public PlexGuid[] Guid { get; set; }
        public Extras Extras { get; set; }
    }

    public class PlexGuid
    {
        [JsonProperty("id")]
        public string id { get; set; }
    }

    public partial class Country
    {
        public long Id { get; set; }
        public string Filter { get; set; }
        public string Tag { get; set; }
    }

    public partial class Extras
    {
        public long Size { get; set; }
        public List<ExtrasMetadatum> Metadata { get; set; }
    }

    public partial class ExtrasMetadatum
    {
        public long RatingKey { get; set; }
        public string Key { get; set; }
        public string Guid { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public long Index { get; set; }
        public long Year { get; set; }
        public string Thumb { get; set; }
        public string Subtype { get; set; }
        public long Duration { get; set; }
        public DateTimeOffset OriginallyAvailableAt { get; set; }
        public long AddedAt { get; set; }
        public long ExtraType { get; set; }
        public List<PurpleMedia> Media { get; set; }
    }

    public partial class PurpleMedia
    {
        public long Id { get; set; }
        public long Duration { get; set; }
        public long Bitrate { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public double AspectRatio { get; set; }
        public string AudioCodec { get; set; }
        public string VideoCodec { get; set; }
        public string VideoResolution { get; set; }
        public string Container { get; set; }
        public bool Premium { get; set; }
        public List<PurplePart> Part { get; set; }
    }

    public partial class PurplePart
    {
        public long Id { get; set; }
        public long Duration { get; set; }
        public string Container { get; set; }
        public string Key { get; set; }
        public bool OptimizedForStreaming { get; set; }
        public List<PurpleStream> Stream { get; set; }
    }

    public partial class PurpleStream
    {
        public long Id { get; set; }
        public long StreamType { get; set; }
        public string Codec { get; set; }
        public long Index { get; set; }
        public long? Bitrate { get; set; }
        public long? Height { get; set; }
        public long? Width { get; set; }
        public string DisplayTitle { get; set; }
        public bool? Selected { get; set; }
        public long? Channels { get; set; }
        public string Language { get; set; }
        public string LanguageCode { get; set; }
    }

    public partial class FluffyMedia
    {
        public long Id { get; set; }
        public long Duration { get; set; }
        public long Bitrate { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public double AspectRatio { get; set; }
        public long AudioChannels { get; set; }
        public string AudioCodec { get; set; }
        public string VideoCodec { get; set; }
        public string VideoResolution { get; set; }
        public string Container { get; set; }
        public string VideoFrameRate { get; set; }
        public string AudioProfile { get; set; }
        public string VideoProfile { get; set; }
        public List<FluffyPart> Part { get; set; }
    }

    public partial class FluffyPart
    {
        public bool Accessible { get; set; }
        public bool Exists { get; set; }
        public long Id { get; set; }
        public string Key { get; set; }
        public long Duration { get; set; }
        public string File { get; set; }
        public long Size { get; set; }
        public string AudioProfile { get; set; }
        public string Container { get; set; }
        public string VideoProfile { get; set; }
        public List<FluffyStream> Stream { get; set; }
    }

    public partial class FluffyStream
    {
        public long Id { get; set; }
        public long StreamType { get; set; }
        public bool? Default { get; set; }
        public string Codec { get; set; }
        public long Index { get; set; }
        public long? Bitrate { get; set; }
        public long? BitDepth { get; set; }
        public string ChromaLocation { get; set; }
        public string ChromaSubsampling { get; set; }
        public double? FrameRate { get; set; }
        public bool? HasScalingMatrix { get; set; }
        public long? Height { get; set; }
        public long? Level { get; set; }
        public string Profile { get; set; }
        public long? RefFrames { get; set; }
        public string ScanType { get; set; }
        public long? Width { get; set; }
        public string DisplayTitle { get; set; }
        public bool? Selected { get; set; }
        public long? Channels { get; set; }
        public string Language { get; set; }
        public string LanguageCode { get; set; }
        public string AudioChannelLayout { get; set; }
        public long? SamplingRate { get; set; }
        public string Title { get; set; }
    }

    public partial class Role
    {
        public long Id { get; set; }
        public string Filter { get; set; }
        public string Tag { get; set; }
        public string RoleRole { get; set; }
        public Uri Thumb { get; set; }
    }
}
