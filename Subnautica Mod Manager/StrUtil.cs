using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subnautica_Mod_Manager
{
	static class StrUtil
	{
		public enum StrContainsOptions
		{
			And, Or
		}
		public static bool Contains(this string str, string[] values, StrContainsOptions options)
		{
			if (options == StrContainsOptions.And)
				return values.All((v) => str.Contains(v));
			else
				return values.Any((v) => str.Contains(v));
		}
	}
}
