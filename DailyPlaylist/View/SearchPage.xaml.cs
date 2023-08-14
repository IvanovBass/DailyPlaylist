using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();


		BindingContext = new SearchViewModel();

    }

    private async Task AnimatePressedButton(ImageButton button)
    {
        await button.ScaleTo(0.8, 130, Easing.Linear);
        await button.ScaleTo(1, 70, Easing.Linear);
    }

    private async void PreviousButton_Clicked(object sender, EventArgs e)
    {
        await AnimatePressedButton(PreviousButton);
    }

    private async void PlayPauseButton_Clicked(object sender, EventArgs e)
    {
        await AnimatePressedButton(PlayPauseButton);
    }

    private async void ForwardButton_Clicked(object sender, EventArgs e)
    {
        await AnimatePressedButton(ForwardButton);
    }

    private async void CollectionPlayButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimatePressedButton(button);
    }

}