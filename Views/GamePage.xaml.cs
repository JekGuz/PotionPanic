using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui; // Rect
using Microsoft.Maui.ApplicationModel; // MainThread
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics; // Color.FromArgb, Rect
using Microsoft.Maui.Layouts;
using PotionPanic.Models; // GameResult
using PotionPanic.Resources;
using PotionPanic.Services; // GameSessionService, IResultsRepository

namespace PotionPanic.Views
{
    public partial class GamePage : ContentPage
    {
        // UI
        Label? _recipeLabel, _progressLabel, _scoreLabel;
        Image? _cauldron;
        Grid? _fxLayer;

        Button? _btnMushroom, _btnCrystal, _btnHerb, _btnFeather, _btnEye, _btnRoot;

        readonly string[] AllIngredients = { "Mushroom", "Crystal", "Herb", "Feather", "Eye", "Root" };
        static string EmojiFor(string key) => key switch
        {
            "Mushroom" => "🍄",
            "Crystal" => "💎",
            "Herb" => "🌿",
            "Feather" => "🕊️",
            "Eye" => "👁️",
            "Root" => "🌱",
            _ => "❓"
        };

        List<string> currentRecipe = new();
        int currentStep = 0;
        const int totalSteps = 3;
        int currentScore = 0;

        readonly Random _rng = new();

        // Сервисы и буфер несохранённых результатов
        // Таким образом результаты каждой игры сохраняются в SQLite и потом отображаются в ResultsPage.
        readonly IResultsRepository _repo;
        readonly GameSessionService _session;
        readonly List<GameResult> _pending = new();
        DateTime _runStartUtc = DateTime.UtcNow;

        public GamePage()
        {
            InitializeComponent();

            _recipeLabel = (Label)FindByName("RecipeLabel");
            _progressLabel = (Label)FindByName("ProgressLabel");
            _scoreLabel = (Label)FindByName("ScoreLabel");
            _cauldron = (Image)FindByName("Cauldron");
            _fxLayer = (Grid)FindByName("FXLayer");

            _btnMushroom = (Button)FindByName("BtnMushroom");
            _btnCrystal = (Button)FindByName("BtnCrystal");
            _btnHerb = (Button)FindByName("BtnHerb");
            _btnFeather = (Button)FindByName("BtnFeather");
            _btnEye = (Button)FindByName("BtnEye");
            _btnRoot = (Button)FindByName("BtnRoot");

            // DI
            _repo = ServiceHelper.Get<IResultsRepository>();
            _session = ServiceHelper.Get<GameSessionService>();

            GenerateNewRecipe();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // старт отсчёта длительности текущего заезда
            _runStartUtc = DateTime.UtcNow;

            // подписка на смену языка из бокового меню
            if (SideMenu != null)
                SideMenu.LanguageChanged += OnLanguageChanged;

            ApplyTexts();
        }

        protected override async void OnDisappearing()
        {
            // при любом уходе со страницы — сохраняем накопленные результаты
            await CommitPendingAsync();

            if (SideMenu != null)
                SideMenu.LanguageChanged -= OnLanguageChanged;

            base.OnDisappearing();
        }

        // ===== Вспомогательный сброс состояния =====
        void ResetGameState()
        {
            currentRecipe.Clear();
            currentStep = 0;
            currentScore = 0;
            _fxLayer?.Children.Clear();

            _runStartUtc = DateTime.UtcNow; // новая попытка
            GenerateNewRecipe();
            ApplyTexts();
        }

        // ===== UI =====
        void ApplyTexts()
        {
            var recipeIcons = string.Join("-", currentRecipe.Select(EmojiFor));
            if (_recipeLabel != null) _recipeLabel.Text = string.Format(AppResources.RecipeFormat, recipeIcons);
            if (_progressLabel != null) _progressLabel.Text = string.Format(AppResources.StepFormat, currentStep, totalSteps);
            if (_scoreLabel != null) _scoreLabel.Text = string.Format(AppResources.ScoreFormat, currentScore);
            if (BtnBack != null) BtnBack.Text = AppResources.BackToMenu;

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
            currentRecipe = AllIngredients.OrderBy(_ => _rng.Next()).Take(totalSteps).ToList();
            currentStep = 0;
            ApplyTexts();
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

                    // добавляем результат в буфер, НО не пишем в БД
                    AddRoundResultToBuffer(currentScore, _runStartUtc);

                    // продолжить игру с новым рецептом
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

        // ====== Буферизация и коммит ======
        void AddRoundResultToBuffer(int score, DateTime startedUtc)
        {
            var duration = (int)Math.Max(0, (DateTime.UtcNow - startedUtc).TotalSeconds);

            _pending.Add(new GameResult
            {
                PlayerName = _session.PlayerName,
                Score = score,
                DateUtc = DateTime.UtcNow,
                DurationSec = duration,
                Notes = "Recipe x3"
            });

            // новая отправная точка для следующего «заезда»
            _runStartUtc = DateTime.UtcNow;
        }

        async Task CommitPendingAsync()
        {
            if (_pending.Count == 0) return;

            foreach (var item in _pending)
                await _repo.AddAsync(item);

            _pending.Clear();
        }

        // ===== Навигация =====
        // Назад в меню
        async void Back_Clicked(object sender, EventArgs e)
        {
            await CommitPendingAsync();
            currentRecipe.Clear();
            currentStep = 0;
            currentScore = 0;
            _fxLayer?.Children.Clear();
            await Shell.Current.GoToAsync("//menu");
        }

        // ===== FX =====
        async Task CorrectFxAsync()
        {
            if (_cauldron is null) return;
            await _cauldron.ScaleTo(1.05, 120);
            await _cauldron.ScaleTo(1.0, 120);
            for (int i = 0; i < 6; i++) _ = SpawnSparkAsync();
        }

        async Task SuccessFxAsync()
        {
            if (_cauldron is null) return;
            await _cauldron.RelRotateTo(10, 80);
            await _cauldron.RelRotateTo(-20, 160);
            await _cauldron.RelRotateTo(10, 80);
            for (int i = 0; i < 10; i++) _ = SpawnSparkAsync(true);
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

            if (_fxLayer.Width <= 0 || _fxLayer.Height <= 0 || _cauldron.Width <= 0 || _cauldron.Height <= 0)
            {
                await Task.Yield();
                if (_fxLayer.Width <= 0 || _fxLayer.Height <= 0 || _cauldron.Width <= 0 || _cauldron.Height <= 0)
                    return;
            }

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

            double cX = _cauldron.X;
            double cY = _cauldron.Y;
            double cW = _cauldron.Width;
            double cH = _cauldron.Height;

            double neckLeft = cX + cW * 0.33;
            double neckRight = cX + cW * 0.67;
            double neckTop = cY + cH * 0.35;
            double neckBot = cY + cH * 0.50;

            double startX = _rng.NextDouble() * (neckRight - neckLeft) + neckLeft;
            double startY = _rng.NextDouble() * (neckBot - neckTop) + neckTop;

            AbsoluteLayout.SetLayoutBounds(dot, new Rect(startX, startY, size, size));
            AbsoluteLayout.SetLayoutFlags(dot, AbsoluteLayoutFlags.None);

            double riseMin = big ? 140 : 110;
            double riseMax = big ? 200 : 160;
            var targetY = startY - _rng.Next((int)riseMin, (int)riseMax);
            double drift = big ? 70 : 50;
            var driftX = startX + (_rng.NextDouble() - 0.5) * (2 * drift);

            await dot.FadeTo(1, 90);

            var t1 = dot.TranslateTo(driftX - startX, targetY - startY, (uint)_rng.Next(400, 700), Easing.CubicOut);
            var t2 = dot.FadeTo(0, (uint)_rng.Next(320, 600));
            await Task.WhenAll(t1, t2);

            _fxLayer.Remove(dot);
        }

        // ======= БОКОВОЕ МЕНЮ =======
        bool _menuOpen = false;

        async void OnMenuClicked(object sender, EventArgs e)
        {
            if (_menuOpen) return;
            _menuOpen = true;

            Drawer.IsVisible = true;
            Dim.IsVisible = true;
            await Task.WhenAll(
                Drawer.TranslateTo(0, 0, 250, Easing.CubicOut),
                Dim.FadeTo(1, 250)
            );
        }

        void OnCloseMenu(object? sender, EventArgs e)
        {
            if (!_menuOpen) return;

            _ = Task.WhenAll(
                Drawer.TranslateTo(-340, 0, 250, Easing.CubicIn),
                Dim.FadeTo(0, 250)
            ).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Drawer.IsVisible = false;
                    Dim.IsVisible = false;
                    _menuOpen = false;
                });
            });
        }

        // Боковое меню -> Новая игра
        async void SideMenu_StartClicked(object sender, EventArgs e)
        {
            await CommitPendingAsync();
            OnCloseMenu(null, EventArgs.Empty);
            await Shell.Current.GoToAsync("//game");
        }

        async void SideMenu_ChallengeClicked(object sender, EventArgs e)
        {
            OnCloseMenu(null, EventArgs.Empty);
            await DisplayAlert("Challenge", "Coming soon!", "OK");
        }

        // Боковое меню -> Результаты
        async void SideMenu_ResultsClicked(object sender, EventArgs e)
        {
            await CommitPendingAsync();
            OnCloseMenu(null, EventArgs.Empty);
            await Shell.Current.GoToAsync("//results");
        }

        // Смена языка: фиксируем буфер и начинаем новую серию
        // Смена языка из бокового меню
        async void OnLanguageChanged(object? sender, string lang)
        {
            await CommitPendingAsync();
            LocalizationService.Apply(lang);

            currentRecipe.Clear();
            currentStep = 0;
            currentScore = 0;
            GenerateNewRecipe();

            OnCloseMenu(null, EventArgs.Empty);
        }
    }
}
