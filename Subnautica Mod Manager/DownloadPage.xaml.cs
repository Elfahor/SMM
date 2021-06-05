using Microsoft.VisualBasic.FileIO;
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
			//var bbcodeParser = new BBCodeParser(new[]
			//{
			//	new BBTag("b", "<b>", "</b>"),
			//	new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
			//	new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
			//	new BBTag("code", "<pre class=\"prettyprint\">", "</pre>"),
			//	new BBTag("img", "<img src=\"${content}\" />", "", false, true),
			//	new BBTag("quote", "<blockquote>", "</blockquote>"),
			//	new BBTag("list", "<ul>", "</ul>"),
			//	new BBTag("*", "<li>", "</li>", true, false),
			//	new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
			//	new BBTag("color", "<span style=\"color:${color}\"", "</span>", new BBAttribute("color", "")),
			//	new BBTag("size", "<a style=\"font-size:${size}px\"", "</a>", new BBAttribute("size", "")),
			//});
			//htmlDesc = bbcodeParser.ToHtml(mod.OnlineInfo.description);
			this.DataContext = this;

			//DescFrame.Source = new System.Uri(modDlUrl);

			DlBtn.Click += (sender, e) =>
			{
				Process.Start(new ProcessStartInfo($"https://www.nexusmods.com/subnautica/mods/{mod.OnlineInfo.mod_id}?tab=files&file_id=3452") { UseShellExecute = true });
			};

			DledBtn.Click += (sender, e) =>
			{
				string fullPath = GetModInstallPath(mod);
				string fileExtension = Path.GetExtension(fullPath);

				switch (fileExtension)
				{
					case ".zip":
						HandleZipQMod(fullPath);
						break;
					case ".exe":
						Process.Start(new ProcessStartInfo(fullPath));
						break;
					default:
						MessageBox.Show("This mod cannot be automatically installed. Please refer to its installation instructions", "Unable to auto-install", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.None);
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

		private void HandleZipQMod(string fullPath)
		{
			string tmpPath = Path.Combine(Path.GetTempPath(), "Subnautica Mod Manager", Path.GetFileName(fullPath));
			if (Directory.Exists(tmpPath))
			{
				Directory.Delete(tmpPath, true);
			}

			DirectoryInfo dir = Directory.CreateDirectory(tmpPath);

			ZipFile.ExtractToDirectory(fullPath, tmpPath);
			DirectoryInfo actualQmodDir = getQmodDir(dir);



			string destination = Path.Combine(Properties.Settings.Default.GamePath, "QMods", $"{actualQmodDir.Name}");
			if (Directory.Exists(destination))
			{
				Directory.Delete(destination);
			}

			FileSystem.MoveDirectory(actualQmodDir.FullName, destination);

			dir.Delete(true);

			static DirectoryInfo getQmodDir(DirectoryInfo dir)
			{
				foreach (FileInfo f in dir.EnumerateFiles())
				{
					if (f.Name == "mod.json")
					{
						return dir;
					}
				}
				foreach (DirectoryInfo f in dir.EnumerateDirectories())
				{
					return getQmodDir(f);
				}
				return null;
			}
		}
	}
}
