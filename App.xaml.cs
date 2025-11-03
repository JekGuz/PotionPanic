using PotionPanic.Services;

namespace PotionPanic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Запуск фоновой музыки (один раз на всё приложение)
            var music = ServiceHelper.Get<MusicService>();
            _ = music.PlayAsync(); // запускаем без ожидания, чтобы не тормозить старт

            // стартовая страница
            MainPage = new Views.IntroPage();
        }
    }
}
