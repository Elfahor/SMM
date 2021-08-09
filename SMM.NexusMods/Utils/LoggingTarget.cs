using System;

namespace SubnauticaModManager.Utils
{
	public abstract class LoggingTarget
	{
		protected bool enabled;

		public abstract void Enable();
		public abstract void Disable();
		public void Log(string message, LogType type = LogType.Info)
		{
			if (enabled && ((type & WrittenTypes) == type))
			{
				LogMessage(message, type);
			}
		}

		protected abstract void LogMessage(string message, LogType type);

		protected abstract LogType WrittenTypes { get; }
	}
}
