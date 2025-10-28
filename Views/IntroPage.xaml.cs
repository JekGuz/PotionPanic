using System.Threading;

namespace PotionPanic.Views;

public partial class IntroPage : ContentPage
{
    CancellationTokenSource? _cts;
    bool _navigated;

    public IntroPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            // Ждём ровно 6 секунд или до нажатия Skip
            await Task.Delay(TimeSpan.FromSeconds(6), _cts.Token);
            await GoToMenuOnceAsync();
        }
        catch (TaskCanceledException)
        {
            // Skip или уход со страницы — нормально
        }
    }

    protected override void OnDisappearing()
    {
        _cts?.Cancel();
        _cts = null;
        base.OnDisappearing();
    }

    async void OnSkipClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        await GoToMenuOnceAsync();
    }

    async Task GoToMenuOnceAsync()
    {
        if (_navigated) return;
        _navigated = true;

        // Ставим Shell корнем (или GoTo на //menu, если маршрут есть)
        Application.Current.MainPage = new AppShell();
        await Shell.Current.GoToAsync("//menu");
    }

    // Блокируем аппаратную кнопку "Назад" на интро
    protected override bool OnBackButtonPressed() => true;
}
