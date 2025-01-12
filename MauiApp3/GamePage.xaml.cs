using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MauiApp3
{
    public partial class GamePage : ContentPage
    {
        private readonly ObservableCollection<string> _players;
        private readonly string _rating;
        private int _currentPlayerIndex = 0;

        public GamePage(ObservableCollection<string> players, string rating)
        {
            InitializeComponent();
            _players = players;
            _rating = rating;
            LoadQuestion();
        }

        private async void LoadQuestion()
        {
            string url = $"https://api.truthordarebot.xyz/v1/truth?rating={_rating}";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var question = JObject.Parse(jsonResponse)["question"]?.ToString();
                    QuestionLabel.Text = question ?? "No question found.";
                }
                else
                {
                    QuestionLabel.Text = "Failed to fetch question.";
                }
            }

            CurrentPlayerLabel.Text = $"{_players[_currentPlayerIndex]}'s turn!";
        }

        private void TruthButton_Clicked(object sender, EventArgs e)
        {
            NextPlayer();
            LoadQuestion();
        }

        private void DrinkButton_Clicked(object sender, EventArgs e)
        {
            NextPlayer();
            LoadQuestion();
        }

        private void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }
    }
}