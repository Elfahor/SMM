﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace SubnauticaModManager.NexusMods
{
	/// <summary>
	/// User configuration
	/// </summary>
	[Serializable]
	public class Settings
	{
		static readonly string s_pathToSave = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cfg");
		static readonly BinaryFormatter s_formatter = new BinaryFormatter();

		static Settings s_main;
		/// <summary>
		/// Singleton pattern
		/// </summary>
		public static Settings Default => s_main ??= new Settings();

		#region Actual Settings
		public string GamePath { get; set; } = @"D:/Programmes/steamapps/common/Subnautica";
		public string NexusApiKey { get; set; }
		public bool SaveApiKey { get; set; }
		#endregion

		public static void SaveToFile()
		{
			using (var f = File.Open(s_pathToSave, FileMode.Create))
			{
				s_formatter.Serialize(f, s_main);
			}
		}

		public static void LoadFromFile()
		{
			using (var f = File.Open(s_pathToSave, FileMode.Open))
			{
				s_main = s_formatter.Deserialize(f) as Settings;
			}
		}
	}
}
