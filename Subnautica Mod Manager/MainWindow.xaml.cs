using SubnauticaModManager.NexusApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SubnauticaModManager.Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<Mod> InstalledModList;
		private List<Mod> LatestModList;
		private List<Mod> PopularModList;

		private ModsToShow modsShown;

		private UserData UserData { get; }

		public MainWindow()
		{
			InitializeComponent();

			Settings.LoadFromFile();
			NexusAPIProvider.UpdateRequestHeaders();

			OpenSettingsBtn.Click += OpenSettingsBtn_Click;
			ApplyModifsBtn.Click += ApplyModifsBtn_Click;
			//SearchForLastVerBtn.Click += SearchForLastVerBtn_Click;
			StartGameBtn.Click += StartGameBtn_Click;

			InstalledModList = GetMods(ModsToShow.ShowInstalled);			
			LatestModList = GetMods(ModsToShow.ShowLatest);
			PopularModList = GetMods(ModsToShow.ShowPopular);

			try
			{
				UserData = UserData.GetOnline();
			}
			catch (AggregateException)
			{
				UserData = default;
				Console.WriteLine("couldn't initialize userdata");
			}

			string[] listViewModes = Enum.GetNames(typeof(ModsToShow));
			ShowWhatComboBox.ItemsSource = listViewModes;
			ShowWhatComboBox.SelectedIndex = 0;

			modsShown = ModsToShow.ShowInstalled;

			ShowWhatComboBox.SelectionChanged += (sender, e) =>
			{
				ModsToShow selectedItem = (ModsToShow)ShowWhatComboBox.SelectedIndex;
				modsShown = selectedItem;
				switch (selectedItem)
				{
					case ModsToShow.ShowInstalled:
						InstalledModListControl.Visibility = Visibility.Visible;
						OnlineModListControl.Visibility = Visibility.Collapsed;
						InstalledModListControl.Items.Refresh();
						break;
					case ModsToShow.ShowLatest:
						InstalledModListControl.Visibility = Visibility.Collapsed;
						OnlineModListControl.Visibility = Visibility.Visible;

						OnlineModListControl.ItemsSource = LatestModList;
						OnlineModListControl.Items.Refresh();
						break;
					case ModsToShow.ShowPopular:
						InstalledModListControl.Visibility = Visibility.Collapsed;
						OnlineModListControl.Visibility = Visibility.Visible;

						OnlineModListControl.ItemsSource = PopularModList;
						OnlineModListControl.Items.Refresh();
						break;
				}
			};

			InstalledModListControl.ItemsSource = InstalledModList;
			OnlineModListControl.ItemsSource = PopularModList;

			CollectionView viewInstalled = (CollectionView)CollectionViewSource.GetDefaultView(InstalledModListControl.ItemsSource);
			if (!(viewInstalled is null))
				viewInstalled.Filter = FilterByModName;
			CollectionView viewOnline = (CollectionView)CollectionViewSource.GetDefaultView(OnlineModListControl.ItemsSource);
			if (!(viewOnline is null))
			viewOnline.Filter = FilterByModName;
		}

		public enum ModsToShow
		{
			ShowInstalled,
			ShowLatest,
			ShowPopular
		}

		private bool DependenciesOk() =>
			InstalledModList.All(m =>
				m.ModJson.Dependencies.All(d =>
					InstalledModList.Any(mc => mc.ModJson.Id == d
					)
				)
			);

		[Obsolete("move to backend")]
		public List<Mod> GetMods(ModsToShow modsToShow)
		{
			List<Mod> mods = new List<Mod>();

			if (modsToShow == ModsToShow.ShowInstalled)
			{
				string[] installedMods = Directory.GetDirectories(Settings.Default.GamePath + "\\QMods", "*", SearchOption.TopDirectoryOnly);

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
				try
				{
					string response = NexusAPIProvider.NexusHttpClient.GetStringAsync("mods/latest_added.json").Result;
					List<Mod.DownloadedModData> downloadedModData = JsonSerializer.Deserialize<List<Mod.DownloadedModData>>(response);
					foreach (Mod.DownloadedModData item in downloadedModData)
					{
						mods.Add(new Mod(item, NexusAPIProvider.NexusHttpClient));
					}
				}
				catch (AggregateException e)
				{
					if (e.InnerException is HttpRequestException)
					{
						return null;
					}
				}
			}
			else if (modsToShow == ModsToShow.ShowPopular)
			{
				try
				{
					string response = NexusAPIProvider.NexusHttpClient.GetStringAsync("mods/trending.json").Result;
					List<Mod.DownloadedModData> downloadedModData = JsonSerializer.Deserialize<List<Mod.DownloadedModData>>(response);
					foreach (Mod.DownloadedModData item in downloadedModData)
					{
						mods.Add(new Mod(item, NexusAPIProvider.NexusHttpClient));
					}
				}
				catch (AggregateException e)
				{
					if (e.InnerException is HttpRequestException)
					{
						return null;
					}
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

		// move to backend
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
			game.StartInfo.FileName = Path.Combine(NexusApi.Settings.Default.GamePath, "Subnautica.exe");
			game.Start();
		}

		private void DownloadBtn_Click(object sender, RoutedEventArgs e)
		{
			Mod modSelected = (Mod)((Button)sender).DataContext;
			StartDownloadOfMod(modSelected);
		}

		private void StartDownloadOfMod(Mod mod)
		{
			if (UserData.IsPremium)
			{
				//handle premium mod download
			}
			else
			{
				DownloadPage dlpage = new DownloadPage(mod)
				{
					Owner = this,
				};
				dlpage.Closed += (sender, e) =>
				{
					InstalledModList = GetMods(ModsToShow.ShowInstalled);
					InstalledModListControl.Items.Refresh();
				};
				dlpage.ShowDialog();
			}
		}

		private bool FilterByModName(object item)
		{
			if (String.IsNullOrEmpty(SearchArea.Text))
			{
				return true;
			}
			else
			{
				string toSearch = modsShown == ModsToShow.ShowInstalled ? (item as Mod).ModJson.DisplayName : (item as Mod).OnlineInfo.name;
				return toSearch.IndexOf(SearchArea.Text, StringComparison.OrdinalIgnoreCase) >= 0;
			}
		}

		private void SearchArea_TextChanged(object sender, TextChangedEventArgs e)
		{
			ListView listView = modsShown == ModsToShow.ShowInstalled ? InstalledModListControl : OnlineModListControl;
			CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
		}

		private void GetThisModBtn_Click(object sender, RoutedEventArgs e)
		{
			string modOnlineInfo = NexusAPIProvider.NexusHttpClient.GetStringAsync($"mods/{SearchArea.Text}.json").Result;
			Mod mod = new Mod(JsonSerializer.Deserialize<Mod.DownloadedModData>(modOnlineInfo), NexusAPIProvider.NexusHttpClient);
			StartDownloadOfMod(mod);
		}
	}
}
