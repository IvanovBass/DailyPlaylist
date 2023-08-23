using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;

namespace DailyPlaylist.Services
{
    public class AppSessionManager
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private User _authUser;
        private AuthService _authService;
        private IServiceScope _currentScope;


        // CONSTRUCTOR //
        public AppSessionManager(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _authService = ServiceHelper.GetService<AuthService>();
        }

        // PROPERTIES //

        public PlaylistViewModel PlaylistViewModel { get; set; }

        public SearchViewModel SearchViewModel { get; set; }

        // METHODS //

        public async void StartNewSession()
        {

            _currentScope = _scopeFactory.CreateScope();

            _authService = ServiceHelper.GetService<AuthService>();

            CrossMediaManager.Current.Queue.Clear();

            if (_authService.ActiveUser is null)
            {
                _authUser = await CheckUserLoggedIn(_authService);
            }
            else
            {
                _authUser = _authService.ActiveUser;
            }

            PlaylistViewModel = new PlaylistViewModel();
            
            await Task.Delay(500);

            PlaylistViewModel.Initialize(_authUser);

            await Task.Delay(2000);

            SearchViewModel = new SearchViewModel();
            // Register the SearchViewModel in the DI scope for other services to consume
            // _currentScope.ServiceProvider.GetRequiredService<ISearchViewModel>().Initialize(playlistViewModel);

            await Task.Delay(1000);

            SearchViewModel.Initialize(PlaylistViewModel);

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

// VM INTERFACES //

    public interface IPlaylistViewModel
    {
        public void Initialize(User authUser);
        // ... other members ...
    }

    public interface ISearchViewModel
    {
        void Initialize(PlaylistViewModel playlistViewModel);
        // ... other members ...
    }
