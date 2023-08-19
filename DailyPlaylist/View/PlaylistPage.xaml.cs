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
}
