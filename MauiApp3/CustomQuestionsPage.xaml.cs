using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace MauiApp3
{
    public partial class CustomQuestionsPage : ContentPage
    {
        public ObservableCollection<CustomQuestion> CustomQuestions { get; set; } = new ObservableCollection<CustomQuestion>();

        public CustomQuestionsPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadCustomQuestions();
        }

        // Load custom questions from the database
        private async void LoadCustomQuestions()
        {
            var questions = await App.Database.GetCustomQuestionsAsync();
            foreach (var question in questions)
            {
                CustomQuestions.Add(question);
            }
        }

        // Add a new custom question
        private async void AddQuestion_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(QuestionEntry.Text))
            {
                var newQuestion = new CustomQuestion
                {
                    QuestionText = QuestionEntry.Text.Trim()
                };

                // Save to the database
                await App.Database.SaveCustomQuestionAsync(newQuestion);

                // Add to the observable collection
                CustomQuestions.Add(newQuestion);

                // Clear the input field
                QuestionEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid question.", "OK");
            }
        }
    }
}
