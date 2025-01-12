using System.IO;
using Microsoft.Maui.Controls;

namespace MauiApp3
{
    public partial class App : Application
    {
        private static GameDatabase _database;

        public static GameDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new GameDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "games.db3"));
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
