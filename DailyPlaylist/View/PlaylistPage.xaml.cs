using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
    private PlaylistViewModel _playlistViewModel;
    private AppSessionManager _sessionManager;

    // CONSTRUCTOR //
	public PlaylistPage()
    {
        InitializeComponent();

        _sessionManager = ServiceHelper.GetService<AppSessionManager>();

        _playlistViewModel = _sessionManager.PlaylistViewModel;

        BindingContext = _playlistViewModel;

        _playlistViewModel.PromptEditEvent += PromptMessageEditAsync;
        _playlistViewModel.PromptCreateEvent += PromptCreateAsync;

    }


    // METHODS //

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (NavigationState.refreshFavoritesNeeded)
        {
            NavigationState.refreshFavoritesNeeded = false;
            _playlistViewModel.LoadTracksForPlaylist(_playlistViewModel.SelectedPlaylist);
        }
        if (NavigationState.IsReloggedPVM)
        {
            _sessionManager = ServiceHelper.GetService<AppSessionManager>();

            _playlistViewModel = _sessionManager.PlaylistViewModel;

            BindingContext = _playlistViewModel;

            NavigationState.IsReloggedPVM = false;

            _playlistViewModel.PromptEditEvent += PromptMessageEditAsync;
            _playlistViewModel.PromptCreateEvent += PromptCreateAsync;

        }

    }
    public async void PromptMessageEditAsync()
    {
        var newName = await DisplayPromptAsync("Edit Playlist", "Enter new name:", "OK", "Cancel", _playlistViewModel.SelectedPlaylist.Name);
        if (!string.IsNullOrEmpty(newName))
        {

            var newDescription = await DisplayPromptAsync("Edit Playlist", "Enter new description:", "OK", "Cancel", _playlistViewModel.SelectedPlaylist.Description);
            if (!string.IsNullOrEmpty(newDescription))
            {
                var tempList = new ObservableCollection<Tracklist>(_playlistViewModel.UserPlaylists);
                _playlistViewModel.SelectedPlaylist.Name = newName;
                _playlistViewModel.SelectedPlaylist.Description = newDescription;
                var tempSelectedPlaylist = _playlistViewModel.SelectedPlaylist;
                _playlistViewModel.UserPlaylists = tempList;
                _playlistViewModel.SelectedPlaylist = tempSelectedPlaylist;
            }
        }
    }

    public async void PromptCreateAsync()
    {
        var newName = await DisplayPromptAsync("Create Playlist", "Enter playlist name:", "OK", "Cancel");
        if (!string.IsNullOrEmpty(newName))
        {
            var newDescription = await DisplayPromptAsync("Create Playlist", "Enter playlist description:", "OK", "Cancel");
            if (!string.IsNullOrEmpty(newDescription))
            {
                var newPlaylist = new Tracklist
                {
                    UserId = _playlistViewModel.ActiveUser.Id,
                    Name = newName,
                    Description = newDescription,
                    DeezerTracks = new List<Track>()
                };

                var insertedPlaylist = await _playlistViewModel.InsertNewPlaylistAsync(newPlaylist);

                if (insertedPlaylist != null)
                {
                    _playlistViewModel.UserPlaylists.Add(newPlaylist);
                    _playlistViewModel.SelectedPlaylist = newPlaylist;

                    await SnackBarVM.ShowSnackBarAsync("Playlist '" + newName + "' successfully created!", "OK", () => { });
                    NavigationState.refreshFavoritesNeeded = true;
                }
                else
                {
                    await SnackBarVM.ShowSnackBarAsync("Failed to create the playlist. Please try again.", "Dismiss", () => { });
                }
            }
        }
    }

}
