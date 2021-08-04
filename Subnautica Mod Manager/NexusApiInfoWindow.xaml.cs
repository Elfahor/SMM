using System.Diagnostics;
using System.Windows;

namespace SubnauticaModManager.Wpf
{
	/// <summary>
	/// Logique d'interaction pour NexusApiInfo.xaml
	/// </summary>
	public partial class NexusApiInfoWindow : Window
	{
		public NexusApiInfoWindow()
		{
			InitializeComponent();

			CloseWinBtn.Click += (sender, e) => Close();
		}

		private void HyperlinkGetApiKey_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
			e.Handled = true;
		}
	}
}
