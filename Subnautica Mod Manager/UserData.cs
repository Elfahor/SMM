using System.Net.Http;
using System.Text.Json;

namespace Subnautica_Mod_Manager
{

	public struct UserData
	{
		public UserData(HttpClient httpClient)
		{
			HttpResponseMessage response = httpClient.GetAsync("https://api.nexusmods.com/v1/users/validate.json").Result;
			this = JsonSerializer.Deserialize<UserData>(response.Content.ReadAsStringAsync().Result);
		}

		public int user_id { get; set; }
		public string key { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string profile_url { get; set; }
		public bool is_supporter { get; set; }
		public bool is_premium { get; set; }
	}

}