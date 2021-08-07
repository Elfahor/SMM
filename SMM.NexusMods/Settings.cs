using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace SubnauticaModManager.NexusApi
{
	/// <summary>
	/// User configuration
	/// </summary>
	[Serializable]
	public class Settings
	{
		private static readonly string s_pathToSave = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cfg");
		private static readonly BinaryFormatter s_formatter = new BinaryFormatter();
		private static Settings s_main;
		/// <summary>
		/// Singleton pattern
		/// </summary>
		public static Settings Default => s_main ??= new Settings();

		#region Actual Settings
		public string GamePath { get; set; } = @"D:/Programmes/steamapps/common/Subnautica";
		public string NexusApiKey { get; set; } = "c1Brb0Vxck51a0c1bU5KRGxHbGg5RkFCRHBSYXlEWGdNanpNU3ZnbEdPMWRQc201YVRvS1ZGSU8yWElmTWNuNi0tR05CN0ZMRHJBNUlrL1ZGdWZsUjhsdz09--febdeecd49139282d0b653bebd72120eebcf7c7e";
		public bool SaveApiKey { get; set; } = true;
		#endregion

		public static void SaveToFile()
		{
			//using FileStream f = File.Open(s_pathToSave, FileMode.Create);
			string json = JsonSerializer.Serialize(s_main);
			byte[] coded = Encoding.UTF8.GetBytes(json);
			string raw = Convert.ToBase64String(coded);
			File.WriteAllText(s_pathToSave, raw);
			//s_formatter.Serialize(f, s_main);
		}

		public static void LoadFromFile()
		{
			try
			{
				//using FileStream f = File.Open(s_pathToSave, FileMode.Open);
				string raw = File.ReadAllText(s_pathToSave);
				byte[] decoded = Convert.FromBase64String(raw);
				string json = Encoding.UTF8.GetString(decoded);
				s_main = JsonSerializer.Deserialize<Settings>(json);
				//s_main = s_formatter.Deserialize(f) as Settings;
			}
			catch (System.Runtime.Serialization.SerializationException)
			{
				s_main = default;
			}
			catch (FileNotFoundException)
			{
				s_main = default;
			}
		}
	}
}
