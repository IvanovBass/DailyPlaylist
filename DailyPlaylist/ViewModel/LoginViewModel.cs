using Realms.Sync;

namespace DailyPlaylist.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            EmailText = "test@test.com";
            PasswordText = "testtest";
        }

        [ObservableProperty]
        ObservableCollection<Playlist> userPlaylists;

        [ObservableProperty]
        string emailText;

        [ObservableProperty]
        string passwordText;

        public static async void StartPlaylist()
        {
            await Shell.Current.GoToAsync("//Main");
        }

        [RelayCommand]
        async Task CreateAccount()
        {
            try
            {
                await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(EmailText, PasswordText);

                await Login();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error creating account;", "Error: " + ex.Message, "OK");
            }
        }

        [RelayCommand]

        public async Task Login()
        {
            try
            {
                var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(EmailText, PasswordText));

                if (user != null)
                {
                    await Shell.Current.GoToAsync("//Main");
                    EmailText = "";
                    PasswordText = "";
                }
                else
                {
                    throw new Exception();
                }
            } 
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error logging in ... ", ex.Message, "OK");
            }
        }
    }
}

//public class LoginViewModel : BaseViewModel
//{
//    private readonly AuthService _authService;
//    private string _username;
//    private string _password;

//    public string Username
//    {
//        get => _username;
//        set
//        {
//            _username = value;
//            OnPropertyChanged();
//        }
//    }

//    public string Password
//    {
//        get => _password;
//        set
//        {
//            _password = value;
//            OnPropertyChanged();
//        }
//    }

//    public ICommand LoginCommand { get; }

//    public LoginViewModel(AuthService authService)
//    {
//        _authService = authService;
//        LoginCommand = new Command(async () => await ExecuteLoginCommand());
//    }

//    private async Task ExecuteLoginCommand()
//    {
//        if (await _authService.IsAuthenticatedAsync(Username, Password))
//        {
//            // succès ...
//        }
//        else
//        {
//            // echec .
//        }
//    }

//    // Implement the INotifyPropertyChanged interface to inform the view of property changes
//    public event PropertyChangedEventHandler PropertyChanged;

//    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//    {
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
