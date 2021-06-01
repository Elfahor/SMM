using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace Subnautica_Mod_Manager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<Mod> InstalledModList;
		public List<Mod> LatestModList;
		public List<Mod> PopularModList;
		private readonly HttpClient httpClient = new HttpClient();

		public MainWindow()
		{
			InitializeComponent();

			OpenSettingsBtn.Click += OpenSettingsBtn_Click;
			ApplyModifsBtn.Click += ApplyModifsBtn_Click;
			SearchForLastVerBtn.Click += SearchForLastVerBtn_Click;
			StartGameBtn.Click += StartGameBtn_Click;

			httpClient.BaseAddress = new Uri("https://api.nexusmods.com/v1/games/subnautica/");
			httpClient.DefaultRequestHeaders.Add("apikey", Properties.Settings.Default.NexusApiKey);

			InstalledModList = GetMods(ModsToShow.ShowInstalled);
			LatestModList = GetMods(ModsToShow.ShowLatest);
			PopularModList = GetMods(ModsToShow.ShowPopular);

			string[] listViewModes = Enum.GetNames(typeof(ModsToShow));
			ShowWhatComboBox.ItemsSource = listViewModes;
			ShowWhatComboBox.SelectedIndex = 0;

			ShowWhatComboBox.SelectionChanged += (sender, e) =>
			{
				ModsToShow selectedItem = (ModsToShow)ShowWhatComboBox.SelectedIndex;
				switch (selectedItem)
				{
					case ModsToShow.ShowInstalled:
						InstalledModsListControl.Visibility = Visibility.Visible;
						OnlineModListControl.Visibility = Visibility.Collapsed;
						break;
					case ModsToShow.ShowLatest:
						InstalledModsListControl.Visibility = Visibility.Collapsed;
						OnlineModListControl.Visibility = Visibility.Visible;
						OnlineModListControl.ItemsSource = LatestModList;
						break;
					case ModsToShow.ShowPopular:
						InstalledModsListControl.Visibility = Visibility.Collapsed;
						OnlineModListControl.Visibility = Visibility.Visible;
						OnlineModListControl.ItemsSource = PopularModList;
						break;
				}
			};

			InstalledModsListControl.ItemsSource = InstalledModList;
			OnlineModListControl.ItemsSource = PopularModList;
		}

		private enum ModsToShow
		{
			ShowInstalled,
			ShowLatest,
			ShowPopular
		}

		private bool DependenciesOk() =>
			InstalledModList.All((m) =>
				m.ModJson.Dependencies.All((d) =>
					InstalledModList.Any((mc) => mc.ModJson.Id == d
					)
				)
			);

		private List<Mod> GetMods(ModsToShow modsToShow)
		{
			List<Mod> mods = new List<Mod>();

			if (modsToShow == ModsToShow.ShowInstalled)
			{
				string[] installedMods = Directory.GetDirectories(Properties.Settings.Default.GamePath + "\\QMods", "*", SearchOption.TopDirectoryOnly);

				foreach (string mod in installedMods)
				{
					if (Path.GetFileName(mod) != ".backups")
					{
						mods.Add(new Mod(mod));
					}
				}
			}
			else if (modsToShow == ModsToShow.ShowLatest)
			{
				HttpResponseMessage response = httpClient.GetAsync("mods/latest_added.json").Result;
				List<Mod.DownloadedModData> downloadedModData = JsonSerializer.Deserialize<List<Mod.DownloadedModData>>(response.Content.ReadAsStringAsync().Result);
				foreach (Mod.DownloadedModData item in downloadedModData)
				{
					mods.Add(new Mod(item));
				}
			}
			else if (modsToShow == ModsToShow.ShowPopular)
			{
				HttpResponseMessage response = httpClient.GetAsync("mods/trending.json").Result;
				List<Mod.DownloadedModData> downloadedModData = JsonSerializer.Deserialize<List<Mod.DownloadedModData>>(response.Content.ReadAsStringAsync().Result);
				foreach (Mod.DownloadedModData item in downloadedModData)
				{
					mods.Add(new Mod(item));
				}
			}

			return mods;
		}

		private void ApplyModifsBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (Mod mod in InstalledModList)
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

		private void SearchForLastVerBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach (Mod mod in InstalledModList)
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
	}
}
