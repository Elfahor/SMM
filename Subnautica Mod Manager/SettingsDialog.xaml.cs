using SubnauticaModManager.Wpf;
using System.Windows;

namespace SubnauticaModManager
{
	/// <summary>
	/// Logique d'interaction pour SettingsDialog.xaml
	/// </summary>
	public partial class SettingsDialog : Window
	{
		public SettingsDialog()
		{
			InitializeComponent();

			NexusApi.Settings.LoadFromFile();

			GamePathTextBox.Text = NexusApi.Settings.Default.GamePath;
			NexusAPIKeyBox.Password = NexusApi.Settings.Default.NexusApiKey;
			SaveApiKeyCheckBox.IsChecked = NexusApi.Settings.Default.SaveApiKey;

			Closed += (sender, e) =>
			{
				NexusApi.Settings.Default.GamePath = GamePathTextBox.Text;
				NexusApi.Settings.Default.SaveApiKey = (bool)SaveApiKeyCheckBox.IsChecked;
				if (NexusApi.Settings.Default.SaveApiKey)
				{
					NexusApi.Settings.Default.NexusApiKey = NexusAPIKeyBox.Password;
				}
				NexusApi.Settings.SaveToFile();
				NexusApi.NexusAPIProvider.UpdateRequestHeaders();
			};

			OpenNexusApiInfoBtn.Click += OpenNexusApiInfoBtn_Click;
		}

		private void OpenNexusApiInfoBtn_Click(object sender, RoutedEventArgs e)
		{
			NexusApiInfoWindow info = new NexusApiInfoWindow()
			{
				Owner = this,
				DataContext = this.DataContext
			};

			info.ShowDialog();
		}
	}
}
