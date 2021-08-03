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

			NexusMods.Settings.LoadFromFile();

			GamePathTextBox.Text = NexusMods.Settings.Default.GamePath;
			NexusAPIKeyBox.Password = NexusMods.Settings.Default.NexusApiKey;
			SaveApiKeyCheckBox.IsChecked = NexusMods.Settings.Default.SaveApiKey;

			Closed += (sender, e) =>
			{
				NexusMods.Settings.Default.GamePath = GamePathTextBox.Text;
				NexusMods.Settings.Default.SaveApiKey = (bool)SaveApiKeyCheckBox.IsChecked;
				if (NexusMods.Settings.Default.SaveApiKey)
				{
					NexusMods.Settings.Default.NexusApiKey = NexusAPIKeyBox.Password;
				}
				NexusMods.Settings.SaveToFile();
				NexusMods.NexusAPIProvider.UpdateRequestHeaders();
			};

			OpenNexusApiInfoBtn.Click += OpenNexusApiInfoBtn_Click;
		}

		private void OpenNexusApiInfoBtn_Click(object sender, RoutedEventArgs e)
		{
			NexusApiInfo info = new NexusApiInfo()
			{
				Owner = this,
				DataContext = this.DataContext
			};

			info.ShowDialog();
		}
	}
}
