using System.Diagnostics;
using SubnauticaModManager.Utils;
using SubnauticaModManager.NexusApi;

namespace SubnauticaModManager
{
	public static class Backend
	{
		public static void Initialize()
		{
			Logger.AddLoggingTarget(new FileLogger(), new ConsoleLogger());
			Settings.LoadFromFile();
			NexusAPIProvider.UpdateRequestHeaders();
			ModFetcher.InitializeModsLists();
		}

		public static void LaunchGame()
		{
			if (!ModFetcher.DependenciesOk())
			{
				Logger.Log("Some mods have dependencies issues", LogType.UserInfo);
				return;
			}
			using Process game = new();
			Process.Start("steam","steam://rungameid/264710");
			//game.StartInfo.FileName = Path.Combine(Settings.Default.GamePath, "Subnautica.exe");
			//game.Start();
		}
	}
}
