using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace Subnautica_Mod_Manager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<Mod> ModList = new List<Mod>();

		public MainWindow()
		{
			InitializeComponent();

			OpenSettingsBtn.Click += OpenSettingsBtn_Click;
			ApplyModifsBtn.Click += ApplyModifsBtn_Click;
			StartGameBtn.Click += StartGameBtn_Click;

			string[] installedMods = Directory.GetDirectories(Properties.Settings.Default.GamePath + "\\QMods", "*", SearchOption.TopDirectoryOnly);

			foreach (string mod in installedMods)
			{
				if (Path.GetFileName(mod) != ".backups")
				{
					ModList.Add(new Mod(mod));
				}
			}

			modListControl.ItemsSource = ModList;
		}

		private void StartGameBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!AreDependenciesFine())
			{
				MessageBox.Show("Some mods have dependencies issues", "Dependency check", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);
				return;
			}
			using Process game = new Process();
			game.StartInfo.FileName = Path.Combine(Properties.Settings.Default.GamePath, "Subnautica.exe");
			game.Start();
		}

		private bool AreDependenciesFine()
		{
			foreach (Mod mod in ModList)
			{
				foreach (string dependencie in mod.ModJson.Dependencies)
				{
					if (!ModList.Any((m) => m.ModJson.DisplayName == dependencie)) return false;
				}
			}
			return true;
		}

		private void ApplyModifsBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (Mod mod in ModList)
			{
				mod.ApplyModJson();
			}
		}

		private void OpenSettingsBtn_Click(object sender, RoutedEventArgs e)
		{
			SettingsDialog settingsDialog = new SettingsDialog()
			{
				Owner = this,
				DataContext = this.DataContext
			};
			settingsDialog.ShowDialog();
		}
	}
}
