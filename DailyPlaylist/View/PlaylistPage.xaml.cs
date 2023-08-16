using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
	public PlaylistPage()
	{
		InitializeComponent();

	}

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }
}