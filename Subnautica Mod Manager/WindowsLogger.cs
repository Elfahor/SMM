using SubnauticaModManager.Utils;
using System.Windows;

namespace SubnauticaModManager.Wpf
{
	internal class WindowsLogger : LoggingTarget
	{
		protected override LogType WrittenTypes => LogType.UserInfo | LogType.Error;

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
			MessageBox.Show(message);
		}
	}
}
