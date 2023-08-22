using DailyPlaylist.ViewModel;
using MauiAppDI.Helpers;
using System.Runtime.CompilerServices;

namespace DailyPlaylist.Services
{
    public class AppSessionManager : IAppSessionManager
    {
        private User _authUser;
        private readonly AuthService _authService;
        private IPlaylistViewModel _playlistViewModel;
        private ISearchViewModel _searchViewModel;

        public AppSessionManager(AuthService authService)
        {
            _authService = authService;
        }

        public async Task StartNewSessionAsync()
        {
            EndSession();  // Dispose of the old session

            _playlistViewModel = new PlaylistViewModel(); // Recreate services
            _searchViewModel = new SearchViewModel();

            try
            {
                _authUser = await GetAuthenticatedUserAsync();

                if (_authUser != null)
                {
                    _playlistViewModel.Initialize(_authUser);
                    _searchViewModel.Initialize(_playlistViewModel);
                }
                else
                {
                    await SnackBarVM.ShowSnackBarAsync("Problem retrieving your details and playlists from the server. Please log out and log in again.", "Dismiss", () => { });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void EndSession()
        {
            _playlistViewModel = null; // Dispose of the services (they will be garbage collected)
            _searchViewModel = null;
        }

        private async Task<User> GetAuthenticatedUserAsync()
        {
            if (_authService.ActiveUser != null)
            {
                return _authService.ActiveUser;
            }

            string emailUser = _authService.WhoIsAuthenticatedAsync();
            return await _authService.RetrieveUserAsync(emailUser);
        }

    }


    public interface IAppSessionManager
    {
        void EndSession();

    }

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
}
