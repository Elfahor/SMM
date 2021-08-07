using SubnauticaModManager.ModInstalling;
using System.Diagnostics;
using System.Windows;

namespace SubnauticaModManager.Wpf
{
	/// <summary>
	/// Logique d'interaction pour DownloadPage.xaml.
	/// Allows to download and install mods
	/// </summary>
	public partial class DownloadPage : Window
	{
		//public string htmlDesc { get; }
		//public string modDlUrl { get => $"https://www.nexusmods.com/subnautica/mods/{mod.OnlineInfo.mod_id}?tab=files&file_id=3452"; }

		public DownloadPage(Mod mod)
		{
			InitializeComponent();

			DataContext = this;

			DlBtn.Click += (sender, e) =>
			{
				// open the download page for the mod
//#warning change the way the file is chosen
				Process.Start(new ProcessStartInfo($"https://www.nexusmods.com/subnautica/mods/{mod.OnlineInfo.ModId}?tab=files&file_id={mod.GetLatestMainRelease().file_id}") { UseShellExecute = true });
			};

			DledBtn.Click += (sender, e) =>
			{
				//install the mod and close this window
				ModInstaller.Install(mod);
				Close();
			};
		}
	}
}
