namespace PotionPanic.Views;

public partial class GamePage : ContentPage
{
    enum Ingredient { Mushroom, Crystal, Herb, Feather, Eye, Root }

    readonly Random _rng = new();
    List<Ingredient> _recipe = new();
    int _index = 0;
    int _score = 0;

    public GamePage()
    {
        InitializeComponent();
        NewRecipe();
    }

    void NewRecipe()
    {
        // длина рецепта растёт с очками
        var len = Math.Clamp(3 + _score / 50, 3, 10);
        _recipe = Enumerable.Range(0, len)
            .Select(_ => (Ingredient)_rng.Next(0, 6))
            .ToList();
        _index = 0;

        RecipeLabel.Text = $"Recipe: {string.Join("-", _recipe)}";
        ProgressLabel.Text = $"Step: {_index}/{_recipe.Count}";
        ScoreLabel.Text = $"Score: {_score}";
    }

    async void OnIngredient(object sender, EventArgs e)
    {
        if (sender is not Button b) return;
        if (!Enum.TryParse<Ingredient>(b.Text, out var ing)) return;

        var expected = _recipe[_index];
        if (ing == expected)
        {
            _index++;
            _score += 10;
            ProgressLabel.Text = $"Step: {_index}/{_recipe.Count}";
            ScoreLabel.Text = $"Score: {_score}";

            if (_index >= _recipe.Count)
            {
                await DisplayAlert("Potion", "Perfect! New recipe.", "OK");
                NewRecipe();
            }
        }
        else
        {
            await DisplayAlert("Boom!", "Wrong ingredient. Try again.", "OK");
            _score = 0;
            NewRecipe();
        }
    }

    async void Back_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//menu");
}
