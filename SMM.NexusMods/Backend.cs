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
	}
}
