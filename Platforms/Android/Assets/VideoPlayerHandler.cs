using Microsoft.Maui.Handlers;
using PotionPanic.Controls;

namespace PotionPanic.Platforms.Android
{
    public class VideoPlayerHandler : ViewHandler<VideoPlayerView, global::Android.Widget.VideoView>
    {
        public static IPropertyMapper<VideoPlayerView, VideoPlayerHandler> Mapper =
            new PropertyMapper<VideoPlayerView, VideoPlayerHandler>(ViewHandler.ViewMapper);

        public VideoPlayerHandler() : base(Mapper) { }

        protected override global::Android.Widget.VideoView CreatePlatformView()
        {
            var vv = new global::Android.Widget.VideoView(Context);

            // Видео: Platforms/Android/Resources/raw/intro.mp4  (строчными!)
            var uri = global::Android.Net.Uri.Parse($"android.resource://{Context.PackageName}/raw/intro");
            vv.SetVideoURI(uri);

            // 🔹 Растянуть на весь контейнер (Full Screen)
            vv.LayoutParameters = new global::Android.Widget.FrameLayout.LayoutParams(
                global::Android.Views.ViewGroup.LayoutParams.MatchParent,
                global::Android.Views.ViewGroup.LayoutParams.MatchParent);

            // Подготовка видео
            vv.SetOnPreparedListener(new PreparedListener());

            // Когда видео закончилось → перейти в меню
            vv.SetOnCompletionListener(new CompletionListener());

            return vv;
        }

        protected override void ConnectHandler(global::Android.Widget.VideoView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.Start();
        }

        protected override void DisconnectHandler(global::Android.Widget.VideoView platformView)
        {
            platformView?.StopPlayback();
            base.DisconnectHandler(platformView);
        }

        // 🔹 Видео готово — можно запустить
        private sealed class PreparedListener
            : global::Java.Lang.Object, global::Android.Media.MediaPlayer.IOnPreparedListener
        {
            public void OnPrepared(global::Android.Media.MediaPlayer mp)
            {
                // Запускаем (без loop!)
                mp.Looping = false;
                mp.Start();
            }
        }

        // 🔹 Видео закончилось → открываем меню
        private sealed class CompletionListener
            : global::Java.Lang.Object, global::Android.Media.MediaPlayer.IOnCompletionListener
        {
            public void OnCompletion(global::Android.Media.MediaPlayer mp)
            {
                // Возврат в меню
                Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            }
        }
    }
}
