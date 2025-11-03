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
    }

    // мызыка (кнопка)
    private async void MusicBtn_Clicked(object? sender, EventArgs e)
    {
        var music = ServiceHelper.Get<MusicService>();
        await music.ToggleAsync();
    }

    void OnStartClicked(object sender, EventArgs e) => StartClicked?.Invoke(this, EventArgs.Empty);
    void OnChallengeClicked(object sender, EventArgs e) => ChallengeClicked?.Invoke(this, EventArgs.Empty);
    void OnResultsClicked(object sender, EventArgs e) => ResultsClicked?.Invoke(this, EventArgs.Empty);

    void OnEnglishTapped(object sender, TappedEventArgs e) => LanguageChanged?.Invoke(this, "en");
    void OnRussianTapped(object sender, TappedEventArgs e) => LanguageChanged?.Invoke(this, "ru");
    void OnEstonianTapped(object sender, TappedEventArgs e) => LanguageChanged?.Invoke(this, "et");
}
