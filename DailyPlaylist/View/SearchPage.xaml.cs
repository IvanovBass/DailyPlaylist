using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();


		BindingContext = new SearchViewModel();
	}
}