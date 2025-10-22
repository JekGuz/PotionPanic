using System.Diagnostics;
using System.IO;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace PotionPanic.Views;

public partial class IntroPage : ContentPage
{
    public IntroPage() => InitializeComponent();

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
            await OnEndAsync();
        }
    }

    async void OnFailed(object? sender, MediaFailedEventArgs e)
    {
        Debug.WriteLine($"Intro failed: {e.ErrorMessage}");
        await OnEndAsync();
    }

    async void OnEnded(object? sender, EventArgs e)
    {
        Debug.WriteLine("Intro: MediaEnded fired");
        await OnEndAsync();
    }

    void OnOpened(object? s, EventArgs e)
    {
        SkipBtn.FadeTo(0.85, 250);
    }

    private async Task OnEndAsync()
    {
        Debug.WriteLine($"Intro: OnEndAsync - Shell.Current = {(Shell.Current is null ? "null" : "not null")}");
        if (Shell.Current is null) return;

        try
        {
            await Shell.Current.GoToAsync("//menu");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Intro: navigation failed: {ex.GetType().Name}: {ex.Message}");
        }
    }
}