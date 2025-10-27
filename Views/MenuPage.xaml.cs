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
        // Сбрасываем все флажки
        var en = this.FindByName<ImageButton>("FlagEN");
        var ru = this.FindByName<ImageButton>("FlagRU");
        var et = this.FindByName<ImageButton>("FlagET");
        if (en is null || ru is null || et is null) return;

        
        en.Opacity = ru.Opacity = et.Opacity = 0.7;
        en.Scale = ru.Scale = et.Scale = 1.0;

        // Выделяем активный язык
        switch (LocalizationService.CurrentCode)
        {
            case "ru": ru.Opacity = 1.0; ru.Scale = 1.08; break;
            case "et": et.Opacity = 1.0; et.Scale = 1.08; break;
            default: en.Opacity = 1.0; en.Scale = 1.08; break;
        }
    }

    // Флажки языка
    void LangEn_Clicked(object s, EventArgs e)
    {
        LocalizationService.Apply("en");
        ApplyTexts();
        HighlightActiveLanguage();
    }

    void LangRu_Clicked(object s, EventArgs e)
    {
        LocalizationService.Apply("ru");
        ApplyTexts();
        HighlightActiveLanguage();
    }

    void LangEt_Clicked(object s, EventArgs e)
    {
        LocalizationService.Apply("et");
        ApplyTexts();
        HighlightActiveLanguage();
    }

    // Кнопки меню
    void StartBtn_Clicked(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//game");

    void ChallengeBtn_Clicked(object sender, EventArgs e)
    {
        // ...
    }

    void ResultsBtn_Clicked(object sender, EventArgs e)
    {
        // ...
    }
}
