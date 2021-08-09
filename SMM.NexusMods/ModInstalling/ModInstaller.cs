using Microsoft.VisualBasic.FileIO;
using SubnauticaModManager.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubnauticaModManager.ModInstalling
{
	/// <summary>
	/// Handles the installation process of mods that have already been downloaded.
	/// </summary>
	public class ModInstaller
	{
		private const string tmpPathName = "Subnautica Mod Manager";

		/// <summary>
		/// Extracts and correctly places the mod
		/// </summary>
		/// <param name="mod">Mod to install based on its metadata</param>
		public static void Install(Mod mod)
		{
			string fullPath = GetPathAtWhichModWasDownloaded(mod);
			string fileExtension = Path.GetExtension(fullPath);

			switch (fileExtension)
			{
				case ".zip":
				case ".7z":
				case ".rar":
					InstallDownloadedMod(fullPath);
					break;
				case ".exe":
					Process.Start(new ProcessStartInfo(fullPath));
					break;
				default:
					Logger.Log("This mod cannot be automatically installed. Please refer to its installation instructions", LogType.UserInfo);
					break;
			}
		}

		/// <summary>
		/// Gets the path at which a mod has been downloaded from the browser.
		/// </summary>
		/// <param name="mod">the mod</param>
		/// <returns>the path</returns>
		private static string GetPathAtWhichModWasDownloaded(Mod mod)
		{
			string url = GetUrl(mod);
			// use a regex to extract file name
			Regex pattern = new Regex(@"^https://file-metadata\.nexusmods\.com/file/nexus-files-s3-meta/1155/[0-9]{1,3}/(?<filename>[\w\._\s-]*)");
			Match match = pattern.Match(url);
			StringBuilder fileName = new StringBuilder(match.Groups["filename"].Value);
			fileName.Replace(".json", "");
			string fullPath = Path.Combine(FileUtils.GetDownloadPath(), fileName.ToString());
			return fullPath;
		}
		/// <summary>
		/// Get the nexus download URL for the main release
		/// </summary>
		/// <param name="mod"></param>
		/// <returns></returns>
		private static string GetUrl(Mod mod)
		{
			return mod.GetLatestMainRelease().content_preview_link;
			//Mod.NexusFilePreviewInfos.FilePreviewMetadata[] files = mod.FilePreview.Files;
			//for (int i = files.Length - 1; i >= 0; --i)
			//{
			//	if (files[i].CategoryId == 1)
			//	{
			//		return files[i].content_preview_link;
			//	}
			//}
			//throw new ArgumentException("This mod has no MAIN release");
		}

		/// <summary>
		/// Once the mod has been downloaded, we can start extracting and copying files
		/// </summary>
		private static void InstallDownloadedMod(string downloadedArchivePath)
		{
			string tmpPathForExtraction = Path.Combine(Path.GetTempPath(), tmpPathName, Path.GetFileName(downloadedArchivePath));
			if (Directory.Exists(tmpPathForExtraction))
			{
				Directory.Delete(tmpPathForExtraction, true);
			}

			DirectoryInfo tmpDirForExtraction = Directory.CreateDirectory(tmpPathForExtraction);
			FileUtils.ExtractArchive(downloadedArchivePath, tmpPathForExtraction);

			// Check if the mod is a CC2
			DirectoryInfo actualDirToCopy = GetCC2Directory(tmpDirForExtraction);

			if (actualDirToCopy is null) // Not CC2
			{
				// try CustomPoster
				actualDirToCopy = GetCustomPosterDir(tmpDirForExtraction);
				if (actualDirToCopy is null) // Not CustomPoster
				{
					// try CustomHullPlates
					actualDirToCopy = GetCustomHullPlatesDir(tmpDirForExtraction);
					if (actualDirToCopy is null) // not CustomHullPlates
					{
						actualDirToCopy = GetQModDir(tmpDirForExtraction);
						if (actualDirToCopy is null) // not QMod
						{
							Logger.Log("This mod cannot be automatically installed. Please refer to its installation instructions", LogType.UserInfo);
							return;
						}
						else
						{
							InstallQMod(actualDirToCopy);
						}
					}
					else
					{
						InstallCustomHullPlates(actualDirToCopy);
					}
				}
				else
				{
					InstallCustomPoster(actualDirToCopy);
				}
			}
			else // mod is CC2
			{
				InstallCC2(actualDirToCopy);
			}

			actualDirToCopy.Delete(true);
			tmpDirForExtraction.Delete(true);
		}

		private static DirectoryInfo GetCC2Directory(DirectoryInfo dir)
		{
			string fileCopyDir = Path.Combine(Path.GetTempPath(), tmpPathName, "cc2 file copy");
			Directory.CreateDirectory(fileCopyDir);

			copyFiles(dir, fileCopyDir);
			if (Directory.GetDirectories(Path.Combine(fileCopyDir, "WorkingFiles")).Length == 0)
			{
				Directory.Delete(fileCopyDir, true);
				return null;
			}
			else
			{
				return new DirectoryInfo(fileCopyDir);
			}

			static void copyFiles(DirectoryInfo dir, string fileCopyDir)
			{
				string workingFiles = Path.Combine(fileCopyDir, "WorkingFiles");
				string assets = Path.Combine(fileCopyDir, "Assets");
				Directory.CreateDirectory(workingFiles);
				Directory.CreateDirectory(assets);
				foreach (FileInfo f in dir.EnumerateFiles())
				{
					if (f.Extension == ".txt" && !f.Name.Contains("README"))
					{
						string content = File.ReadAllText(f.FullName);
						if (content.Contains(Mod.ValidCC2Names, StrUtil.StrContainsOptions.Or))
						{
							f.CopyTo(Path.Combine(workingFiles, f.Name));
						}
					}
					if (f.Extension == ".png")
					{
						string destFileName = Path.Combine(assets, f.Name);
						if (File.Exists(destFileName))
						{
							File.Delete(destFileName);
						}

						f.CopyTo(destFileName);
					}
				}
				foreach (DirectoryInfo f in dir.EnumerateDirectories())
				{
					copyFiles(f, fileCopyDir);
				}
			}
		}

		private static DirectoryInfo GetCustomHullPlatesDir(DirectoryInfo dir)
		{
			string fileCopyDir = Path.Combine(Path.GetTempPath(), tmpPathName, "chp file copy");
			Directory.CreateDirectory(fileCopyDir);

			copyFiles(dir, fileCopyDir);
			if (Directory.GetDirectories(fileCopyDir).Length == 0)
			{
				Directory.Delete(fileCopyDir, true);
				return null;
			}
			else
			{
				return new DirectoryInfo(fileCopyDir);
			}

			static void copyFiles(DirectoryInfo from, in string to)
			{
				foreach (FileInfo f in from.EnumerateFiles())
				{
					string platePath = Path.Combine(to, from.Name);
					Directory.CreateDirectory(platePath);
					if (f.Name == "info.json")
					{
						string content = File.ReadAllText(f.FullName);
						if (content.Contains("InternalName") && content.Contains("DisplayName") && content.Contains("Description"))
						{
							f.CopyTo(Path.Combine(platePath, f.Name));
						}
					}
					if (f.Name == "icon.png")
					{
						f.CopyTo(Path.Combine(to, from.Name, f.Name));
					}

					if (f.Name == "texture.png")
					{
						f.CopyTo(Path.Combine(to, from.Name, f.Name));
					}
					if (Directory.GetFileSystemEntries(platePath).Length == 0)
					{
						Directory.Delete(platePath);
					}
				}
				foreach (DirectoryInfo d in from.EnumerateDirectories())
				{
					copyFiles(d, to);
				}

			}
		}

		private static DirectoryInfo GetCustomPosterDir(DirectoryInfo dir)
		{
			string fileCopyDir = Path.Combine(Path.GetTempPath(), tmpPathName, "cp file copy");
			Directory.CreateDirectory(fileCopyDir);

			copyFiles(dir, fileCopyDir);
			if (Directory.GetDirectories(fileCopyDir).Length == 0 || Directory.GetDirectories(fileCopyDir).Any(d => Directory.GetFiles(d).Length != 3))
			{
				Directory.Delete(fileCopyDir, true);
				return null;
			}
			else
			{
				return new DirectoryInfo(fileCopyDir);
			}

			static void copyFiles(DirectoryInfo from, in string to)
			{
				foreach (FileInfo f in from.EnumerateFiles())
				{
					string posterPath = Path.Combine(to, from.Name);
					Directory.CreateDirectory(posterPath);
					if (f.Name == "info.json")
					{
						string content = File.ReadAllText(f.FullName);
						if (content.Contains("InternalName") && content.Contains("Orientation") && content.Contains("DisplayName") && content.Contains("Description"))
						{
							f.CopyTo(Path.Combine(posterPath, f.Name));
						}
					}
					if (f.Name == "icon.png")
					{
						f.CopyTo(Path.Combine(to, from.Name, f.Name));
					}

					if (f.Name == "texture.png")
					{
						f.CopyTo(Path.Combine(to, from.Name, f.Name));
					}
					if (Directory.GetFileSystemEntries(posterPath).Length == 0)
					{
						Directory.Delete(posterPath);
					}
				}
				foreach (DirectoryInfo d in from.EnumerateDirectories())
				{
					copyFiles(d, to);
				}

			}
		}

		private static DirectoryInfo GetQModDir(DirectoryInfo dir)
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
				DirectoryInfo isDir = GetQModDir(f);
				if (isDir is null)
				{
					continue;
				}
				else
				{
					return isDir;
				}
			}
			return null;
		}

		private static void InstallCC2(DirectoryInfo actualDirToCopy)
		{
			string cc2Path = Path.Combine(Settings.Default.GamePath, "QMods", "CustomCraft2SML");
			if (!Directory.Exists(cc2Path))
			{
				Logger.Log("Please install CustomCraft2 beforehand", LogType.UserInfo);
				return;
			}
			string workingFilesDestination = Path.Combine(cc2Path, "WorkingFiles");
			string assetsDestination = Path.Combine(cc2Path, "Assets");
			Directory.CreateDirectory(workingFilesDestination);
			Directory.CreateDirectory(assetsDestination);
			foreach (FileInfo file in new DirectoryInfo(Path.Combine(actualDirToCopy.FullName, "WorkingFiles")).GetFiles())
			{
				string destFileName = Path.Combine(workingFilesDestination, file.Name);
				if (FileSystem.FileExists(destFileName))
				{
					FileSystem.DeleteFile(destFileName);
				}

				file.MoveTo(destFileName);
			}
			foreach (FileInfo file in new DirectoryInfo(Path.Combine(actualDirToCopy.FullName, "Assets")).GetFiles())
			{
				string destFileName = Path.Combine(assetsDestination, file.Name);
				if (FileSystem.FileExists(destFileName))
				{
					FileSystem.DeleteFile(destFileName);
				}

				file.MoveTo(destFileName);
			}
		}

		private static void InstallCustomHullPlates(DirectoryInfo actualDirToCopy)
		{
			string cpPath = Path.Combine(Settings.Default.GamePath, "QMods", "CustomHullPlates", "HullPlates");
			if (!Directory.Exists(cpPath))
			{
				Logger.Log("Please install CustomPosters beforehand", LogType.UserInfo);
				return;
			}
			foreach (DirectoryInfo dir in actualDirToCopy.GetDirectories())
			{
				string path = Path.Combine(cpPath, dir.Name);
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
				}

				FileSystem.MoveDirectory(dir.FullName, path);
			}
		}
		private static void InstallCustomPoster(DirectoryInfo actualDirToCopy)
		{
			string cpPath = Path.Combine(Settings.Default.GamePath, "QMods", "CustomPosters", "Posters");
			if (!Directory.Exists(cpPath))
			{
				Logger.Log("Please install CustomPosters beforehand", LogType.UserInfo);
				return;
			}
			foreach (DirectoryInfo dir in actualDirToCopy.GetDirectories())
			{
				string path = Path.Combine(cpPath, dir.Name);
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
				}

				FileSystem.MoveDirectory(dir.FullName, path);
			}
		}
		private static void InstallQMod(DirectoryInfo actualDirToCopy)
		{
			string destination = Path.Combine(Settings.Default.GamePath, "QMods", $"{actualDirToCopy.Name}");
			if (Directory.Exists(destination))
			{
				Directory.Delete(destination);
			}

			// from Microsoft.VisualBasic. Bad.
			if (!FileSystem.FileExists(destination))
			{
				FileSystem.CopyDirectory(actualDirToCopy.FullName, destination);
			}
		}
	}
}
