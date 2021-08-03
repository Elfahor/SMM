using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubnauticaModManager.NexusMods
{
	public struct UserData
	{
		public static UserData GetOnline()
		{
			string response = NexusAPIProvider.NexusHttpClient.GetStringAsync("https://api.nexusmods.com/v1/users/validate.json").Result;
			return JsonSerializer.Deserialize<UserData>(response);
		}

		[JsonPropertyName("user_id")]
		public int UserId { get; set; }
		[JsonPropertyName("key")]
		public string Key { get; set; }
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("email")]
		public string Email { get; set; }
		[JsonPropertyName("profile_url")]
		public string ProfileUrl { get; set; }
		[JsonPropertyName("is_supporter")]
		public bool IsSupporter { get; set; }
		[JsonPropertyName("is_premium")]
		public bool IsPremium { get; set; }
	}

}