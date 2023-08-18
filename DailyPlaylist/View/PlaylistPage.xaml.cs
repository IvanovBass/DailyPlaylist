using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
	private readonly AuthService _authService;
    private PlaylistViewModel _playlistViewModel;
	public PlaylistPage()
    {
        InitializeComponent();

        var authService = ServiceHelper.GetService<AuthService>();

        _authService = authService;

        _playlistViewModel = new PlaylistViewModel(_authService);

        BindingContext = _playlistViewModel;

    }

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        NavigationState.LastVisitedPage = nameof(PlaylistPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (NavigationState.LastVisitedPage == nameof(SearchPage)
            && _playlistViewModel.PlaylistTracks != null
            && _playlistViewModel.PlaylistTracks is ObservableCollection<Track>)
        {
            CrossMediaManager.Current.Dispose();
            CrossMediaManager.Current.Init();

            var mediaPlayer = new MediaPlayerService(_playlistViewModel.PlaylistTracks.ToList());
            mediaPlayer.storedIndex = _playlistViewModel.preStoredIndex;
        }
    }

}
