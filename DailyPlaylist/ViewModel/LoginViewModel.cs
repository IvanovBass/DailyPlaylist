using DailyPlaylist.Services;
using DailyPlaylist.View;
using System.Text.RegularExpressions;

namespace DailyPlaylist.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _email;
        private string _password;
        private User _activeUser;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public User ActiveUser
        {
            get => _activeUser;
            set
            {
                _activeUser = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public ICommand CreateAccountCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;

            LoginCommand = new Command(async () => await ExecuteLoginCommand());

            CreateAccountCommand = new Command(async () => await ExecuteCreateAccountCommand());

        }

        private async Task ExecuteLoginCommand()
        {
            if (await _authService.IsAuthenticatedAsync())
            {
                await ShowSnackBarAsync("You're already logged in", "Dismiss", () => { });
            }
            else
            {
                // aller check si username existe etcetera ....
            }
        }

        private async Task ExecuteCreateAccountCommand()
        {
            if (!IsValidEmail(Email))
            {
                await ShowSnackBarAsync("Invalid Email format...", "Dismiss", () => { });
                return;
            }
            if (!IsValidPassword(Password))
            {
                await ShowSnackBarAsync("Password must contains 7 characters and at least 1 digit", "Dismiss", () => { });
                return;
            }
            
            bool succes = await _authService.CreateAccountAsync(Email, Password);

            if (succes)
            {

                _activeUser = new User { Email = Email };
                _authService.Login();
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                // to the homepage with a succes snackbar
            }
            else
            {
                // something wrent wrong or user already exists , please retry
            }
        }

        private bool IsValidEmail(string email)
        {
            // regex found on StackOverflow
            var emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$";
            return Regex.IsMatch(email, emailRegex);
        }

        private bool IsValidPassword(string password)
        {
            // makes sure that the password has at least 7 characters, from which at least one digit and one letter
            return password.Length >= 7 && Regex.IsMatch(password, @"\d") && Regex.IsMatch(password, @"[a-zA-Z]");
        }

        public async Task ShowSnackBarAsync(string message, string actionText, Action action, int durationInSeconds = 3)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkSlateBlue,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Orange,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(12),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12),
                CharacterSpacing = 0.5
            };

            var snackbar = Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(durationInSeconds), snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }
    }
}