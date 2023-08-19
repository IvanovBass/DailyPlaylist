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
}