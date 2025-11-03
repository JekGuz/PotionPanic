using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio; // для фоновой музыки
using PotionPanic.Services;

#if ANDROID
using PotionPanic.Controls;
using PotionPanic.Platforms.Android;
#endif

namespace PotionPanic
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Transcity.otf", "Transcity");
                });


            builder.Services.AddSingleton(AudioManager.Current); // аудиосистема
            builder.Services.AddSingleton<MusicService>(); // сервис фоновой музыки
            builder.Services.AddSingleton<IResultsRepository, ResultsRepository>(); // результаты
            builder.Services.AddSingleton<GameSessionService>(); // сессия игрока


#if ANDROID
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(VideoPlayerView), typeof(VideoPlayerHandler));
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // создаём приложение
            var app = builder.Build();

            // регистрируем ServiceHelper
            ServiceHelper.Configure(app.Services);

            return app;
        }
    }
}
