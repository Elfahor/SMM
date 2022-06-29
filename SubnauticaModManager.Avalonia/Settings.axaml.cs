using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SubnauticaModManager.Avalonia
{
    public class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            DataContext = SubnauticaModManager.Settings.Default;
            Closing += (sender, args) => SubnauticaModManager.Settings.SaveToFile(); 
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}