using Microsoft.VisualBasic.FileIO; // bad
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Subnautica_Mod_Manager
{
	/// <summary>
	/// Logique d'interaction pour DownloadPage.xaml
	/// </summary>
	public partial class DownloadPage : Window
	{
		public Mod mod { get; }
		public string htmlDesc { get; }
		public string modDlUrl { get => $"https://www.nexusmods.com/subnautica/mods/{mod.OnlineInfo.mod_id}?tab=files&file_id=3452"; }

		public DownloadPage(Mod mod)
		{
			InitializeComponent();

			this.mod = mod;
			this.DataContext = this;

			DlBtn.Click += (sender, e) =>
			{
				Process.Start(new ProcessStartInfo($"https://www.nexusmods.com/subnautica/mods/{mod.OnlineInfo.mod_id}?tab=files&file_id={mod.Files.files[mod.Files.files.Length - 1].file_id}") { UseShellExecute = true });
			};

			DledBtn.Click += (sender, e) =>
			{
				string fullPath = GetModInstallPath(mod);
				string fileExtension = Path.GetExtension(fullPath);

				switch (fileExtension)
				{
					case ".zip":
					case ".7z":
						ModInstaller.HandleZipQMod(fullPath);
						break;
					case ".exe":
						Process.Start(new ProcessStartInfo(fullPath));
						break;
					default:
						MessageBox.Show(
							"This mod cannot be automatically installed. Please refer to its installation instructions",
							"Unable to auto-install",
							MessageBoxButton.OK,
							MessageBoxImage.Warning,
							MessageBoxResult.OK,
							MessageBoxOptions.None
						);
						break;
				}
				Close();
			};
		}

		private static string GetModInstallPath(Mod mod)
		{
			string url = GetUrl(mod);
			Regex pattern = new Regex(@"^https://file-metadata\.nexusmods\.com/file/nexus-files-s3-meta/1155/[0-9]{1,3}/(?<filename>[\w\._\s-]*)");
			Match match = pattern.Match(url);
			StringBuilder fileName = new StringBuilder(match.Groups["filename"].Value);
			fileName.Replace(".json", "");
			string fullPath = Path.Combine(new Syroot.Windows.IO.KnownFolder(Syroot.Windows.IO.KnownFolderType.Downloads).Path, fileName.ToString());
			return fullPath;
		}

		private static string GetUrl(Mod mod)
		{
			Mod.FileInfo.File[] files = mod.Files.files;
			for (int i = files.Length - 1; i >= 0; --i)
			{
				if (files[i].category_id == 1)
				{
					return files[i].content_preview_link;
				}
			}
			throw new ArgumentException("This mod has no MAIN release");
		}

	}
}
