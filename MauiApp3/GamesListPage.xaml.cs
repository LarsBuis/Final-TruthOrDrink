using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MauiApp3
{
    public partial class GamesListPage : ContentPage
    {
        public ObservableCollection<GameViewModel> Games { get; set; } = new ObservableCollection<GameViewModel>();

        public GamesListPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ReloadGames();
        }

        private async void ReloadGames()
        {
            // Clear the existing list to avoid duplicates
            Games.Clear();

            // Fetch the games from the database
            var games = await App.Database.GetSavedGamesAsync();

            foreach (var game in games)
            {
                try
                {
                    // Deserialize the DrinksPerPlayer JSON to a dictionary
                    var drinksPerPlayer = JsonConvert.DeserializeObject<Dictionary<string, int>>(game.DrinksPerPlayer);

                    // Format the drinks per player for display
                    string drinksFormatted = string.Join("\n", drinksPerPlayer.Select(d => $"{d.Key}: {d.Value} drinks"));

                    // Add the game to the observable collection
                    Games.Add(new GameViewModel
                    {
                        Date = $"Date: {game.Date}",
                        Players = game.Players,
                        DrinksPerPlayerFormatted = drinksFormatted
                    });
                }
                catch (Exception ex)
                {
                    // Handle any JSON deserialization errors or other exceptions
                    await DisplayAlert("Error", $"Failed to load game: {ex.Message}", "OK");
                }
            }
        }
    }

    // View model for displaying games
    public class GameViewModel
    {
        public string Date { get; set; }
        public string Players { get; set; }
        public string DrinksPerPlayerFormatted { get; set; }
    }
}
