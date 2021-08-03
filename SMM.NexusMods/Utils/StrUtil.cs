using System.Linq;

namespace SubnauticaModManager.CommonUtils
{
	internal static class StrUtil
	{
		public enum StrContainsOptions
		{
			And, Or
		}
		public static bool Contains(this string str, string[] values, StrContainsOptions options)
		{
			if (options == StrContainsOptions.And)
			{
				return values.All((v) => str.Contains(v));
			}
			else
			{
				return values.Any((v) => str.Contains(v));
			}
		}
	}
}
