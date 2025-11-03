using System.Threading.Tasks;
using PotionPanic.Resources;
using PotionPanic.Services;

namespace PotionPanic.Views;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyTexts();
        HighlightActiveLanguage();
    }

    void ApplyTexts()
    {
        TitleLabel.Text = AppResources.Title;
        StartBtn.Text = AppResources.Start;
        ChallengeBtn.Text = AppResources.Challenge;
        ResultsBtn.Text = AppResources.Results;
        Title = AppResources.Title;
    }

    void HighlightActiveLanguage()
    {
        var en = this.FindByName<ImageButton>("FlagEN");
        var ru = this.FindByName<ImageButton>("FlagRU");
        var et = this.FindByName<ImageButton>("FlagET");
        if (en is null || ru is null || et is null) return;

        en.Opacity = ru.Opacity = et.Opacity = 0.7;
        en.Scale = ru.Scale = et.Scale = 1.0;

        switch (LocalizationService.CurrentCode)
        {
            case "ru": ru.Opacity = 1.0; ru.Scale = 1.08; break;
            case "et": et.Opacity = 1.0; et.Scale = 1.08; break;
            default: en.Opacity = 1.0; en.Scale = 1.08; break;
        }
    }

    // Флажки языка
    void LangEn_Clicked(object s, EventArgs e) { LocalizationService.Apply("en"); ApplyTexts(); HighlightActiveLanguage(); }
    void LangRu_Clicked(object s, EventArgs e) { LocalizationService.Apply("ru"); ApplyTexts(); HighlightActiveLanguage(); }
    void LangEt_Clicked(object s, EventArgs e) { LocalizationService.Apply("et"); ApplyTexts(); HighlightActiveLanguage(); }

    // Кнопки меню
    void StartBtn_Clicked(object sender, EventArgs e) => _ = StartFlowAsync();

    async Task StartFlowAsync()
    {
        var session = ServiceHelper.Get<GameSessionService>();
        session.LoadOrStart();

        var name = await DisplayPromptAsync(
            AppResources.Title,
            "Введите имя игрока:",
            accept: "OK", cancel: "Cancel",
            initialValue: session.PlayerName,
            maxLength: 24, keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(name))
            return; // отменили

        session.StartNew(name.Trim());
        await Shell.Current.GoToAsync("//game");
    }

    // музыка
    async void MusicToggle_Clicked(object sender, EventArgs e)
    {
        var music = ServiceHelper.Get<MusicService>();
        await music.ToggleAsync();
    }

    async void ChallengeBtn_Clicked(object sender, EventArgs e)
        => await DisplayAlert("Challenge", "Coming soon!", "OK");

    async void ResultsBtn_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//results");
}
