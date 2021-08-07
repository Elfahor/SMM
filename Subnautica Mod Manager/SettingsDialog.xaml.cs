using System.Windows;

namespace SubnauticaModManager.Wpf
{
	/// <summary>
	/// Logique d'interaction pour SettingsDialog.xaml
	/// </summary>
	public partial class SettingsDialog : Window
	{
		public SettingsDialog()
		{
			InitializeComponent();

			Settings.LoadFromFile();

			GamePathTextBox.Text = Settings.Default.GamePath;
			NexusAPIKeyBox.Password = Settings.Default.NexusApiKey;
			SaveApiKeyCheckBox.IsChecked = Settings.Default.SaveApiKey;

			Closed += (sender, e) =>
			{
				Settings.Default.GamePath = GamePathTextBox.Text;
				Settings.Default.SaveApiKey = (bool)SaveApiKeyCheckBox.IsChecked;
				if (Settings.Default.SaveApiKey)
				{
					Settings.Default.NexusApiKey = NexusAPIKeyBox.Password;
				}
				Settings.SaveToFile();
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
