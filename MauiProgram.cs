using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
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

            // DI: репозиторий результатов
            builder.Services.AddSingleton<IResultsRepository, ResultsRepository>();

            // DI: сессия игры
            builder.Services.AddSingleton<GameSessionService>();

#if ANDROID
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(VideoPlayerView), typeof(VideoPlayerHandler));
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Собираем приложение
            var app = builder.Build();

            // Даём доступ к ServiceProvider через наш хелпер
            ServiceHelper.Configure(app.Services);

            return app;
        }
    }
}
