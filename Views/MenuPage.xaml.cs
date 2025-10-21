using System.Globalization;
using PotionPanic.Resources;

namespace PotionPanic.Views;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
        ApplyStrings();
    }

    void ApplyStrings()
    {
        Title = AppResources.Title;
        TitleLabel.Text = AppResources.Title;
        StartBtn.Text = AppResources.Start;
        ChallengeBtn.Text = AppResources.Challenge;
        ResultsBtn.Text = AppResources.Results;
        LangLabel.Text = AppResources.Language;
        LangCodesLabel.Text = "RU/ET";
    }

    void SetCulture(string code)
    {
        var ci = new CultureInfo(code);
        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
        AppResources.Culture = ci;
        ApplyStrings();
    }

    void LangSwitch_Toggled(object sender, ToggledEventArgs e)
        => SetCulture(e.Value ? "et" : "ru");

    async void StartBtn_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//game");

    async void ChallengeBtn_Clicked(object sender, EventArgs e)
        => await DisplayAlert(AppResources.Title, "Challenge подключим позже.", "OK");

    async void ResultsBtn_Clicked(object sender, EventArgs e)
        => await DisplayAlert(AppResources.Title, "ResultsPage сделаем после GamePage.", "OK");
}
