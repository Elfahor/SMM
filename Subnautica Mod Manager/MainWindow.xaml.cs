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
		//private List<Mod> InstalledModList;
		//private List<Mod> LatestModList;
		//private List<Mod> PopularModList;

		private ModFetcher.ModsToShow modsShown;

		private UserData UserData { get; }

		public MainWindow()
		{
			InitializeComponent();

			Settings.LoadFromFile();
			NexusAPIProvider.UpdateRequestHeaders();
			ModFetcher.InitializeModsLists();

			OpenSettingsBtn.Click += OpenSettingsBtn_Click;
			ApplyModifsBtn.Click += (sender, e) => ModFetcher.ApplyJsonModifsToLocals();
			//SearchForLastVerBtn.Click += SearchForLastVerBtn_Click;
			StartGameBtn.Click += StartGameBtn_Click;

			try
			{
				UserData = UserData.GetOnline();
			}
			catch (AggregateException)
			{
				UserData = default;
				Console.WriteLine("couldn't initialize userdata");
			}

			string[] listViewModes = Enum.GetNames(typeof(ModFetcher.ModsToShow));
			ShowWhatComboBox.ItemsSource = listViewModes;
			ShowWhatComboBox.SelectedIndex = 0;

			modsShown = ModFetcher.ModsToShow.ShowInstalled;

			ShowWhatComboBox.SelectionChanged += (sender, e) =>
			{
				ModFetcher.ModsToShow selectedItem = (ModFetcher.ModsToShow)ShowWhatComboBox.SelectedIndex;
				modsShown = selectedItem;
				switch (selectedItem)
				{
					case ModFetcher.ModsToShow.ShowInstalled:
						InstalledModListControl.Visibility = Visibility.Visible;
						OnlineModListControl.Visibility = Visibility.Collapsed;
						InstalledModListControl.Items.Refresh();
						break;
					case ModFetcher.ModsToShow.ShowLatest:
						InstalledModListControl.Visibility = Visibility.Collapsed;
						OnlineModListControl.Visibility = Visibility.Visible;

						OnlineModListControl.ItemsSource = ModFetcher.LatestMods;
						OnlineModListControl.Items.Refresh();
						break;
					case ModFetcher.ModsToShow.ShowPopular:
						InstalledModListControl.Visibility = Visibility.Collapsed;
						OnlineModListControl.Visibility = Visibility.Visible;

						OnlineModListControl.ItemsSource = ModFetcher.PopularMods;
						OnlineModListControl.Items.Refresh();
						break;
				}
			};

			InstalledModListControl.ItemsSource = ModFetcher.InstalledMods;
			OnlineModListControl.ItemsSource = ModFetcher.PopularMods;

			CollectionView viewInstalled = (CollectionView)CollectionViewSource.GetDefaultView(InstalledModListControl.ItemsSource);
			if (!(viewInstalled is null))
				viewInstalled.Filter = FilterByModName;
			CollectionView viewOnline = (CollectionView)CollectionViewSource.GetDefaultView(OnlineModListControl.ItemsSource);
			if (!(viewOnline is null))
			viewOnline.Filter = FilterByModName;
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
			foreach (Mod mod in ModFetcher.InstalledMods)
			{
				int modId = int.Parse(mod.ModJson.NexusId.Subnautica);
			}
		}

		private void StartGameBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!ModFetcher.DependenciesOk())
			{
				MessageBox.Show("Some mods have dependencies issues", "Dependency check", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.None);
				return;
			}
			using Process game = new Process();
			game.StartInfo.FileName = Path.Combine(Settings.Default.GamePath, "Subnautica.exe");
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
					ModFetcher.InitializeLocalMods();
					InstalledModListControl.Items.Refresh();
				};
				dlpage.ShowDialog();
			}
		}

		private bool FilterByModName(object item)
		{
			if (string.IsNullOrEmpty(SearchArea.Text))
			{
				return true;
			}
			else
			{
				string toSearch = modsShown == ModFetcher.ModsToShow.ShowInstalled ? (item as Mod).ModJson.DisplayName : (item as Mod).OnlineInfo.Name;
				return !string.IsNullOrEmpty(toSearch) && toSearch.IndexOf(SearchArea.Text, StringComparison.OrdinalIgnoreCase) >= 0;
			}
		}

		private void SearchArea_TextChanged(object sender, TextChangedEventArgs e)
		{
			ListView listView = modsShown == ModFetcher.ModsToShow.ShowInstalled ? InstalledModListControl : OnlineModListControl;
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
