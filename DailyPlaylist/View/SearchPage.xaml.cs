using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{
    public SearchViewModel _bindingContext {get; set;}
	public SearchPage()
	{
		InitializeComponent();


		_bindingContext = new SearchViewModel();

    }

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }
}
