namespace PotionPanic;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        FlyoutBehavior = FlyoutBehavior.Disabled; // на интро — скрыт
        Navigated += OnNavigated;

        // стратуем на Intro
        _ = GoToAsync("//intro");
    }

    void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        // включаем Flyout везде, кроме Intro
        var route = Current?.CurrentItem?.CurrentItem?.Route;
        FlyoutBehavior = route == "intro" ? FlyoutBehavior.Disabled : FlyoutBehavior.Flyout;
    }
}