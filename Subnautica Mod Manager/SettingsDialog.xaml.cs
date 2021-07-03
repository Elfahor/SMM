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

			Properties.Settings.Default.Reload();
			GamePathTextBox.Text = Properties.Settings.Default.GamePath;
			NexusAPIKeyBox.Password = Properties.Settings.Default.NexusApiKey;
			SaveApiKeyCheckBox.IsChecked = Properties.Settings.Default.SaveApiKey;

			Closed += (sender, e) =>
			{
				Properties.Settings.Default.GamePath = GamePathTextBox.Text;
				Properties.Settings.Default.SaveApiKey = (bool)SaveApiKeyCheckBox.IsChecked;
				if (Properties.Settings.Default.SaveApiKey)
				{
					Properties.Settings.Default.NexusApiKey = NexusAPIKeyBox.Password;
				}
				Properties.Settings.Default.Save();
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
