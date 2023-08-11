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
            // Logic to generate playlist based on user selections.
            // You can access the values like GenrePicker.SelectedItem, DecadePicker.SelectedItem, etc.

            // Example:
            string selectedGenre = GenrePicker.SelectedItem?.ToString();

            // After processing, navigate to the generated playlist page or show results.
            await Navigation.PushAsync(new PlaylistPage()); // Assuming you have a PlaylistPage to display the generated playlist.
        }
    }
}
