using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
namespace MonkFixPlexDB
{

	public partial class PlexUser
	{
		public User User { get; set; }
	}

	public partial class User
	{
		public long Id { get; set; }
		public string Uuid { get; set; }
		public string Email { get; set; }
		public DateTimeOffset JoinedAt { get; set; }
		public string Username { get; set; }
		public string Title { get; set; }
		public Uri Thumb { get; set; }
		public bool HasPassword { get; set; }
		public string AuthToken { get; set; }
		public string AuthenticationToken { get; set; }
		public Subscription Subscription { get; set; }
		public Roles Roles { get; set; }
		public List<string> Entitlements { get; set; }
		[JsonProperty("confirmedAt", NullValueHandling = NullValueHandling.Ignore)]
		public DateTimeOffset ConfirmedAt { get; set; }
		[JsonProperty("forumId", NullValueHandling = NullValueHandling.Ignore)]
		public object ForumId { get; set; }
		public bool RememberMe { get; set; }
	}

	public partial class Roles
	{
		public List<string> RolesRoles { get; set; }
	}

	public partial class Subscription
	{
		public bool Active { get; set; }
		public string Status { get; set; }
		public string Plan { get; set; }
		public List<string> Features { get; set; }
	}

	public class media_parts
	{
		[PrimaryKey, AutoIncrement, Indexed]

		public long id { get; set; }
		public long media_item_id { get; set; }
		public long directory_id { get; set; }
		public string hash { get; set; }
		public string open_subtitle_hash { get; set; }
		public string file { get; set; }
		public long index { get; set; }
		public long size { get; set; }
		public long duration { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public DateTime deleted_at { get; set; }
		public string extra_data { get; set; }
	}
	public class metadata_items
	{
		[PrimaryKey, AutoIncrement, Indexed]
		public long id { get; set; }
		public long library_section_id { get; set; }
		public long parent_id { get; set; }
		public long metadata_type { get; set; }
		public string guid { get; set; }
		public long media_item_count { get; set; }
		public string title { get; set; }
		public string title_sort { get; set; }
		public string original_title { get; set; }
		public string studio { get; set; }
		public float rating { get; set; }
		public long rating_count { get; set; }
		public string tagline { get; set; }
		public string summary { get; set; }
		public string trivia { get; set; }
		public string quotes { get; set; }
		public string content_rating { get; set; }
		public long content_rating_age { get; set; }
		public long index { get; set; }
		public long absolute_index { get; set; }
		public long duration { get; set; }
		public string user_thumb_url { get; set; }
		public string user_art_url { get; set; }
		public string user_banner_url { get; set; }
		public string user_music_url { get; set; }
		public string user_fields { get; set; }
		public string tags_genre { get; set; }
		public string tags_collection { get; set; }
		public string tags_director { get; set; }
		public string tags_writer { get; set; }
		public string tags_star { get; set; }
		public DateTime originally_available_at { get; set; }
		public DateTime available_at { get; set; }
		public DateTime expires_at { get; set; }
		public DateTime refreshed_at { get; set; }
		public long year { get; set; }
		public DateTime added_at { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public DateTime deleted_at { get; set; }
		public string tags_country { get; set; }
		public string extra_data { get; set; }
		public string hash { get; set; }
		public float audience_rating { get; set; }
		public long changed_at { get; set; }
		public long resources_changed_at { get; set; }
		public long remote { get; set; }
	}

	public class media_items
	{
		[PrimaryKey, AutoIncrement, Indexed]
		public long id { get; set; }
		public long library_section_id { get; set; }
		public long section_location_id { get; set; }
		public long metadata_item_id { get; set; }
		public long type_id { get; set; }
		public long width { get; set; }
		public long height { get; set; }
		public long size { get; set; }
		public long duration { get; set; }
		public long bitrate { get; set; }
		public string container { get; set; }
		public string video_codec { get; set; }
		public string audio_codec { get; set; }
		public float display_aspect_ratio { get; set; }
		public float frames_per_second { get; set; }
		public long audio_channels { get; set; }
		public bool interlaced { get; set; }
		public string source { get; set; }
		public string hints { get; set; }
		public long display_offset { get; set; }
		public string settings { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public bool optimized_for_streaming { get; set; }
		public DateTime deleted_at { get; set; }
		public long media_analysis_version { get; set; }
		public float sample_aspect_ratio { get; set; }
		public string extra_data { get; set; }
		public long proxy_type { get; set; }
		public long channel_id { get; set; }
		public DateTime begins_at { get; set; }
		public DateTime ends_at { get; set; }
		public string color_trc { get; set; }
	}
}
