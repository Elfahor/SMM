using System.Diagnostics;
using System.Windows;

namespace SubnauticaModManager
{
	/// <summary>
	/// Logique d'interaction pour NexusApiInfo.xaml
	/// </summary>
	public partial class NexusApiInfo : Window
	{
		public NexusApiInfo()
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
