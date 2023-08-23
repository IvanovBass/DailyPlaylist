using DailyPlaylist.ViewModel;
using DailyPlaylist.Services;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{

    private AppSessionManager _sessionManager;
    private SearchViewModel _searchViewModel;


    public SearchPage()
    {
        InitializeComponent();

        _sessionManager = ServiceHelper.GetService<AppSessionManager>();

        _searchViewModel = _sessionManager.SearchViewModel;

        BindingContext = _searchViewModel;
            
        _searchViewModel.Initialize(_sessionManager.PlaylistViewModel);

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (NavigationState.refreshFavoritesNeeded)
        {
            NavigationState.refreshFavoritesNeeded = false;
            _searchViewModel.LoadSelectedFavoriteTrackUris();
        }

        if (NavigationState.IsReloggedSVM)
        {
            _sessionManager = ServiceHelper.GetService<AppSessionManager>();

            _searchViewModel = _sessionManager.SearchViewModel;

            BindingContext = _searchViewModel;

            _searchViewModel.Initialize(_sessionManager.PlaylistViewModel);

            NavigationState.IsReloggedSVM = false;
        }
    }
    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }
}