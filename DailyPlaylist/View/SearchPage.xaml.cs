using DailyPlaylist.ViewModel;
using DailyPlaylist.Services;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{

    private SearchViewModel _viewModel;
	public SearchPage()
	{
		InitializeComponent();

        var viewModel = new SearchViewModel();
        _viewModel = viewModel;
		BindingContext = _viewModel;

    }

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        NavigationState.LastVisitedPage = nameof(SearchPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (NavigationState.LastVisitedPage == nameof(PlaylistPage)
            && _viewModel.SearchResults != null
            && _viewModel.SearchResults is ObservableCollection<Track>)
        {
            CrossMediaManager.Current.Dispose();
            CrossMediaManager.Current.Init();

            var mediaPlayer = new MediaPlayerService(_viewModel.SearchResults.ToList());
            mediaPlayer.storedIndex = _viewModel.preStoredIndex;
        }
    }


}