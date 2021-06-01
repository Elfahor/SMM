using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
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
			SearchForLastVerBtn.Click += SearchForLastVerBtn_Click;
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

		private void SearchForLastVerBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (Mod mod in ModList)
			{
				int modId = int.Parse(mod.ModJson.NexusId.Subnautica);
			}
		}

		private void StartGameBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!DependenciesOk())
			{
				MessageBox.Show("Some mods have dependencies issues", "Dependency check", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);
				return;
			}
			using Process game = new Process();
			game.StartInfo.FileName = Path.Combine(Properties.Settings.Default.GamePath, "Subnautica.exe");
			game.Start();
		}

		private bool DependenciesOk() =>
			ModList.All((m) =>
				m.ModJson.Dependencies.All((d) =>
					ModList.Any((mc) => mc.ModJson.Id == d
					)
				)
			);

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
