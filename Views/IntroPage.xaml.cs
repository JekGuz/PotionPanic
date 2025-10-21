using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives; // MediaFailedEventArgs
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage; // FileSystem

namespace PotionPanic.Views;

public partial class IntroPage : ContentPage
{
    public IntroPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            using var inStream = await FileSystem.OpenAppPackageFileAsync("intro.mp4");
            var tempPath = Path.Combine(FileSystem.CacheDirectory, "intro.mp4");
            using (var outStream = File.Create(tempPath))
                await inStream.CopyToAsync(outStream);

            Video.Source = MediaSource.FromFile(tempPath);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Video", "Cannot open intro: " + ex.Message, "OK");
            await OnEndAsync();
        }
    }

    void OnOpened(object? s, EventArgs e)
    {
        // показываем кнопку м€гко, когда видео уже вывелось Ч меньше шансов мигать
        SkipBtn.FadeTo(0.85, 250);
    }

    async void OnFailed(object? sender, MediaFailedEventArgs e)
    {
        await DisplayAlert("Video", "Playback failed.", "OK");
        await OnEndAsync();
    }

    async void OnEnded(object? sender, EventArgs e) => await OnEndAsync();

    // завершаем интро
    private async Task OnEndAsync()
    {
        var shell = App.Current?.Windows[0].Page as AppShell;
        shell?.EnableFlyout();
        await Shell.Current.GoToAsync("//main");
    }
}
