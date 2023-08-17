using DailyPlaylist.ViewModel;
using DailyPlaylist.Services;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();


		BindingContext = new SearchViewModel();

    }

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }
}