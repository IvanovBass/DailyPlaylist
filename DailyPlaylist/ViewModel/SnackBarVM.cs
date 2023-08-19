namespace DailyPlaylist.ViewModel
{
    public static class SnackBarVM
    {
        public static async Task ShowSnackBarAsync(string message, string actionText, Action action, int durationInSeconds = 3)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkSlateBlue,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Orange,
                CornerRadius = new CornerRadius(10),

                Font = Microsoft.Maui.Font.SystemFontOfSize(15),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(15),
                CharacterSpacing = 0
            };

            var snackbar = Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(durationInSeconds), snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static async Task ShowSnackBarShortAsync(string message, string actionText, Action action, int durationInSeconds = 2)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkSlateBlue,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Orange,
                CornerRadius = new CornerRadius(10),

                Font = Microsoft.Maui.Font.SystemFontOfSize(16),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(16),
                CharacterSpacing = 0.1
            };

            var snackbar = Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(durationInSeconds), snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }
    }

}
