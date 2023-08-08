using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class PlaylistPage : ContentPage
{
	public PlaylistPage(PlaylistViewModel pvm)
	{
		InitializeComponent();
		BindingContext = pvm;
	}
}