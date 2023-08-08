using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DailyPlaylist.ViewModel
{
    internal class LoginViewModel
    {
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
