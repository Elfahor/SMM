using System;
using System.IO;
using System.Text.Json;

namespace Subnautica_Mod_Manager
{
	public class Mod
	{
		public static JsonSerializerOptions JsonSerializerOptions =
			new JsonSerializerOptions() { WriteIndented = true };

		public Mod(string path)
		{
			Name = Path.GetFileName(path);
			ModJsonPath = path + "\\mod.json";
			GetFromJson();
		}

		public Mod()
		{

		}

		public Mod(DownloadedModData onlineInfo)
		{
			OnlineInfo = onlineInfo;
			Name = OnlineInfo.name;
			LastVersion = onlineInfo.version;
		}

		public string LastVersion { get; set; }

		public ModJsonFile ModJson { get; private set; }

		public DownloadedModData OnlineInfo { get; private set; }

		public string ModJsonPath { get; }

		public string Name { get; set; }

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
				throw new ArgumentException($"{ModJsonPath} does not exist");
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
	}
}