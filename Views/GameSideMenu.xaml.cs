using System;
using Microsoft.Maui.Controls;
using PotionPanic.Services;

namespace PotionPanic.Views;

public partial class GameSideMenu : ContentView
{
    public event EventHandler? StartClicked;
    public event EventHandler? ChallengeClicked;
    public event EventHandler? ResultsClicked;
    public event EventHandler<string>? LanguageChanged;

    public GameSideMenu()
    {
        InitializeComponent();

        // Показать корректный текст на кнопке при открытии меню
        var music = ServiceHelper.Get<MusicService>();
        UpdateMusicButton(music.IsPlaying);
    }

    // Музыка: вкл/выкл
    async void MusicToggle_Clicked(object sender, EventArgs e)
    {
        var music = ServiceHelper.Get<MusicService>();
        await music.ToggleAsync();
        UpdateMusicButton(music.IsPlaying);
    }

    void UpdateMusicButton(bool isPlaying)
    {
        if (MusicBtn == null) return;
        MusicBtn.Text = isPlaying ? "🔊" : "🔇";
    }

    void OnStartClicked(object sender, EventArgs e) => StartClicked?.Invoke(this, EventArgs.Empty);
    void OnChallengeClicked(object sender, EventArgs e) => ChallengeClicked?.Invoke(this, EventArgs.Empty);
    void OnResultsClicked(object sender, EventArgs e) => ResultsClicked?.Invoke(this, EventArgs.Empty);

    void OnEnglishTapped(object sender, TappedEventArgs e) => LanguageChanged?.Invoke(this, "en");
    void OnRussianTapped(object sender, TappedEventArgs e) => LanguageChanged?.Invoke(this, "ru");
    void OnEstonianTapped(object sender, TappedEventArgs e) => LanguageChanged?.Invoke(this, "et");
}
