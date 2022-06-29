using System;
using System.Collections.Generic;
using System.Linq;

namespace SubnauticaModManager.Utils
{
	public static class Logger
	{
		private static readonly List<LoggingTarget> loggingTargets = new List<LoggingTarget>();

		public static void AddLoggingTarget(LoggingTarget target)
		{
			loggingTargets.Add(target);
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
			foreach (LoggingTarget target in loggingTargets)
			{
				LogToTarget(message, type, target);
			}
		}

		private static void LogToTarget(string message, LogType type, LoggingTarget target)
		{
			target.Log($"[{DateTime.Now}] {message}", type);
		}

		public static void Log(string message, Type target, LogType type = LogType.Info)
		{
			foreach (LoggingTarget t in loggingTargets.Where(t => t.GetType() == target))
			{
				LogToTarget(message, type, t);
			}
		}
	}
}
