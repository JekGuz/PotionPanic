using System.Globalization;
using PotionPanic.Resources;

namespace PotionPanic.Views;

public partial class GamePage : ContentPage
{
    // ������� ��������� ���� (�������� ���� ��������/�������� �� �� ������ ����)
    string currentRecipeString = "Crystal-Mushroom-Feather";
    int currentStep = 0;
    int totalSteps = 3;
    int currentScore = 0;

    public GamePage()
    {
        InitializeComponent();

        // ��������� ��������� ������� �� ������� ��������
        UpdateUiTexts(currentRecipeString, currentStep, totalSteps, currentScore);
    }

    // ���������� ��� �������
    void UpdateUiTexts(string recipe, int step, int total, int score)
    {
        RecipeLabel.Text = string.Format(AppResources.RecipeFormat, recipe);
        ProgressLabel.Text = string.Format(AppResources.StepFormat, step, total);
        ScoreLabel.Text = string.Format(AppResources.ScoreFormat, score);
    }

    // ���������� �������� + �������������� ���������� �������
    void ApplyCulture(string cultureCode)
    {
        var ci = new CultureInfo(cultureCode);
        CultureInfo.CurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        AppResources.Culture = ci; // ����� ��� .resx

        // �������������� ������
        UpdateUiTexts(currentRecipeString, currentStep, totalSteps, currentScore);
    }

    // ����������� ������ �����
    void LangEn_Clicked(object sender, EventArgs e) => ApplyCulture("en");
    void LangRu_Clicked(object sender, EventArgs e) => ApplyCulture("ru");
    void LangEt_Clicked(object sender, EventArgs e) => ApplyCulture("et");

    // ����� �������� �������� � ���� � �� ������ �������:
    void OnIngredient(object sender, EventArgs e)
    {
        // ... ���� ������
        // ������ ���������, ��������:
        // currentStep++;
        // currentScore += 10;
        UpdateUiTexts(currentRecipeString, currentStep, totalSteps, currentScore);
    }

    void Back_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//menu");
    }
}