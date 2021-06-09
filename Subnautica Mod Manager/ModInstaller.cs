using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Windows;

namespace Subnautica_Mod_Manager
{
	internal class ModInstaller
	{
		private const string tmpPathName = "Subnautica Mod Manager";

		public static void HandleZipQMod(string downloadedArchivePath)
		{
			string tmpPathForExtraction = Path.Combine(Path.GetTempPath(), tmpPathName, Path.GetFileName(downloadedArchivePath));
			if (Directory.Exists(tmpPathForExtraction))
			{
				Directory.Delete(tmpPathForExtraction, true);
			}

			DirectoryInfo tmpDirForExtraction = Directory.CreateDirectory(tmpPathForExtraction);
			System.IO.Compression.ZipFile.ExtractToDirectory(downloadedArchivePath, tmpPathForExtraction);

			// Check if the mod is a CC2
			DirectoryInfo actualDirToCopy = GetCC2Directory(tmpDirForExtraction);

			if (actualDirToCopy is null) // Not CC2
			{
				// try CustomPoster
				actualDirToCopy = GetCustomPosterDir(tmpDirForExtraction);
				if (actualDirToCopy is null) // Not CustomPoster
				{
					actualDirToCopy = GetQModDir(tmpDirForExtraction);
					// not custom poster
					if (actualDirToCopy is null)
					{
						Issue("This mod cannot be automatically installed. Please refer to its installation instructions");
					}
					else
					{
						InstallQMod(actualDirToCopy);
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

		private static DirectoryInfo GetCustomPosterDir(DirectoryInfo dir)
		{
			string fileCopyDir = Path.Combine(Path.GetTempPath(), tmpPathName, "cp file copy");
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
			string cc2Path = Path.Combine(Properties.Settings.Default.GamePath, "QMods", "CustomCraft2SML");
			if (!Directory.Exists(cc2Path))
			{
				Issue("Please install CustomCraft2 beforehand");
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
		private static void InstallCustomPoster(DirectoryInfo actualDirToCopy)
		{
			string cpPath = Path.Combine(Properties.Settings.Default.GamePath, "QMods", "CustomPosters", "Posters");
			if (!Directory.Exists(cpPath))
			{
				Issue("Please install CustomPosters beforehand");
				return;
			}
			foreach (DirectoryInfo dir in actualDirToCopy.GetDirectories())
			{
				string path = Path.Combine(cpPath, dir.Name);
				if (Directory.Exists(path))
				{
					Directory.Delete(path);
				}

				FileSystem.MoveDirectory(dir.FullName, path);
			}
		}
		private static void InstallQMod(DirectoryInfo actualDirToCopy)
		{
			string destination = Path.Combine(Properties.Settings.Default.GamePath, "QMods", $"{actualDirToCopy.Name}");
			if (Directory.Exists(destination))
			{
				Directory.Delete(destination);
			}

			// from Microsoft.VisualBasic. Bad.
			if (!FileSystem.FileExists(destination))
			{
				FileSystem.MoveDirectory(actualDirToCopy.FullName, destination);
			}
		}

		private static void Issue(string message)
		{
			MessageBox.Show(
				message,
				"Unable to auto-install",
				MessageBoxButton.OK,
				MessageBoxImage.Warning,
				MessageBoxResult.OK,
				MessageBoxOptions.None
			);
		}
	}
}
