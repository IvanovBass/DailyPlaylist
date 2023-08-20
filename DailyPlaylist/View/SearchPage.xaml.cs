using DailyPlaylist.ViewModel;
using DailyPlaylist.Services;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{

    private SearchViewModel _searchViewModel;
    private PlaylistViewModel _playlistViewModel;
	public SearchPage()
	{
		InitializeComponent();

        var playlistViewModel = ServiceHelper.GetService<PlaylistViewModel>();
        if (playlistViewModel != null)
        {
            _playlistViewModel = playlistViewModel;
            BindingContext = _playlistViewModel;
        }
        else
        {
            BindingContext = null;
            Application.Current.MainPage.DisplayAlert("Error", "There was an error loading the Playlist context, you won't be able to add songs to your favorite playlists. Consider logging out and back in", "OK");
        }

        var viewModel = new SearchViewModel(_playlistViewModel);
        _searchViewModel = viewModel;
		BindingContext = _searchViewModel;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (NavigationState.LastVisitedPage == nameof(PlaylistPage))
        {
            _searchViewModel.LoadSelectedFavoriteTrackUris();
            if (_searchViewModel.SearchResults.Count > 0)
            {
                CrossMediaManager.Current.Queue.Clear();
                _searchViewModel.mediaPlayerService = new MediaPlayerService(_searchViewModel.SearchResults.ToList(), true);
                CrossMediaManager.Current.PlayQueueItem(_searchViewModel.SearchResults.IndexOf(_searchViewModel.SelectedTrack));
            }
        }
    }
    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }
}