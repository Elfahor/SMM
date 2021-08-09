using System;
using System.Collections.Generic;
using System.Text;

namespace SubnauticaModManager.Utils
{
	public static class Logger
	{
		private static readonly List<LoggingTarget> s_loggingTargets = new List<LoggingTarget>();

		public static void AddLoggingTarget(LoggingTarget target)
		{
			s_loggingTargets.Add(target);
			target.Enable();
		}

		public static void AddLoggingTarget(params LoggingTarget[] targets)
		{
			foreach (LoggingTarget t in targets)
			{
				AddLoggingTarget(t);
			}
		}

		public static void Log(string message, LogType type = LogType.Info)
		{
			foreach (LoggingTarget target in s_loggingTargets)
			{
				target.Log($"[{DateTime.Now}] {message}", type);
			}
		}
	}
}
