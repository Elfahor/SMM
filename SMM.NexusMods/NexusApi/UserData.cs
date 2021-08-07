using System.Text.Json;
using System.Text.Json.Serialization;

namespace SubnauticaModManager.NexusApi
{
	public struct UserData
	{
		/// <summary>
		/// Returns UserData for the current user, implied by the <see cref="NexusAPIProvider.NexusHttpClient"/> apikey header
		/// </summary>
		/// <returns></returns>
		public static UserData GetOnline()
		{
			string response = NexusAPIProvider.NexusHttpClient.GetStringAsync("https://api.nexusmods.com/v1/users/validate.json").Result;
			return JsonSerializer.Deserialize<UserData>(response);
		}

		/// <summary>
		/// Unique identifier of the user
		/// </summary>
		[JsonPropertyName("user_id")]
		public int UserId { get; set; }
		[JsonPropertyName("key")]
		public string Key { get; set; }
		/// <summary>
		/// Username
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; set; }
		/// <summary>
		/// email of the user
		/// </summary>
		[JsonPropertyName("email")]
		public string Email { get; set; }
		/// <summary>
		/// URL of the user on the Nexus website
		/// </summary>
		[JsonPropertyName("profile_url")]
		public string ProfileUrl { get; set; }
		/// <summary>
		/// If the user is a supporter?
		/// </summary>
		[JsonPropertyName("is_supporter")]
		public bool IsSupporter { get; set; }
		/// <summary>
		///Being premium allows to automatically download mods and get access to a lot of Nexus features.
		/// </summary>
		[JsonPropertyName("is_premium")]
		public bool IsPremium { get; set; }
	}

}