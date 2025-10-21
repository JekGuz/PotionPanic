namespace PotionPanic.Views;

public partial class IntroPage : ContentPage
{
    public IntroPage()
    {
        InitializeComponent();
    }

    void MediaElement_MediaEnded(object sender, EventArgs e)
        => Shell.Current.GoToAsync("//menu");

    void OnTapped(object sender, TappedEventArgs e)
        => Shell.Current.GoToAsync("//menu");
}
