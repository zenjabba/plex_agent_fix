

namespace MonkFixPlexDB
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PlexLibrary
    {
        [JsonProperty("MediaContainer", NullValueHandling = NullValueHandling.Ignore)]
        public MediaContainer MediaContainer { get; set; }
    }

    public partial class MediaContainer
    {
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        [JsonProperty("totalSize", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalSize { get; set; }

        [JsonProperty("allowSync", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowSync { get; set; }

        [JsonProperty("art", NullValueHandling = NullValueHandling.Ignore)]
        public string Art { get; set; }

        [JsonProperty("identifier", NullValueHandling = NullValueHandling.Ignore)]
        public string Identifier { get; set; }

        [JsonProperty("librarySectionID", NullValueHandling = NullValueHandling.Ignore)]
        public long? LibrarySectionId { get; set; }

        [JsonProperty("librarySectionTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string LibrarySectionTitle { get; set; }

        [JsonProperty("librarySectionUUID", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? LibrarySectionUuid { get; set; }

        [JsonProperty("mediaTagPrefix", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaTagPrefix { get; set; }

        [JsonProperty("mediaTagVersion", NullValueHandling = NullValueHandling.Ignore)]
        public long? MediaTagVersion { get; set; }

        [JsonProperty("offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? Offset { get; set; }

        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb { get; set; }

        [JsonProperty("title1", NullValueHandling = NullValueHandling.Ignore)]
        public string Title1 { get; set; }

        [JsonProperty("title2", NullValueHandling = NullValueHandling.Ignore)]
        public string Title2 { get; set; }

        [JsonProperty("viewGroup", NullValueHandling = NullValueHandling.Ignore)]
        public ViewGroup? ViewGroup { get; set; }

        [JsonProperty("viewMode", NullValueHandling = NullValueHandling.Ignore)]
        public long? ViewMode { get; set; }

        [JsonProperty("Meta", NullValueHandling = NullValueHandling.Ignore)]
        public Meta Meta { get; set; }

        [JsonProperty("Metadata", NullValueHandling = NullValueHandling.Ignore)]
        public List<Metadatum> Metadata { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("Type", NullValueHandling = NullValueHandling.Ignore)]
        public List<TypeElement> Type { get; set; }

        [JsonProperty("FieldType", NullValueHandling = NullValueHandling.Ignore)]
        public List<FieldType> FieldType { get; set; }
    }

    public partial class FieldType
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("Operator", NullValueHandling = NullValueHandling.Ignore)]
        public List<Operator> Operator { get; set; }
    }

    public partial class Operator
    {
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }

    public partial class TypeElement
    {
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Active { get; set; }

        [JsonProperty("Filter", NullValueHandling = NullValueHandling.Ignore)]
        public List<Filter> Filter { get; set; }

        [JsonProperty("Sort", NullValueHandling = NullValueHandling.Ignore)]
        public List<Sort> Sort { get; set; }

        [JsonProperty("Field", NullValueHandling = NullValueHandling.Ignore)]
        public List<Field> Field { get; set; }
    }

    public partial class Field
    {
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("subType", NullValueHandling = NullValueHandling.Ignore)]
        public string SubType { get; set; }
    }

    public partial class Filter
    {
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
        public string FilterFilter { get; set; }

        [JsonProperty("filterType", NullValueHandling = NullValueHandling.Ignore)]
        public string? FilterType { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; set; }

        [JsonProperty("advanced", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Advanced { get; set; }
    }

    public partial class Sort
    {
        [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Active { get; set; }

        [JsonProperty("activeDirection", NullValueHandling = NullValueHandling.Ignore)]
        public string? ActiveDirection { get; set; }

        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        public string? Default { get; set; }

        [JsonProperty("defaultDirection", NullValueHandling = NullValueHandling.Ignore)]
        public string? DefaultDirection { get; set; }

        [JsonProperty("descKey", NullValueHandling = NullValueHandling.Ignore)]
        public string DescKey { get; set; }

        [JsonProperty("firstCharacterKey", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstCharacterKey { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }

    public partial class Metadatum
    {
        [JsonProperty("ratingKey", NullValueHandling = NullValueHandling.Ignore)]
        public long? RatingKey { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("guid", NullValueHandling = NullValueHandling.Ignore)]
        public string Guid { get; set; }

        [JsonProperty("studio", NullValueHandling = NullValueHandling.Ignore)]
        public string Studio { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public ViewGroup? Type { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("contentRating", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentRating { get; set; }

        [JsonProperty("summary", NullValueHandling = NullValueHandling.Ignore)]
        public string Summary { get; set; }

        [JsonProperty("audienceRating", NullValueHandling = NullValueHandling.Ignore)]
        public double? AudienceRating { get; set; }

        [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
        public long? Year { get; set; }

        [JsonProperty("tagline", NullValueHandling = NullValueHandling.Ignore)]
        public string Tagline { get; set; }

        [JsonProperty("thumb", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb { get; set; }

        [JsonProperty("art", NullValueHandling = NullValueHandling.Ignore)]
        public string Art { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public long? Duration { get; set; }

        [JsonProperty("originallyAvailableAt", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? OriginallyAvailableAt { get; set; }

        [JsonProperty("addedAt", NullValueHandling = NullValueHandling.Ignore)]
        public long? AddedAt { get; set; }

        [JsonProperty("updatedAt", NullValueHandling = NullValueHandling.Ignore)]
        public long? UpdatedAt { get; set; }

        [JsonProperty("audienceRatingImage", NullValueHandling = NullValueHandling.Ignore)]
        public string AudienceRatingImage { get; set; }
        /*

        [JsonProperty("Media", NullValueHandling = NullValueHandling.Ignore)]
        public List<Media> Media { get; set; }

        [JsonProperty("Genre", NullValueHandling = NullValueHandling.Ignore)]
        public List<Collection> Genre { get; set; }

        [JsonProperty("Director", NullValueHandling = NullValueHandling.Ignore)]
        public List<Collection> Director { get; set; }

        [JsonProperty("Writer", NullValueHandling = NullValueHandling.Ignore)]
        public List<Collection> Writer { get; set; }

        [JsonProperty("Country", NullValueHandling = NullValueHandling.Ignore)]
        public List<Collection> Country { get; set; }

        [JsonProperty("Role", NullValueHandling = NullValueHandling.Ignore)]
        public List<Collection> Role { get; set; }

        [JsonProperty("chapterSource", NullValueHandling = NullValueHandling.Ignore)]
        public string? ChapterSource { get; set; }

        [JsonProperty("primaryExtraKey", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryExtraKey { get; set; }

        [JsonProperty("originalTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string OriginalTitle { get; set; }

        [JsonProperty("Collection", NullValueHandling = NullValueHandling.Ignore)]
        public List<Collection> Collection { get; set; }

        [JsonProperty("titleSort", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleSort { get; set; }

        [JsonProperty("viewCount", NullValueHandling = NullValueHandling.Ignore)]
        public long? ViewCount { get; set; }

        [JsonProperty("lastViewedAt", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastViewedAt { get; set; }
        */
    }

    public partial class Collection
    {
        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }
    }

    public partial class Media
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public long? Duration { get; set; }

        [JsonProperty("bitrate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bitrate { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public long? Width { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public long? Height { get; set; }

        [JsonProperty("aspectRatio", NullValueHandling = NullValueHandling.Ignore)]
        public double? AspectRatio { get; set; }

        [JsonProperty("audioChannels", NullValueHandling = NullValueHandling.Ignore)]
        public long? AudioChannels { get; set; }

        [JsonProperty("audioCodec", NullValueHandling = NullValueHandling.Ignore)]
        public string? AudioCodec { get; set; }

        [JsonProperty("videoCodec", NullValueHandling = NullValueHandling.Ignore)]
        public string? VideoCodec { get; set; }

        [JsonProperty("videoResolution", NullValueHandling = NullValueHandling.Ignore)]
        public string? VideoResolution { get; set; }

        [JsonProperty("container", NullValueHandling = NullValueHandling.Ignore)]
        public Container? Container { get; set; }

        [JsonProperty("videoFrameRate", NullValueHandling = NullValueHandling.Ignore)]
        public string? VideoFrameRate { get; set; }

        [JsonProperty("audioProfile", NullValueHandling = NullValueHandling.Ignore)]
        public string? AudioProfile { get; set; }

        [JsonProperty("videoProfile", NullValueHandling = NullValueHandling.Ignore)]
        public string? VideoProfile { get; set; }

        [JsonProperty("Part", NullValueHandling = NullValueHandling.Ignore)]
        public List<Part> Part { get; set; }

        [JsonProperty("optimizedForStreaming", NullValueHandling = NullValueHandling.Ignore)]
        public long? OptimizedForStreaming { get; set; }

        [JsonProperty("has64bitOffsets", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Has64BitOffsets { get; set; }
    }

    public partial class Part
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public long? Duration { get; set; }

        [JsonProperty("file", NullValueHandling = NullValueHandling.Ignore)]
        public string File { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }

        [JsonProperty("audioProfile", NullValueHandling = NullValueHandling.Ignore)]
        public string? AudioProfile { get; set; }

        [JsonProperty("container", NullValueHandling = NullValueHandling.Ignore)]
        public Container? Container { get; set; }

        [JsonProperty("videoProfile", NullValueHandling = NullValueHandling.Ignore)]
        public string? VideoProfile { get; set; }

        [JsonProperty("has64bitOffsets", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Has64BitOffsets { get; set; }

        [JsonProperty("optimizedForStreaming", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OptimizedForStreaming { get; set; }

        [JsonProperty("hasThumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public long? HasThumbnail { get; set; }
    }

    public enum FilterType { Boolean, Integer, String };

    public enum TypeEnum { Filter };

    public enum ActiveDirection { Asc, Desc };

    public enum AudienceRatingImage { ImdbImageRating, ThemoviedbImageRating };

    public enum ChapterSource { Media, Mixed };

    public enum AudioCodec { Aac, Ac3, Dca, DcaMa, Eac3, Flac, Mp3, Pcm, Truehd };

    public enum AudioProfile { Dts, Lc, Ma, PcmS16Le };

    public enum Container { Avi, Mkv, Mp4 };

    public enum VideoProfile { AdvancedSimple, ConstrainedBaseline, High, Main, Simple };

    public enum VideoCodec { H264, Mpeg4 };

    public enum VideoFrameRate { Ntsc, Pal, The24P };

    public enum VideoResolutionEnum { Sd };

    public enum ViewGroup { Movie, Show, Season, Episode };

    public partial struct VideoResolutionUnion
    {
        public VideoResolutionEnum? Enum;
        public long? Integer;

        public static implicit operator VideoResolutionUnion(VideoResolutionEnum Enum) => new VideoResolutionUnion { Enum = Enum };
        public static implicit operator VideoResolutionUnion(long Integer) => new VideoResolutionUnion { Integer = Integer };
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                FilterTypeConverter.Singleton,
                TypeEnumConverter.Singleton,
                ActiveDirectionConverter.Singleton,
                AudioProfileConverter.Singleton,
                ContainerConverter.Singleton,
                VideoProfileConverter.Singleton,
                AudioCodecConverter.Singleton,
                VideoCodecConverter.Singleton,
                VideoFrameRateConverter.Singleton,
                VideoResolutionUnionConverter.Singleton,
                VideoResolutionEnumConverter.Singleton,
                AudienceRatingImageConverter.Singleton,
                ChapterSourceConverter.Singleton,
                ViewGroupConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class FilterTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FilterType) || t == typeof(FilterType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "boolean":
                    return FilterType.Boolean;
                case "integer":
                    return FilterType.Integer;
                case "string":
                    return FilterType.String;
            }
            throw new Exception("Cannot unmarshal type FilterType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (FilterType)untypedValue;
            switch (value)
            {
                case FilterType.Boolean:
                    serializer.Serialize(writer, "boolean");
                    return;
                case FilterType.Integer:
                    serializer.Serialize(writer, "integer");
                    return;
                case FilterType.String:
                    serializer.Serialize(writer, "string");
                    return;
            }
            throw new Exception("Cannot marshal type FilterType");
        }

        public static readonly FilterTypeConverter Singleton = new FilterTypeConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "filter")
            {
                return TypeEnum.Filter;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            if (value == TypeEnum.Filter)
            {
                serializer.Serialize(writer, "filter");
                return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class ActiveDirectionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ActiveDirection) || t == typeof(ActiveDirection?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "asc":
                    return ActiveDirection.Asc;
                case "desc":
                    return ActiveDirection.Desc;
            }
            throw new Exception("Cannot unmarshal type ActiveDirection");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ActiveDirection)untypedValue;
            switch (value)
            {
                case ActiveDirection.Asc:
                    serializer.Serialize(writer, "asc");
                    return;
                case ActiveDirection.Desc:
                    serializer.Serialize(writer, "desc");
                    return;
            }
            throw new Exception("Cannot marshal type ActiveDirection");
        }

        public static readonly ActiveDirectionConverter Singleton = new ActiveDirectionConverter();
    }

    internal class AudioProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AudioProfile) || t == typeof(AudioProfile?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "dts":
                    return AudioProfile.Dts;
                case "lc":
                    return AudioProfile.Lc;
                case "ma":
                    return AudioProfile.Ma;
                case "pcm_s16le":
                    return AudioProfile.PcmS16Le;
            }
            throw new Exception("Cannot unmarshal type AudioProfile");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AudioProfile)untypedValue;
            switch (value)
            {
                case AudioProfile.Dts:
                    serializer.Serialize(writer, "dts");
                    return;
                case AudioProfile.Lc:
                    serializer.Serialize(writer, "lc");
                    return;
                case AudioProfile.Ma:
                    serializer.Serialize(writer, "ma");
                    return;
                case AudioProfile.PcmS16Le:
                    serializer.Serialize(writer, "pcm_s16le");
                    return;
            }
            throw new Exception("Cannot marshal type AudioProfile");
        }

        public static readonly AudioProfileConverter Singleton = new AudioProfileConverter();
    }

    internal class ContainerConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Container) || t == typeof(Container?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "avi":
                    return Container.Avi;
                case "mkv":
                    return Container.Mkv;
                case "mp4":
                    return Container.Mp4;
            }
            throw new Exception("Cannot unmarshal type Container");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Container)untypedValue;
            switch (value)
            {
                case Container.Avi:
                    serializer.Serialize(writer, "avi");
                    return;
                case Container.Mkv:
                    serializer.Serialize(writer, "mkv");
                    return;
                case Container.Mp4:
                    serializer.Serialize(writer, "mp4");
                    return;
            }
            throw new Exception("Cannot marshal type Container");
        }

        public static readonly ContainerConverter Singleton = new ContainerConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class VideoProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VideoProfile) || t == typeof(VideoProfile?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "advanced simple":
                    return VideoProfile.AdvancedSimple;
                case "constrained baseline":
                    return VideoProfile.ConstrainedBaseline;
                case "high":
                    return VideoProfile.High;
                case "main":
                    return VideoProfile.Main;
                case "simple":
                    return VideoProfile.Simple;
            }
            throw new Exception("Cannot unmarshal type VideoProfile");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VideoProfile)untypedValue;
            switch (value)
            {
                case VideoProfile.AdvancedSimple:
                    serializer.Serialize(writer, "advanced simple");
                    return;
                case VideoProfile.ConstrainedBaseline:
                    serializer.Serialize(writer, "constrained baseline");
                    return;
                case VideoProfile.High:
                    serializer.Serialize(writer, "high");
                    return;
                case VideoProfile.Main:
                    serializer.Serialize(writer, "main");
                    return;
                case VideoProfile.Simple:
                    serializer.Serialize(writer, "simple");
                    return;
            }
            throw new Exception("Cannot marshal type VideoProfile");
        }

        public static readonly VideoProfileConverter Singleton = new VideoProfileConverter();
    }

    internal class AudioCodecConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AudioCodec) || t == typeof(AudioCodec?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "aac":
                    return AudioCodec.Aac;
                case "ac3":
                    return AudioCodec.Ac3;
                case "dca":
                    return AudioCodec.Dca;
                case "dca-ma":
                    return AudioCodec.DcaMa;
                case "eac3":
                    return AudioCodec.Eac3;
                case "flac":
                    return AudioCodec.Flac;
                case "mp3":
                    return AudioCodec.Mp3;
                case "pcm":
                    return AudioCodec.Pcm;
                case "truehd":
                    return AudioCodec.Truehd;
            }
            throw new Exception("Cannot unmarshal type AudioCodec");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AudioCodec)untypedValue;
            switch (value)
            {
                case AudioCodec.Aac:
                    serializer.Serialize(writer, "aac");
                    return;
                case AudioCodec.Ac3:
                    serializer.Serialize(writer, "ac3");
                    return;
                case AudioCodec.Dca:
                    serializer.Serialize(writer, "dca");
                    return;
                case AudioCodec.DcaMa:
                    serializer.Serialize(writer, "dca-ma");
                    return;
                case AudioCodec.Eac3:
                    serializer.Serialize(writer, "eac3");
                    return;
                case AudioCodec.Flac:
                    serializer.Serialize(writer, "flac");
                    return;
                case AudioCodec.Mp3:
                    serializer.Serialize(writer, "mp3");
                    return;
                case AudioCodec.Pcm:
                    serializer.Serialize(writer, "pcm");
                    return;
                case AudioCodec.Truehd:
                    serializer.Serialize(writer, "truehd");
                    return;
            }
            throw new Exception("Cannot marshal type AudioCodec");
        }

        public static readonly AudioCodecConverter Singleton = new AudioCodecConverter();
    }

    internal class VideoCodecConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VideoCodec) || t == typeof(VideoCodec?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "h264":
                    return VideoCodec.H264;
                case "mpeg4":
                    return VideoCodec.Mpeg4;
            }
            throw new Exception("Cannot unmarshal type VideoCodec");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VideoCodec)untypedValue;
            switch (value)
            {
                case VideoCodec.H264:
                    serializer.Serialize(writer, "h264");
                    return;
                case VideoCodec.Mpeg4:
                    serializer.Serialize(writer, "mpeg4");
                    return;
            }
            throw new Exception("Cannot marshal type VideoCodec");
        }

        public static readonly VideoCodecConverter Singleton = new VideoCodecConverter();
    }

    internal class VideoFrameRateConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VideoFrameRate) || t == typeof(VideoFrameRate?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "24p":
                    return VideoFrameRate.The24P;
                case "NTSC":
                    return VideoFrameRate.Ntsc;
                case "PAL":
                    return VideoFrameRate.Pal;
            }
            throw new Exception("Cannot unmarshal type VideoFrameRate");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VideoFrameRate)untypedValue;
            switch (value)
            {
                case VideoFrameRate.The24P:
                    serializer.Serialize(writer, "24p");
                    return;
                case VideoFrameRate.Ntsc:
                    serializer.Serialize(writer, "NTSC");
                    return;
                case VideoFrameRate.Pal:
                    serializer.Serialize(writer, "PAL");
                    return;
            }
            throw new Exception("Cannot marshal type VideoFrameRate");
        }

        public static readonly VideoFrameRateConverter Singleton = new VideoFrameRateConverter();
    }

    internal class VideoResolutionUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VideoResolutionUnion) || t == typeof(VideoResolutionUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    if (stringValue == "sd")
                    {
                        return new VideoResolutionUnion { Enum = VideoResolutionEnum.Sd };
                    }
                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return new VideoResolutionUnion { Integer = l };
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type VideoResolutionUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (VideoResolutionUnion)untypedValue;
            if (value.Enum != null)
            {
                if (value.Enum == VideoResolutionEnum.Sd)
                {
                    serializer.Serialize(writer, "sd");
                    return;
                }
            }
            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value.ToString());
                return;
            }
            throw new Exception("Cannot marshal type VideoResolutionUnion");
        }

        public static readonly VideoResolutionUnionConverter Singleton = new VideoResolutionUnionConverter();
    }

    internal class VideoResolutionEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VideoResolutionEnum) || t == typeof(VideoResolutionEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "sd")
            {
                return VideoResolutionEnum.Sd;
            }
            throw new Exception("Cannot unmarshal type VideoResolutionEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VideoResolutionEnum)untypedValue;
            if (value == VideoResolutionEnum.Sd)
            {
                serializer.Serialize(writer, "sd");
                return;
            }
            throw new Exception("Cannot marshal type VideoResolutionEnum");
        }

        public static readonly VideoResolutionEnumConverter Singleton = new VideoResolutionEnumConverter();
    }

    internal class AudienceRatingImageConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AudienceRatingImage) || t == typeof(AudienceRatingImage?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "imdb://image.rating":
                    return AudienceRatingImage.ImdbImageRating;
                case "themoviedb://image.rating":
                    return AudienceRatingImage.ThemoviedbImageRating;
            }
            throw new Exception("Cannot unmarshal type AudienceRatingImage");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AudienceRatingImage)untypedValue;
            switch (value)
            {
                case AudienceRatingImage.ImdbImageRating:
                    serializer.Serialize(writer, "imdb://image.rating");
                    return;
                case AudienceRatingImage.ThemoviedbImageRating:
                    serializer.Serialize(writer, "themoviedb://image.rating");
                    return;
            }
            throw new Exception("Cannot marshal type AudienceRatingImage");
        }

        public static readonly AudienceRatingImageConverter Singleton = new AudienceRatingImageConverter();
    }

    internal class ChapterSourceConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ChapterSource) || t == typeof(ChapterSource?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "media":
                    return ChapterSource.Media;
                case "mixed":
                    return ChapterSource.Mixed;
            }
            throw new Exception("Cannot unmarshal type ChapterSource");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ChapterSource)untypedValue;
            switch (value)
            {
                case ChapterSource.Media:
                    serializer.Serialize(writer, "media");
                    return;
                case ChapterSource.Mixed:
                    serializer.Serialize(writer, "mixed");
                    return;
            }
            throw new Exception("Cannot marshal type ChapterSource");
        }

        public static readonly ChapterSourceConverter Singleton = new ChapterSourceConverter();
    }

    internal class ViewGroupConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ViewGroup) || t == typeof(ViewGroup?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "movie")
            {
                return ViewGroup.Movie;
            }else if (value == "show")
            {
                return ViewGroup.Show;
            }
            else if (value == "season")
            {
                return ViewGroup.Season;
            }
            else if (value == "episode")
            {
                return ViewGroup.Episode;
            }
            throw new Exception("Cannot unmarshal type ViewGroup");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ViewGroup)untypedValue;
            if (value == ViewGroup.Movie)
            {
                serializer.Serialize(writer, "movie");
                return;
            }
            throw new Exception("Cannot marshal type ViewGroup");
        }

        public static readonly ViewGroupConverter Singleton = new ViewGroupConverter();
    }
}
