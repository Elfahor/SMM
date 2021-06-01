using System;
using System.IO;
using System.Text.Json;

namespace Subnautica_Mod_Manager
{
	public class Mod
	{
		public Mod(string path)
		{
			Name = Path.GetFileName(path);
			ModJsonPath = path + "\\mod.json";
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

		public string Name { get; set; }

		public string ModJsonPath { get; }

		public ModJsonFile ModJson { get; }

		public string LastVersion { get; set; }

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


		public class ModJsonFile
		{
			public string Id { get; set; }
			public string DisplayName { get; set; }
			public string Author { get; set; }
			public string Version { get; set; }
			public string[] Dependencies { get; set; }
			public string[] LoadBefore { get; set; }
			public bool Enable { get; set; }
			public string Game { get; set; }
			public Nexusid NexusId { get; set; }
			public Versionchecker VersionChecker { get; set; }
			public string AssemblyName { get; set; }

			public struct Nexusid
			{
				public string Subnautica { get; set; }
			}

			public struct Versionchecker
			{
				public string LatestVersionURL { get; set; }
			}
		}

		internal void ApplyModJson()
		{
			string newContent = JsonSerializer.Serialize(ModJson);
			File.WriteAllText(ModJsonPath, newContent);
		}
	}
}