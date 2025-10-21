namespace PotionPanic;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Intro становится стартовым и так, но можно явно:
        Loaded += async (_, __) => await GoToAsync("//intro");
    }

    public void EnableFlyout() => FlyoutBehavior = FlyoutBehavior.Flyout;
}
