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

                var albums = await _viewModel.SearchAlbumsByGenreAndDecadeAsync(selectedGenre.Id, selectedDecade);

       
                List<Track> MyAlgorithmedPlaylist = new();
                // I don't know how I'm gonna deal with that since Deezer's GET API is less smart than I though , I won't be able to parameter a search query
        
                await Navigation.PushAsync(new PlaylistPage(MyAlgorithmedPlaylist)); 
            }
            else
            {
                await DisplayAlert("Selection Missing", "Please select both a genre and a decade.", "OK");
            }
        }

    }
}
