using DailyPlaylist.ViewModel;

namespace DailyPlaylist.View;

public partial class LoginPage : ContentPage
{

	public LoginPage(LoginViewModel lvm)
	{
		InitializeComponent();

		BindingContext = lvm ;
	}
}
 //   private readonly AuthService _authService; 
    
 //   public LoginPage(AuthService authService)
	//{
	//	InitializeComponent();
 //       _authService = authService;

 //       // to check tuto indian + github Login : https://www.youtube.com/watch?v=97G-XkuENYE
 //       // to check login UI : https://github.com/syazwan089/.NET-MAUI-SIMPLE-LOGIN-PAGE/blob/main/SpeedUI/MainPage.xaml
 //   }

 //   private async void ButtonLoginClicked(object sender, EventArgs e)
 //   {
 //       // aller chercher dynamiquement depuis les input/entrys et dans la fonction Login, aller API la DB à la collection User pour voir si OK
 //       // pas oublier de hasher le mdp
 //       // + check le layout Login au-dessus et résous problème labels/police ? en android
 //       // Algorithmer ici ou passer le username et password en arguments de Login ?
 //       _authService.Login();
 //       await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
 //   }

 //   private void ButtonLogoutClicked(object sender, EventArgs e)
 //   {
 //       _authService?.Logout();
 //       Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
 //       // + snackbar avec message you're succesfully logged out ?