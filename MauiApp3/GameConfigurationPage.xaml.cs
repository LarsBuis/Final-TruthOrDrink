using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace MauiApp3
{
    public partial class GameConfigurationPage : ContentPage
    {
        public ObservableCollection<string> Players { get; set; } = new ObservableCollection<string>();

        public GameConfigurationPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void AddPlayer_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayerEntry.Text))
            {
                Players.Add(PlayerEntry.Text.Trim());
                PlayerEntry.Text = string.Empty;
            }
        }

        private async void StartGame_Clicked(object sender, EventArgs e)
        {
            if (Players.Count == 0)
            {
                await DisplayAlert("Error", "Please add at least one player.", "OK");
                return;
            }

            if (RatingPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please select a rating.", "OK");
                return;
            }

            if (!int.TryParse(QuestionEntry.Text, out int totalQuestions) || totalQuestions <= 0)
            {
                await DisplayAlert("Error", "Please enter a valid number of questions.", "OK");
                return;
            }

            string rating = RatingPicker.SelectedItem.ToString();
            bool useCustomQuestions = CustomQuestionsSwitch.IsToggled;

            // Navigate to the GamePage and pass the toggle state
            await Navigation.PushAsync(new GamePage(Players, rating, totalQuestions, useCustomQuestions));
        }
    }
}
