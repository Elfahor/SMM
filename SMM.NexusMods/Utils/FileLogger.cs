using System;
using System.IO;
using System.Text;

namespace SubnauticaModManager.Utils
{
	public class FileLogger : LoggingTarget, IDisposable
	{
		private readonly string path = "smm.log";
		private FileStream file;

		protected override LogType WrittenTypes => LogType.Error | LogType.Info | LogType.UserInfo;

		public override void Disable()
		{
			enabled = false;
			file.Flush();
			file.Close();
			file.Dispose();
		}

		public void Dispose()
		{
			file.Flush();
			file.Close();
			file.Dispose();
		}

		public override void Enable()
		{
			enabled = true;
			file = File.Open(path, FileMode.OpenOrCreate);
		}

		protected override void LogMessage(string message, LogType type)
		{
			file.Write(Encoding.UTF8.GetBytes(message), 0, message.Length);
		}
	}
}
