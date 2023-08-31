using Newtonsoft.Json.Linq;
using DailyPlaylist.ViewModel;


namespace DailyPlaylist.Services
{

    public class AuthService : BaseViewModel
    {
        private const string AuthStateKey = "AuthState";
        private const string AuthUserKey = "AuthUser";
        private User _activeUser;
        private HttpService _httpService;


        // PROPERTIES //

        public User ActiveUser
        {
            get => _activeUser;
            set
            {
                SetProperty(ref _activeUser, value);
            }
        }

        // CONSTRUCTOR //

        public AuthService() 
        {
            _httpService = new HttpService();          
        }

        // METHODS //

        public bool IsAuthenticatedAsync()
        {

            var authState = Preferences.Default.Get<bool>(AuthStateKey, false);
            // on essaye d'aller chercher la valeur bool de la clé authstate dans les préférences,
            // si on ne l'obtient pas, ça retourne false par défaut
            // stocker ainsi le statut d'authentification dans les pref améliore l'expérience utilisateur car une fois logué,
            // l'utilisateur le reste, et ne devra plus se loguer systématiquement

            return authState;
        }

        public string WhoIsAuthenticatedAsync()
        {

            var authUser = Preferences.Default.Get<string>(AuthUserKey, null);
            return authUser;
        }

        public void Login(User user)
        {

            Preferences.Default.Set<bool>(AuthStateKey, true);
            Preferences.Default.Set<string>(AuthUserKey, user.Email);
            _activeUser = user;

        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
            Preferences.Default.Remove(AuthUserKey);
            _activeUser = null;
        }

        public async Task<User> CreateAccountAsync(string email, string userPassword)
        {
            
            var existingUser = await RetrieveUserAsync(email);
            if (existingUser != null)
            {
                return null;
            }
            userPassword = HashPassword(userPassword);
            User createdUser = new User() { Email = email };

            var action = "insertOne";
            var payload = new
            {
                collection = "User",
                database = "DailyPlaylist",
                dataSource = "DailyPlaylistMongoDB",
                document = new
                {
                    _id = createdUser.Id,
                    email = createdUser.Email,
                    password = userPassword,
                }
            };

            var response = await _httpService.MakeHttpRequestAsync(action, payload);

            if (response.IsSuccessStatusCode)
            {
                return createdUser;
            }
            else
            {
                // Afficher le statut de l'erreur dans le debug si échec
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error: {error}");
                return null;
            }
        }

        public async Task<User> LoginAsync(string userEmail, string userPassword)
        {

            var existingUser = await RetrieveUserAsync(userEmail);

            if (existingUser == null)
            {
                return null;
            }
            else
            {
                if (VerifyPassword(userPassword, existingUser.Password))
                {
                    return existingUser;
                }
                else { return null; }
            }
        }


        public async Task<User> RetrieveUserAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail)) { return null; }

            var action = "findOne";
            var payload = new
            {
                collection = "User",
                database = "DailyPlaylist",
                dataSource = "DailyPlaylistMongoDB",
                filter = new { email = userEmail }
            };

            var response = await _httpService.MakeHttpRequestAsync(action, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseData);
                var userJson = jsonObject["document"]?.ToString();
                Debug.WriteLine($"Retrieved User from server: {userJson}");

                if (userJson.Contains(userEmail)) // we make sure that we don't get a result such as {"document" : null}
                                                    // but rather an object containing the email address we were looking for, it's a double check
                {
                    var retrievedUser = JsonConvert.DeserializeObject<User>(userJson);

                    if (retrievedUser != null && retrievedUser is User)
                    {
                        return retrievedUser;
                    }
                }
            } 
            else
            {
                await SnackBarVM.ShowSnackBarAsync("There's a problem reaching the server for the moment, please retry", "Dismiss", () => { });
                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error: {error}");
            }
       
            return null;
        }

        // HASH METHODS //

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}

