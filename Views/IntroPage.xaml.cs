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
            // 1) Открываем видео из пакета (Resources/Raw/intro.mp4)
            using var inStream = await FileSystem.OpenAppPackageFileAsync("intro.mp4");

            // 2) Копируем во временный файл (кэш приложения)
            var tempPath = Path.Combine(FileSystem.CacheDirectory, "intro.mp4");
            using (var outStream = File.Create(tempPath))
                await inStream.CopyToAsync(outStream);

            // 3) Задаём источник как "обычный файл" — это самый стабильный вариант
            Video.Source = MediaSource.FromFile(tempPath);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Video", "Cannot open intro video: " + ex.Message, "OK");
            await OnEndAsync();
        }
    }

    // события MediaElement
    void OnOpened(object? sender, EventArgs e) { /* ok */ }

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
