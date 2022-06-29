using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SubnauticaModManager.Avalonia
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Backend.Initialize();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void StartGame(object? sender, RoutedEventArgs e)
        {
            Backend.LaunchGame();
        }

        private List<Mod> InstalledMods => ModFetcher.InstalledMods;

        private void OpenSettings(object? sender, RoutedEventArgs e)
        {
            Settings win = new();
            win.ShowDialog(this);
        }
    }
}