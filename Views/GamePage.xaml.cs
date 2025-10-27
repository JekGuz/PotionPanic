using System.Linq;
using Microsoft.Maui.Layouts;
using PotionPanic.Resources;

namespace PotionPanic.Views;

public partial class GamePage : ContentPage
{
    // UI
    Label? _recipeLabel;
    Label? _progressLabel;
    Label? _scoreLabel;
    Image? _cauldron;
    Grid? _fxLayer;

    // Кнопки ингредиентов
    Button? _btnMushroom, _btnCrystal, _btnHerb, _btnFeather, _btnEye, _btnRoot;

    // Канонические КЛЮЧИ (слова), по ним работает логика
    readonly string[] AllIngredients = { "Mushroom", "Crystal", "Herb", "Feather", "Eye", "Root" };

    // Отображение ключа как красивой иконки (для задания/рецепта)
    static string EmojiFor(string key) => key switch
    {
        "Mushroom" => "🍄",
        "Crystal" => "💎",
        "Herb" => "🌿",
        "Feather" => "🕊️",
        "Eye" => "👁️",
        "Root" => "🌱",
        // На всякий случай
        // Если ключ не найден, возвращаем вопросительный знак
        _ => "❓"
    };

    List<string> currentRecipe = new(); // список ключей
    int currentStep = 0;
    const int totalSteps = 3;
    int currentScore = 0;

    readonly Random _rng = new();

    public GamePage()
    {
        InitializeComponent();

        _recipeLabel = this.FindByName<Label>("RecipeLabel");
        _progressLabel = this.FindByName<Label>("ProgressLabel");
        _scoreLabel = this.FindByName<Label>("ScoreLabel");
        _cauldron = this.FindByName<Image>("Cauldron");
        _fxLayer = this.FindByName<Grid>("FXLayer");

        _btnMushroom = this.FindByName<Button>("BtnMushroom");
        _btnCrystal = this.FindByName<Button>("BtnCrystal");
        _btnHerb = this.FindByName<Button>("BtnHerb");
        _btnFeather = this.FindByName<Button>("BtnFeather");
        _btnEye = this.FindByName<Button>("BtnEye");
        _btnRoot = this.FindByName<Button>("BtnRoot");

        GenerateNewRecipe();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyTexts(); // подтянет текущую культуру
    }

    // ===== UI =====
    void ApplyTexts()
    {
        var recipeIcons = string.Join("-", currentRecipe.Select(EmojiFor));
        if (_recipeLabel != null) _recipeLabel.Text = string.Format(AppResources.RecipeFormat, recipeIcons);
        if (_progressLabel != null) _progressLabel.Text = string.Format(AppResources.StepFormat, currentStep, totalSteps);
        if (_scoreLabel != null) _scoreLabel.Text = string.Format(AppResources.ScoreFormat, currentScore);
        if (BtnBack != null) BtnBack.Text = AppResources.BackToMenu;

        // ОБНОВЛЯЕМ КНОПКИ из ресурсов — тогда язык сменится «на лету»
        if (_btnMushroom != null) _btnMushroom.Text = AppResources.Ing_Mushroom;
        if (_btnCrystal != null) _btnCrystal.Text = AppResources.Ing_Crystal;
        if (_btnHerb != null) _btnHerb.Text = AppResources.Ing_Herb;
        if (_btnFeather != null) _btnFeather.Text = AppResources.Ing_Feather;
        if (_btnEye != null) _btnEye.Text = AppResources.Ing_Eye;
        if (_btnRoot != null) _btnRoot.Text = AppResources.Ing_Root;

    }

    // ===== LOGIC =====
    void GenerateNewRecipe()
    {
        // Берём КЛЮЧИ, а не эмодзи
        currentRecipe = AllIngredients.OrderBy(_ => _rng.Next()).Take(totalSteps).ToList();
        currentStep = 0;
        ApplyTexts();
    }

    async void OnIngredient(object sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        // На кнопках в XAML CommandParameter = "Mushroom"/... — то, что нам нужно
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

        ApplyTexts();
    }

    void Back_Clicked(object sender, EventArgs e)
    {
        // Полный сброс состояния, чтобы при старте была «новая игра»
        currentRecipe.Clear();
        currentStep = 0;
        currentScore = 0;

        // Никаких таймеров/эффектов не держим
        _fxLayer?.Children.Clear();

        // Навигация в меню (где можно менять язык)
        Shell.Current.GoToAsync("//menu");
    }

    // ===== FX =====
    // Эффекты для правильного ингредиента
    async Task CorrectFxAsync()
    {
        if (_cauldron is null) return;
        await _cauldron.ScaleTo(1.05, 120);
        await _cauldron.ScaleTo(1.0, 120);
        for (int i = 0; i < 6; i++) _ = SpawnSparkAsync(); // небольшие искры 4 шт сначала решила увеличить до 6
    }

    // Эффекты для успешного завершения рецепта
    async Task SuccessFxAsync()
    {
        if (_cauldron is null) return;
        await _cauldron.RelRotateTo(10, 80);
        await _cauldron.RelRotateTo(-20, 160);
        await _cauldron.RelRotateTo(10, 80);
        for (int i = 0; i < 10; i++) _ = SpawnSparkAsync(true);
    }

    // Эффекты для неправильного ингредиента
    async Task WrongFxAsync(View btn)
    {
        await btn.TranslateTo(-6, 0, 60);
        await btn.TranslateTo(6, 0, 60);
        await btn.TranslateTo(0, 0, 60);
    }

    // Спавн искры, которая поднимается вверх и исчезает
    // big = крупная искра для успеха
    async Task SpawnSparkAsync(bool big = false)
    {
        if (_fxLayer is null || _cauldron is null) return;

        // Если разметка ещё не измерена — подождём кадр
        if (_fxLayer.Width <= 0 || _fxLayer.Height <= 0 || _cauldron.Width <= 0 || _cauldron.Height <= 0)
        {
            await Task.Yield();
            if (_fxLayer.Width <= 0 || _fxLayer.Height <= 0 || _cauldron.Width <= 0 || _cauldron.Height <= 0)
                return;
        }

        // Размеры искр — больше
        double size = big ? _rng.Next(10, 18) : _rng.Next(7, 12);

        var dot = new BoxView
        {
            WidthRequest = size,
            HeightRequest = size,
            Color = Color.FromArgb("#D4AF37"),
            CornerRadius = (float)(size / 2),
            Opacity = 0
        };

        _fxLayer.Add(dot);
        await Task.Yield();

        // Координаты котла (в тех же координатах, что и FXLayer)
        // Предполагаем, что FXLayer и Cauldron имеют одного и того же визуального предка (обычно Grid).
        // В большинстве разметок _cauldron.X/Y достаточно точны для FXLayer-оверлея.
        double cX = _cauldron.X;
        double cY = _cauldron.Y;
        double cW = _cauldron.Width;
        double cH = _cauldron.Height;

        // Область старта: Горловина котла (центральная треть по X, верхняя треть по Y)
        double neckLeft = cX + cW * 0.33;
        double neckRight = cX + cW * 0.67;
        double neckTop = cY + cH * 0.35;  // чуть ниже верха котла
        double neckBot = cY + cH * 0.50;  // середина котла

        double startX = _rng.NextDouble() * (neckRight - neckLeft) + neckLeft;
        double startY = _rng.NextDouble() * (neckBot - neckTop) + neckTop;

        AbsoluteLayout.SetLayoutBounds(dot, new Rect(startX, startY, size, size));
        AbsoluteLayout.SetLayoutFlags(dot, AbsoluteLayoutFlags.None);

        // Траектория — выше и слегка в сторону
        double riseMin = big ? 140 : 110;
        double riseMax = big ? 200 : 160;
        var targetY = startY - _rng.Next((int)riseMin, (int)riseMax);

        double drift = big ? 70 : 50; // горизонтальный разлёт
        var driftX = startX + (_rng.NextDouble() - 0.5) * (2 * drift);

        await dot.FadeTo(1, 90);

        var t1 = dot.TranslateTo(driftX - startX, targetY - startY, (uint)_rng.Next(400, 700), Easing.CubicOut);
        var t2 = dot.FadeTo(0, (uint)_rng.Next(320, 600));
        await Task.WhenAll(t1, t2);

        _fxLayer.Remove(dot);
    }
}
