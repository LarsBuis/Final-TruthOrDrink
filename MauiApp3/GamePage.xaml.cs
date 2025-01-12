using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MauiApp3
{
    public partial class GamePage : ContentPage
    {
        private readonly ObservableCollection<string> _players;
        private readonly string _rating;
        private readonly int _totalQuestions;
        private int _questionsAsked = 0;
        private int _currentPlayerIndex = 0;
        private readonly Dictionary<string, int> _drinkCounts;
        private readonly bool _useCustomQuestions;
        private List<CustomQuestion> _remainingCustomQuestions;

        public GamePage(ObservableCollection<string> players, string rating, int totalQuestions, bool useCustomQuestions)
        {
            InitializeComponent();
            _players = players;
            _rating = rating;
            _totalQuestions = totalQuestions;
            _drinkCounts = _players.ToDictionary(player => player, player => 0); // Initialize drink counts
            _useCustomQuestions = useCustomQuestions;
            LoadQuestion();
            
        }

        private async void LoadQuestion()
        {
            if (_questionsAsked >= _totalQuestions)
            {
                await DisplayAlert("Game Over", "You've reached the end of the game!", "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            string question;

            if (_useCustomQuestions)
            {
                // Load custom questions from the database if they haven't been loaded yet
                if (_remainingCustomQuestions == null)
                {
                    _remainingCustomQuestions = await App.Database.GetCustomQuestionsAsync();
                }

                if (_remainingCustomQuestions.Count > 0)
                {
                    // Select a random question and remove it from the list to prevent repeats
                    var random = new Random();
                    var randomIndex = random.Next(_remainingCustomQuestions.Count);
                    var randomQuestion = _remainingCustomQuestions[randomIndex];

                    question = randomQuestion.QuestionText;

                    // Remove the selected question to avoid repeating it
                    _remainingCustomQuestions.RemoveAt(randomIndex);
                }
                else
                {
                    await DisplayGameSummary();
                    return;
                }
            }
            else
            {
                // Load a normal question from the API
                string url = $"https://api.truthordarebot.xyz/v1/truth?rating={_rating}";
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        question = JObject.Parse(jsonResponse)["question"]?.ToString() ?? "No question found.";
                    }
                    else
                    {
                        question = "Failed to fetch question.";
                    }
                }
            }

            QuestionLabel.Text = question;
            CurrentPlayerLabel.Text = $"{_players[_currentPlayerIndex]}'s turn!";
            _questionsAsked++;
        }

        private void TruthButton_Clicked(object sender, EventArgs e)
        {
            NextPlayer();
            LoadQuestion();
        }

        private void DrinkButton_Clicked(object sender, EventArgs e)
        {
            string currentPlayer = _players[_currentPlayerIndex];
            _drinkCounts[currentPlayer]++;
            NextPlayer();
            LoadQuestion();
        }

        private void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }

        private async Task DisplayGameSummary()
        {
            string summary = "Game Over! Here are the drink counts:\n\n";
            foreach (var player in _drinkCounts)
            {
                summary += $"{player.Key}: {player.Value} drinks\n";
            }

            await DisplayAlert("Game Summary", summary, "OK");

            // Convert drink counts to JSON
            string drinksJson = JsonConvert.SerializeObject(_drinkCounts);

            // Create a new Game object and save it to the database
            var game = new Game
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Players = string.Join(", ", _players),
                DrinksPerPlayer = drinksJson
            };

            await App.Database.SaveGameAsync(game);

            await Navigation.PopToRootAsync();
        }
    }
}