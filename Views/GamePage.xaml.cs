using System.Globalization;
using PotionPanic.Resources;

namespace PotionPanic.Views;

public partial class GamePage : ContentPage
{
    // Текущее состояние игры (подставь свои значения/обновляй их из логики игры)
    string currentRecipeString = "Crystal-Mushroom-Feather";
    int currentStep = 0;
    int totalSteps = 3;
    int currentScore = 0;

    public GamePage()
    {
        InitializeComponent();

        // начальная установка текстов по текущей культуре
        UpdateUiTexts(currentRecipeString, currentStep, totalSteps, currentScore);
    }

    // Обновление трёх лейблов
    void UpdateUiTexts(string recipe, int step, int total, int score)
    {
        RecipeLabel.Text = string.Format(AppResources.RecipeFormat, recipe);
        ProgressLabel.Text = string.Format(AppResources.StepFormat, step, total);
        ScoreLabel.Text = string.Format(AppResources.ScoreFormat, score);
    }

    // Применение культуры + принудительное обновление лейблов
    void ApplyCulture(string cultureCode)
    {
        var ci = new CultureInfo(cultureCode);
        CultureInfo.CurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        AppResources.Culture = ci; // важно для .resx

        // переотрисовать тексты
        UpdateUiTexts(currentRecipeString, currentStep, totalSteps, currentScore);
    }

    // Обработчики кнопок языка
    void LangEn_Clicked(object sender, EventArgs e) => ApplyCulture("en");
    void LangRu_Clicked(object sender, EventArgs e) => ApplyCulture("ru");
    void LangEt_Clicked(object sender, EventArgs e) => ApplyCulture("et");

    // Когда меняется прогресс в игре — не забудь вызвать:
    void OnIngredient(object sender, EventArgs e)
    {
        // ... твоя логика
        // обнови состояние, например:
        // currentStep++;
        // currentScore += 10;
        UpdateUiTexts(currentRecipeString, currentStep, totalSteps, currentScore);
    }

    void Back_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//menu");
    }
}