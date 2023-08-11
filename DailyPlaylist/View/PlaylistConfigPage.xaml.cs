using DailyPlaylist.ViewModel;


namespace DailyPlaylist.View
{
    public partial class PlaylistConfigPage : ContentPage
    {
        private readonly PlaylistConfigViewModel _viewModel;

        public PlaylistConfigPage()
        {
            InitializeComponent();

            _viewModel = new PlaylistConfigViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel.LoadGenresCommand.CanExecute(null))
            {
                _viewModel.LoadGenresCommand.Execute(null);
            }
        }


        private async void OnGeneratePlaylist(object sender, EventArgs e)
        {

            var selectedGenre = GenrePicker.SelectedItem as Genre;
            var selectedDecade = DecadePicker.SelectedItem?.ToString();

            if (selectedGenre != null && !string.IsNullOrEmpty(selectedDecade))
            {               
                // Pass the selected Genre Id and Decade to the ViewModel method to fetch albums
                var albums = await _viewModel.SearchAlbumsByGenreAndDecadeAsync(selectedGenre.Id, selectedDecade);

                // Do further processing if necessary. For instance, you might want to select top albums, get tracks etc.

                // Navigate to the PlaylistPage to display the generated playlist, 
                // but also consider passing the generated list or an identifier to that page to display the content.
                await Navigation.PushAsync(new PlaylistPage(albums)); // You might modify PlaylistPage to take a list of albums or some other data structure.
            }
            else
            {
                await DisplayAlert("Selection Missing", "Please select both a genre and a decade.", "OK");
            }
        }

    }
}
