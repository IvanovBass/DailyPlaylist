using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
    private PlaylistViewModel _playlistViewModel;
	public PlaylistPage()
    {
        InitializeComponent();

        var playlistViewModel = ServiceHelper.GetService<PlaylistViewModel>();
        if (playlistViewModel != null )
        {
            _playlistViewModel = playlistViewModel;
            BindingContext = _playlistViewModel;
            _playlistViewModel.PromptEditEvent += PromptMessageEditAsync;
            _playlistViewModel.PromptCreateEvent += PromptCreateAsync;

        }
        else 
        { 
            BindingContext = null;
            Application.Current.MainPage.DisplayAlert("Error", "There was an error loading the Playlists, consider logging out and back in", "OK");
        }
    }

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        await AnimationHelper.AnimatePressedImageButton(button);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (NavigationState.LastVisitedPage == nameof(SearchPage))
        {
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
                // _playlistViewModel.SelectedPlaylist.DateUpdated = DateTime.Now;  for the Save process
                _playlistViewModel.UserPlaylists = tempList;
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
                    DeezerTrackIds = new List<long>()
                };

                var insertedPlaylist = await _playlistViewModel.InsertNewPlaylistAsync(newPlaylist);

                if (insertedPlaylist != null)
                {
                    _playlistViewModel.UserPlaylists.Add(newPlaylist);
                    _playlistViewModel.SelectedPlaylist = newPlaylist;

                    await SnackBarVM.ShowSnackBarAsync("Playlist '" + newName + "' successfully created!", "OK", () => { });
                }
                else
                {
                    await SnackBarVM.ShowSnackBarAsync("Failed to create the playlist. Please try again.", "Dismiss", () => { });
                }
            }
        }
    }

}
