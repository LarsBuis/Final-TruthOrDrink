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
                await DisplayGameSummary();
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

        private void OnSwipeLeft(object sender, SwipedEventArgs e)
        {
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
            catch (FeatureNotSupportedException)
            {
                Console.WriteLine("Vibration not supported on this device.");
            }

            TruthButton_Clicked(sender, e);
        }

        private void OnSwipeRight(object sender, SwipedEventArgs e)
        {
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
            catch (FeatureNotSupportedException)
            {
                Console.WriteLine("Vibration not supported on this device.");
            }

            DrinkButton_Clicked(sender, e);
        }


        private void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }

        private async Task DisplayGameSummary()
        {
            var game = new Game
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Players = string.Join(", ", _players),
                DrinksPerPlayer = JsonConvert.SerializeObject(_drinkCounts)
            };

            int rowsAffected = await App.Database.SaveGameAsync(game);
            bool isSaved = rowsAffected > 0;

            if (isSaved)
            {
                Console.WriteLine("Game saved successfully!");
            }
            else
            {
                Console.WriteLine("Game failed to save!");
            }

            // Navigate back to the root
            await Navigation.PopToRootAsync();
        }

    }
}