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
                var tempList = new ObservableCollection<Playlist>(_playlistViewModel.UserPlaylists);
                _playlistViewModel.SelectedPlaylist.Name = newName;
                _playlistViewModel.SelectedPlaylist.Description = newDescription;
                _playlistViewModel.UserPlaylists = tempList;
            }
        }
    }

}
