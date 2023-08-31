using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
    private PlaylistViewModel _playlistViewModel;

    // CONSTRUCTOR //
	public PlaylistPage(AppSessionManager appSessionManager)
    {
        InitializeComponent();

        var playlistViewModel = appSessionManager.PlaylistViewModel;

        _playlistViewModel = playlistViewModel;

        BindingContext = playlistViewModel;

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
                await SnackBarVM.ShowSnackBarAsync("Playlist '" + newName + "' successfully edited", "OK", () => { });
                await Task.Delay(400);
                _playlistViewModel.UserPlaylists = tempList;
                _playlistViewModel.SelectedPlaylist = tempSelectedPlaylist;
                return;
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
                    UserId = _playlistViewModel.ActiveUser?.Id,
                    Name = newName,
                    Description = newDescription,
                    DeezerTracks = new List<Track>()
                };

                var insertedPlaylist = await _playlistViewModel.InsertNewPlaylistAsync(newPlaylist);

                _playlistViewModel.UserPlaylists.Add(newPlaylist);
                _playlistViewModel.SelectedPlaylist = newPlaylist;
                NavigationState.refreshFavoritesNeeded = true;

                if (insertedPlaylist != null)
                {
                    await SnackBarVM.ShowSnackBarAsync("Playlist '" + newName + "' successfully created!", "OK", () => { });
                }
                else
                {                  
                    await SnackBarVM.ShowSnackBarAsync("Failed to store the Playlist. Please make sure you are connected and your playlists are sync'd.", "Dismiss", () => { });
                }
                return;
            }
        }
    }

}
