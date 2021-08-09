using System;

namespace SubnauticaModManager.Utils
{
	public class ConsoleLogger : LoggingTarget
	{
		protected override LogType WrittenTypes => LogType.UserInfo | LogType.Error | LogType.Info;

		public override void Disable()
		{
			enabled = false;
		}

		public override void Enable()
		{
			enabled = true;
		}

		protected override void LogMessage(string message, LogType type)
		{
			Console.WriteLine(message);
		}
	}
}
