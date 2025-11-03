using PotionPanic.Services;

namespace PotionPanic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // фоновая музыка
            var music = ServiceHelper.Get<MusicService>();
            _ = music.PlayAsync(); // без await, просто запустить в фоне

            // стартовая страница
            MainPage = new Views.IntroPage();
        }
    }
}
