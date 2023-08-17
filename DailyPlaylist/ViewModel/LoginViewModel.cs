﻿using DailyPlaylist.Services;
using DailyPlaylist.View;
using System.Text.RegularExpressions;

namespace DailyPlaylist.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _email;
        private string _password;

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

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowSnackBarAsync("Please fill in the fields correctly", "Dismiss", () => { });
                return;
            }

            await Shell.Current.GoToAsync($"//{nameof(LoadingPage)}"); // on navigue vers la page intermédiaire de loading
            var authUser = await _authService.LoginAsync(Email, Password);  // on check si Login OK

            if (authUser != null && authUser is User)
            {
                _authService.Login(authUser);  // on log le User dans le système, qui va devenir le User actif
                await ShowSnackBarAsync("Succesfully logged in", "Dismiss", () => { });  // self-explanatory
            }
            else
            {
                await ShowSnackBarAsync("Wrong credentials/user. Please retry", "Dismiss", () => { });
                return;
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

            await Shell.Current.GoToAsync($"//{nameof(LoadingPage)}");
            User createdUser = await _authService.CreateAccountAsync(Email, Password);

            if (createdUser != null && createdUser is User)
            {

                _authService.Login(createdUser);
                await ShowSnackBarAsync("User created succesfully!", "Dismiss", () => { });
            }
            else
            {
                await ShowSnackBarAsync("Something wrent wrong. Make sure the Email doesn't already exist and retry", "Dismiss", () => { });
                return;
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
            // ensures that the password has at least 7 characters, from which at least one digit and one letter
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
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                CharacterSpacing = 0
            };

            var snackbar = Snackbar.Make(message, action, actionText, TimeSpan.FromSeconds(durationInSeconds), snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }
    }
}