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
        // Если Switch используем как “циклическую” кнопку — держим его в положении Off
        // чтобы визуально не вводил в заблуждение
        if (LangSwitch.IsToggled) LangSwitch.IsToggled = false;
    }

    // Перерисовать все тексты по текущей культуре
    void ApplyTexts()
    {
        TitleLabel.Text = AppResources.Title;
        StartBtn.Text = AppResources.Start;
        ChallengeBtn.Text = AppResources.Challenge;
        ResultsBtn.Text = AppResources.Results;

        LangLabel.Text = AppResources.Language;
        LangCodesLabel.Text = $"{AppResources.RU} / {AppResources.ET} / EN";
        // Заголовок страницы — если используешь Title из XAML, можно не трогать
        Title = AppResources.Title;
    }

    // Переключатель языка: крутим EN→RU→ET→EN
    void LangSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var next = LocalizationService.Next();
        LocalizationService.Apply(next);
        ApplyTexts();
        // Сбрасываем переключатель обратно (он выполняет роль кнопки)
        LangSwitch.IsToggled = false;
    }

    // Твои обработчики (оставь как были)
    void StartBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//game"); // или твой маршрут
    }

    void ChallengeBtn_Clicked(object sender, EventArgs e)
    {
        // ...
    }

    void ResultsBtn_Clicked(object sender, EventArgs e)
    {
        // ...
    }
}
