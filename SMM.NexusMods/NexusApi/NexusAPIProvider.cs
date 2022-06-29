using System;
using System.Net.Http;

namespace SubnauticaModManager.NexusApi
{
	public static class NexusAPIProvider
	{
		public static HttpClient NexusHttpClient { get; private set; } = new HttpClient() 
		{
			BaseAddress = new Uri("https://api.nexusmods.com/v1/games/subnautica/")
		};

		public static void UpdateRequestHeaders()
		{
			NexusHttpClient.DefaultRequestHeaders.Add("apikey", Settings.Default.NexusApiKey);
		}

	}
}
