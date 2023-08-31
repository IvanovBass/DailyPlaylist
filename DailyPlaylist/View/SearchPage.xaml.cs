using DailyPlaylist.ViewModel;
using DailyPlaylist.Services;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{

    private SearchViewModel _searchViewModel;


    public SearchPage(AppSessionManager appSessionManager)
    {
        InitializeComponent();

        var searchViewModel = appSessionManager.SearchViewModel;

        _searchViewModel = searchViewModel;

        BindingContext = searchViewModel;

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