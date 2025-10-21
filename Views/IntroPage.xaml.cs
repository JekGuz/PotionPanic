using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;

namespace PotionPanic.Views;

public partial class IntroPage : ContentPage
{
    bool _played;

    public IntroPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TryPlayWithFallback();
    }

    async void TryPlayWithFallback()
    {
        try { IntroPlayer?.Play(); } catch { /* игнор */ }

        // Фолбэк: если через 2.5 сек не стартануло — идём в меню
        await Task.Delay(2500);
        if (!_played)
            await Shell.Current.GoToAsync("//menu");
    }

    void MediaElement_MediaOpened(object sender, EventArgs e)
    {
        _played = true; // пошло видео — фолбэк не сработает
    }

    void MediaElement_MediaEnded(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//menu");

    void MediaElement_MediaFailed(object sender, MediaFailedEventArgs e)
        => Shell.Current.GoToAsync("//menu");

    void OnTapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("//menu"); // «пропустить»
}
