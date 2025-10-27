using Microsoft.Maui.Layouts;
using PotionPanic.Resources;

namespace PotionPanic.Views;

public partial class GamePage : ContentPage
{
    // ссылки на элементы (ищем их после InitializeComponent)
    Image? _cauldron;
    Grid? _fxLayer;

    // Канонические имена ингредиентов
    readonly string[] AllIngredients = new[] { "Mushroom", "Crystal", "Herb", "Feather", "Eye", "Root" };

    List<string> currentRecipe = new();
    int currentStep = 0;
    int totalSteps = 3;
    int currentScore = 0;

    readonly Random _rng = new();

    public GamePage()
    {
        InitializeComponent();

        // найдём элементы по x:Name — это безопаснее, чем полагаться на автогенерацию полей
        _cauldron = this.FindByName<Image>("Cauldron");
        _fxLayer = this.FindByName<Grid>("FXLayer");

        GenerateNewRecipe();
        UpdateUiTexts();
    }

    void GenerateNewRecipe()
    {
        currentRecipe = AllIngredients.OrderBy(_ => _rng.Next()).Take(3).ToList();
        currentStep = 0;

        var recipeStr = string.Join("-", currentRecipe);
        RecipeLabel.Text = string.Format(AppResources.RecipeFormat, recipeStr);
        UpdateUiTexts();
    }

    void UpdateUiTexts()
    {
        ProgressLabel.Text = string.Format(AppResources.StepFormat, currentStep, totalSteps);
        ScoreLabel.Text = string.Format(AppResources.ScoreFormat, currentScore);
    }

    async void OnIngredient(object sender, EventArgs e)
    {
        if (sender is not Button btn) return;
        var key = btn.CommandParameter as string;
        if (string.IsNullOrEmpty(key)) return;

        if (currentStep < currentRecipe.Count && key == currentRecipe[currentStep])
        {
            currentStep++;
            currentScore += 10;
            await CorrectFxAsync();

            if (currentStep >= totalSteps)
            {
                currentScore += 20;
                await SuccessFxAsync();
                GenerateNewRecipe();
                return;
            }
        }
        else
        {
            await WrongFxAsync(btn);
        }

        UpdateUiTexts();
    }

    void Back_Clicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//menu");

    // 

    async Task CorrectFxAsync()
    {
        if (_cauldron is null) return;
        var up = _cauldron.ScaleTo(1.05, 120);
        await up;
        await _cauldron.ScaleTo(1.0, 120);

        for (int i = 0; i < 4; i++)
            _ = SpawnSparkAsync();
    }

    async Task SuccessFxAsync()
    {
        if (_cauldron is null) return;
        await _cauldron.RelRotateTo(10, 80);
        await _cauldron.RelRotateTo(-20, 160);
        await _cauldron.RelRotateTo(10, 80);
        for (int i = 0; i < 10; i++)
            _ = SpawnSparkAsync(true);
    }

    async Task WrongFxAsync(View btn)
    {
        await btn.TranslateTo(-6, 0, 60);
        await btn.TranslateTo(6, 0, 60);
        await btn.TranslateTo(0, 0, 60);
    }

    async Task SpawnSparkAsync(bool big = false)
    {
        if (_fxLayer is null || _cauldron is null) return;

        double size = big ? _rng.Next(6, 12) : _rng.Next(4, 8);
        var color = Color.FromArgb("#D4AF37");

        var dot = new BoxView
        {
            WidthRequest = size,
            HeightRequest = size,
            Color = color,
            CornerRadius = (float)(size / 2),
            Opacity = 0
        };

        _fxLayer.Add(dot);

        // дождёмся раскладки, чтобы знать размеры
        await Task.Yield();
        var parentW = _fxLayer.Width;
        var parentH = _fxLayer.Height;

        // центр котла и базовая Y-точка
        var cauldronCenterX = parentW * 0.5;
        var baseY = parentH - _cauldron.Height - 16;

        // стартовая позиция искры
        double startX = cauldronCenterX - 20 + _rng.NextDouble() * 40;
        double startY = baseY + 30 + _rng.NextDouble() * 10;

        AbsoluteLayout.SetLayoutBounds(dot, new Rect(startX, startY, size, size));
        AbsoluteLayout.SetLayoutFlags(dot, AbsoluteLayoutFlags.None);

        var targetY = startY - (big ? _rng.Next(90, 140) : _rng.Next(60, 100));
        var driftX = startX + (_rng.NextDouble() - 0.5) * 40;

        await dot.FadeTo(1, 80);
        var t1 = dot.TranslateTo(driftX - startX, targetY - startY, (uint)_rng.Next(350, 600), Easing.CubicOut);
        var t2 = dot.FadeTo(0, (uint)_rng.Next(300, 550));
        await Task.WhenAll(t1, t2);

        _fxLayer.Remove(dot);
    }
}
