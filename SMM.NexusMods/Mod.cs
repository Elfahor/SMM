using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SubnauticaModManager
{
	public class Mod
	{
		public static readonly string[] ValidCc2Names =
		{
			"CustomFabricators",
			"CustomCraftingTabs",
			"AddedRecipes",
			"AliasRecipes",
			"ModifiedRecipes",
			"CustomFoods",
			"MovedRecipes",
			"CustomFragmentCounts",
			"CustomSizes",
			"CustomBioFuels"
		};


		public static JsonSerializerOptions JsonSerializerOptions =
			new JsonSerializerOptions() { WriteIndented = true };

		public Mod(string path)
		{
			Name = Path.GetFileName(path);
			ModJsonPath = path + $"{Path.DirectorySeparatorChar}mod.json";
			GetFromJson();
		}

		public Mod(DownloadedModData onlineInfo, HttpClient httpClient)
		{
			OnlineInfo = onlineInfo;
			Name = OnlineInfo.Name;
			LastVersion = onlineInfo.Version;

			if (onlineInfo.AvailableOnSite && onlineInfo.Status != "hidden")
			{
				Task<string> responseTask = httpClient.GetStringAsync($"mods/{OnlineInfo.ModId}/files.json");
				string response = responseTask.Result;
				FilePreview = JsonSerializer.Deserialize<NexusFilePreviewInfos>(response);
			}
		}

		public NexusFilePreviewInfos FilePreview { get; private set; }
		public string LastVersion { get; set; }

		public ModJsonFile ModJson { get; private set; }

		public string ModJsonPath { get; }
		public string Name { get; set; }
		public DownloadedModData OnlineInfo { get; private set; }
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}

		public void ApplyModJson()
		{
			string newContent = JsonSerializer.Serialize(ModJson, JsonSerializerOptions);
			File.WriteAllText(ModJsonPath, newContent);
			GetFromJson();
		}

		private void GetFromJson()
		{
			if (File.Exists(ModJsonPath))
			{
				ModJson = JsonSerializer.Deserialize<ModJsonFile>(File.ReadAllText(ModJsonPath));
				ModJson.Dependencies = ModJson.Dependencies ?? new string[0];
			}
			else
			{
				//throw new ArgumentException($"{ModJsonPath} does not exist");
			}
		}

		public class DownloadedModData
		{
			[JsonPropertyName("allow_rating")]
			public bool AllowRating { get; set; }
			[JsonPropertyName("author")]
			public string AuthorName { get; set; }
			[JsonPropertyName("available")]
			public bool AvailableOnSite { get; set; }
			/// <summary>
			/// 1: MAIN release
			/// </summary>
			[JsonPropertyName("category_id")]
			public int Category { get; set; }
			[JsonPropertyName("contains_adult_content")]
			public bool ContainsAdultContent { get; set; }
			/// <summary>
			/// Time at which the mod has been uploaded
			/// </summary>
			[JsonPropertyName("created_time")]
			public DateTime CreationTime { get; set; }
			[JsonPropertyName("created_timestamp")]
			public int CreationTimestamp { get; set; }
			/// <summary>
			/// A long BB-Code description
			/// </summary>
			[JsonPropertyName("description")]
			public string Description { get; set; }
			[JsonPropertyName("domain_name")]
			public string GameModIsFor { get; set; }
			[JsonPropertyName("endorsement")]
			public object EndorsementDetails { get; set; }
			[JsonPropertyName("endorsement_count")]
			public int EndorsementCount { get; set; }
			/// <summary>
			/// Id for the game this mod is for
			/// </summary>
			[JsonPropertyName("game_id")]
			public int GameId { get; set; }
			/// <summary>
			/// Id for the mod
			/// </summary>
			[JsonPropertyName("mod_id")]
			public int ModId { get; set; }
			[JsonPropertyName("name")]
			public string Name { get; set; }
			/// <summary>
			/// Url for the banner this mod has on the Nexus website
			/// </summary>
			[JsonPropertyName("picture_url")]
			public string PictureUrl { get; set; }
			/// <summary>
			/// published, etc...
			/// </summary>
			[JsonPropertyName("status")]
			public string Status { get; set; }
			/// <summary>
			/// A short plain text description
			/// </summary>
			[JsonPropertyName("summary")]
			public string Summary { get; set; }
			/// <summary>
			/// Unique identifier for the mod we don't care about
			/// </summary>
			[JsonPropertyName("uid")]
			public long Uid { get; set; }
			/// <summary>
			/// Last time the mod was updated
			/// </summary>
			[JsonPropertyName("updated_time")]
			public DateTime UpdatedTime { get; set; }
			[JsonPropertyName("updated_timestamp")]
			public int UpdatedTimestamp { get; set; }
			/// <summary>
			/// The user who uploaded the mod, might not be the author. <br/>Ex with QMod: author = QMod Team, uploader = PrimeSonic
			/// </summary>
			[JsonPropertyName("uploaded_by")]
			public string Uploader { get; set; }
			[JsonPropertyName("uploaded_users_profile_url")]
			public string UploaderProfileUrl { get; set; }
			[JsonPropertyName("user")]
			public UploadUser UploaderDetails { get; set; }
			[JsonPropertyName("version")]
			public string Version { get; set; }
			public class UploadUser
			{
				[JsonPropertyName("member_group_id")]
				public int MemberGroupId { get; set; }
				[JsonPropertyName("member_id")]
				public int MemberId { get; set; }
				[JsonPropertyName("name")]
				public string Name { get; set; }
			}
		}

		public class NexusFilePreviewInfos
		{
			[JsonPropertyName("file_updates")]
			public object[] FileUpdates { get; set; }
			[JsonPropertyName("files")]
			public FilePreviewMetadata[] Files { get; set; }
			public class FilePreviewMetadata
			{
				[JsonPropertyName("category_id")]
				public int CategoryId { get; set; }
				public object category_name { get; set; }
				public string changelog_html { get; set; }
				public string content_preview_link { get; set; }
				public string description { get; set; }
				public string external_virus_scan_url { get; set; }
				public int file_id { get; set; }
				public string file_name { get; set; }
				public int[] id { get; set; }
				public bool is_primary { get; set; }
				public string mod_version { get; set; }
				public string name { get; set; }
				public int size { get; set; }
				public int size_kb { get; set; }
				public long uid { get; set; }
				public DateTime uploaded_time { get; set; }
				public int uploaded_timestamp { get; set; }
				public string version { get; set; }
			}

		}

		public class ModJsonFile
		{
			public string AssemblyName { get; set; }
			public string Author { get; set; }
			public string[] Dependencies { get; set; }
			public string DisplayName { get; set; }
			public bool Enable { get; set; }
			public string EntryMethod { get; set; }
			public string Game { get; set; }
			public string Id { get; set; }
			public string[] LoadBefore { get; set; }
			public NexusIdDetails NexusId { get; set; }
			public string Version { get; set; }
			public VersionCheckerDetails VersionChecker { get; set; }
			public struct NexusIdDetails
			{
				public string Subnautica { get; set; }
				public string BelowZero { get; set; }
			}

			public struct VersionCheckerDetails
			{
				public string LatestVersionURL { get; set; }
			}
		}

		public enum DledModType
		{
			QMod,
			CC2,
			CustomPoster,
			CustomHullPlate,
			CustomBattery
		}

		public NexusFilePreviewInfos.FilePreviewMetadata GetLatestMainRelease()
		{
			NexusFilePreviewInfos.FilePreviewMetadata[] files = FilePreview.Files;
			// Linq version
			//return files.Where(f => f.CategoryId == 1).Last() ?? throw new ArgumentException();
			for (int i = files.Length - 1; i >= 0; --i)
			{
				if (files[i].CategoryId == 1)
				{
					return files[i];
				}
			}
			throw new ArgumentException("This mod has no MAIN release");
		}
	}
}