using BCrypt.Net;
using System.Text;

namespace DailyPlaylist.Services
{

    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "pUpHMasJWKjApIU8HZwTHToI4HsLm2NyrEjoW79HKNQ5PFJpyW8ZAB2RpJKLf2Vq";

        public async Task<bool> IsAuthenticatedAsync()
        {

            await Task.Delay(2000);

            var authState = Preferences.Default.Get<bool>(AuthStateKey, false);
            // on essaye d'aller chercher la valeur bool de la clé authstate dans les préférences,
            // si on ne l'obtient pas, ça retourne false par défaut
            // stocker ainsi le statut d'authentification dans les pref améliore l'expérience utilisateur car une fois logué,
            // l'utilisateur le reste, et ne devra plus se loguer systématiquement

            return authState;
        }

        public void Login()
        {

            Preferences.Default.Set<bool>(AuthStateKey, true);

        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }

        public async Task<bool> CreateAccountAsync(string email, string password)
        {
            
            password = HashPassword(password);

            using (_httpClient)
            {
                var requestUri = "https://eu-central-1.aws.data.mongodb-api.com/app/data-httpe/endpoint/data/v1/action/findOne";
                var payload = new
                {
                    collection = "User",
                    database = "DailyPlaylistDB",
                    dataSource = "DailyPlaylistMongoDB",
                    projection = new
                    {
                        _id = Guid.NewGuid().ToString(),
                        email = email,
                        password = password, 
                        playlistIds = new List<string>() 
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                _httpClient.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
                _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
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

