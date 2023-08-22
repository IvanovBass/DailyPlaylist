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

        var searchVM = ServiceHelper.GetService<SearchViewModel>();
        if (searchVM != null)
        {
            _searchViewModel = searchVM;
            BindingContext = searchVM;
        }
        else
        {
            BindingContext = null;
            Application.Current.MainPage.DisplayAlert("Error", "There was an error loading the Playlist context, you won't be able to add songs to your favorite playlists. Consider logging out and back in", "OK");
        }

        var playVM = ServiceHelper.GetService<PlaylistViewModel>();
        _playlistViewModel = playVM;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (NavigationState.refreshFavoritesNeeded)
        {
            NavigationState.refreshFavoritesNeeded = false;
            _searchViewModel.LoadSelectedFavoriteTrackUris();
        }
    }
    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }
}