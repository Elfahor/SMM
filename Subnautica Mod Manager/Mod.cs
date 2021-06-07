using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Subnautica_Mod_Manager
{
	public class Mod
	{
		public static readonly string[] ValidCC2Names =
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
			ModJsonPath = path + "\\mod.json";
			GetFromJson();
		}

		public Mod(DownloadedModData onlineInfo, HttpClient httpClient)
		{
			OnlineInfo = onlineInfo;
			Name = OnlineInfo.name;
			LastVersion = onlineInfo.version;

			if (onlineInfo.available && onlineInfo.status != "hidden")
			{
				Task<string> responseTask = httpClient.GetStringAsync($"mods/{OnlineInfo.mod_id}/files.json");
				string response = responseTask.Result;
				Files = JsonSerializer.Deserialize<FileInfo>(response);
			}
		}

		public FileInfo Files { get; private set; }
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

		internal void ApplyModJson()
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
			public bool allow_rating { get; set; }
			public string author { get; set; }
			public bool available { get; set; }
			public int category_id { get; set; }
			public bool contains_adult_content { get; set; }
			public DateTime created_time { get; set; }
			public int created_timestamp { get; set; }
			public string description { get; set; }
			public string domain_name { get; set; }
			public object endorsement { get; set; }
			public int endorsement_count { get; set; }
			public int game_id { get; set; }
			public int mod_id { get; set; }
			public string name { get; set; }
			public string picture_url { get; set; }
			public string status { get; set; }
			public string summary { get; set; }
			public long uid { get; set; }
			public DateTime updated_time { get; set; }
			public int updated_timestamp { get; set; }
			public string uploaded_by { get; set; }
			public string uploaded_users_profile_url { get; set; }
			public User user { get; set; }
			public string version { get; set; }
			public class User
			{
				public int member_group_id { get; set; }
				public int member_id { get; set; }
				public string name { get; set; }
			}
		}
		public class FileInfo
		{

			public object[] file_updates { get; set; }
			public File[] files { get; set; }
			public class File
			{
				public int category_id { get; set; }
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
			public Nexusid NexusId { get; set; }
			public string Version { get; set; }
			public Versionchecker VersionChecker { get; set; }
			public struct Nexusid
			{
				public string Subnautica { get; set; }
			}

			public struct Versionchecker
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
	}
}