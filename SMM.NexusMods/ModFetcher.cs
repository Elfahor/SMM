using SubnauticaModManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace SubnauticaModManager
{
	public static class ModFetcher
	{
		public static List<Mod> InstalledMods;
		public static List<Mod> PopularMods;
		public static List<Mod> LatestMods;

		public static void InitializeModsLists()
		{
			InitializeLocalMods();
			InitializeOnlineMods();
		}

		public static void InitializeOnlineMods()
		{
			PopularMods = GetMods(ModsToShow.ShowPopular);
			LatestMods = GetMods(ModsToShow.ShowLatest);
		}

		public static void InitializeLocalMods()
		{
			InstalledMods = GetMods(ModsToShow.ShowInstalled);
		}

		public static bool DependenciesOk() =>
			InstalledMods.All(m =>
				m.ModJson.Dependencies.All(d =>
					InstalledMods.Any(mc => mc.ModJson.Id == d
					)
				)
			);

		public enum ModsToShow
		{
			ShowInstalled,
			ShowLatest,
			ShowPopular
		}

		public static List<Mod> GetMods(ModsToShow modsToShow)
		{
			List<Mod> mods = new List<Mod>();

			if (modsToShow == ModsToShow.ShowInstalled)
			{
				string modDir = Settings.Default.GamePath + "/QMods";
				if (Directory.Exists(modDir))
				{
					string[] installedMods = Directory.GetDirectories(modDir, "*", SearchOption.TopDirectoryOnly);
				
					foreach (string mod in installedMods)
					{
						if (Path.GetFileName(mod) != ".backups")
						{
							mods.Add(new Mod(mod));
						}
					}
				}
				else
				{
					Logger.Log($"Path not found {Settings.Default.GamePath}", LogType.Error);
				}

			}
			else if (modsToShow == ModsToShow.ShowLatest)
			{
				try
				{
					string response = NexusApi.NexusAPIProvider.NexusHttpClient.GetStringAsync("mods/latest_added.json").Result;
					List<Mod.DownloadedModData> downloadedModData = JsonSerializer.Deserialize<List<Mod.DownloadedModData>>(response);
					foreach (Mod.DownloadedModData item in downloadedModData)
					{
						mods.Add(new Mod(item, NexusApi.NexusAPIProvider.NexusHttpClient));
					}
				}
				catch (AggregateException e)
				{
					if (e.InnerException is HttpRequestException)
					{
						Logger.Log("Couldn't fetch the latest mods", LogType.UserInfo);
						return null;
					}
				}
			}
			else if (modsToShow == ModsToShow.ShowPopular)
			{
				try
				{
					string response = NexusApi.NexusAPIProvider.NexusHttpClient.GetStringAsync("mods/trending.json").Result;
					List<Mod.DownloadedModData> downloadedModData = JsonSerializer.Deserialize<List<Mod.DownloadedModData>>(response);
					foreach (Mod.DownloadedModData item in downloadedModData)
					{
						mods.Add(new Mod(item, NexusApi.NexusAPIProvider.NexusHttpClient));
					}
				}
				catch (AggregateException e)
				{
					if (e.InnerException is HttpRequestException)
					{
						Logger.Log("Couldn't fetch the popular mods", LogType.UserInfo);
						return null;
					}
				}
			}

			return mods;
		}

		public static void ApplyJsonModifsToLocals()
		{
			InstalledMods.ForEach(m => m.ApplyModJson());
		}
	}
}
