using System.Diagnostics;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives; // MediaFailedEventArgs
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace PotionPanic.Views;

public partial class IntroPage : ContentPage
{
    bool _completed; // защита от повторного выхода

    public IntroPage()
    {
        InitializeComponent();
        // iOS: реальный фуллскрин без Safe Area
        On<iOS>().SetUseSafeArea(false);
        Padding = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            Debug.WriteLine("Intro: OnAppearing start");

            using var inStream = await FileSystem.OpenAppPackageFileAsync("intro.mp4");
            var tempPath = Path.Combine(FileSystem.CacheDirectory, "intro_intro.mp4");
            using (var outStream = File.Create(tempPath))
                await inStream.CopyToAsync(outStream);

            Video.Source = MediaSource.FromFile(tempPath);
            Debug.WriteLine($"Intro: video set at {tempPath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Intro open error: {ex.GetType().Name}: {ex.Message}");
            await SafeExitAsync();
        }
    }

    void OnOpened(object? s, EventArgs e)
    {
        _ = SkipBtn.FadeTo(0.85, 250);
    }

    async void OnFailed(object? sender, MediaFailedEventArgs e)
    {
        Debug.WriteLine($"Intro failed: {e.ErrorMessage}");
        await SafeExitAsync();
    }

    async void OnEnded(object? sender, EventArgs e)
    {
        Debug.WriteLine("Intro: MediaEnded fired");
        await SafeExitAsync();
    }

    async Task SafeExitAsync()
    {
        if (_completed) return;
        _completed = true;

        try { if (Blackout != null) await Blackout.FadeTo(1, 120); } catch { }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.GoToAsync("//menu"));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Intro: navigation failed: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
