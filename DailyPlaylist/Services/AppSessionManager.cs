using DailyPlaylist.View;
using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.Services
{
    public class AppSessionManager
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private AuthService _authService;
        private IServiceScope _currentScope;


        // CONSTRUCTOR //
        public AppSessionManager(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _authService = ServiceHelper.GetService<AuthService>();
        }

        // PROPERTIES //

        public User AuthUser;
        public PlaylistViewModel PlaylistViewModel;
        public SearchViewModel SearchViewModel;

        // METHODS //

        public async void StartNewSession()
        {

            _currentScope = _scopeFactory.CreateScope();

            _authService = ServiceHelper.GetService<AuthService>();

            CrossMediaManager.Current.Queue.Clear();

            if (_authService.ActiveUser is null)
            {
                AuthUser = await CheckUserLoggedIn(_authService);
            }
            else
            {
                AuthUser = _authService.ActiveUser;
            }

            PlaylistViewModel = new PlaylistViewModel(AuthUser);

            SearchViewModel = new SearchViewModel(PlaylistViewModel);

        }

        public T GetService<T>()
        {
            return _currentScope.ServiceProvider.GetRequiredService<T>();
        }

        public void DisposeCurrentScope()
        {
            _currentScope?.Dispose();
            PlaylistViewModel = null;
            SearchViewModel = null;
        }

        public async Task<User> CheckUserLoggedIn(AuthService authService)
        {
            try
            {
                if (authService != null)
                {
                    string emailUser = authService.WhoIsAuthenticatedAsync();
                    User authUser = await authService.RetrieveUserAsync(emailUser);

                    if (authUser != null && authUser is User)
                    {
                         return authUser;
                    }
                    else
                    {
                        await SnackBarVM.ShowSnackBarAsync("Problem retrieving your details and playlists from server, please log out and log in again", "Dismiss", () => { });
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }

 }
