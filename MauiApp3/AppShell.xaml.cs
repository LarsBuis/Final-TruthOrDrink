using Microsoft.Maui.Controls;

namespace MauiApp3
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(GameConfigurationPage), typeof(GameConfigurationPage));
            Routing.RegisterRoute(nameof(GamesListPage), typeof(GamesListPage));
        }
    }
}