using BCrypt.Net;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DailyPlaylist.Services
{

    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private Lazy<HttpClient> _httpClient = new Lazy<HttpClient>();
        private readonly string _apiKey = "tviGbZrm0b4nfxTgVGvKB0skS4VIkV8xpjJ0qB5hcXZ9VwqAYDnXHPg6ZgAyXKh5";

        public User ActiveUser { get; set; }

        public bool IsAuthenticatedAsync()
        {

            var authState = Preferences.Default.Get<bool>(AuthStateKey, false);
            // on essaye d'aller chercher la valeur bool de la clé authstate dans les préférences,
            // si on ne l'obtient pas, ça retourne false par défaut
            // stocker ainsi le statut d'authentification dans les pref améliore l'expérience utilisateur car une fois logué,
            // l'utilisateur le reste, et ne devra plus se loguer systématiquement

            return authState;
        }

        public void Login(User user)
        {

            Preferences.Default.Set<bool>(AuthStateKey, true);
            ActiveUser = user;

        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
            ActiveUser = null;
        }

        public async Task<User> CreateAccountAsync(string email, string userPassword)
        {

            var client = _httpClient.Value;
            
            var existingUser = await RetrieveUserAsync(email);
            if (existingUser != null)
            {
                return null;
            }
            userPassword = HashPassword(userPassword);
            User createdUser = new User() { Email = email };

            var requestUri = "https://eu-central-1.aws.data.mongodb-api.com/app/data-httpe/endpoint/data/v1/action/insertOne";
            var payload = new
            {
                collection = "User",
                database = "DailyPlaylistDB",
                dataSource = "DailyPlaylistMongoDB",
                document = new
                {
                    _id = createdUser.Id,
                    email = createdUser.Email,
                    password = userPassword,
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
            client.DefaultRequestHeaders.Add("api-key", _apiKey);

            var response = await client.PostAsync(requestUri, content);

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

            var requestUri = "https://eu-central-1.aws.data.mongodb-api.com/app/data-httpe/endpoint/data/v1/action/findOne";
            var payload = new
            {
                collection = "User",
                database = "DailyPlaylistDB",
                dataSource = "DailyPlaylistMongoDB",
                filter = new { email = userEmail }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var client = _httpClient.Value;
      
            client.DefaultRequestHeaders.Clear();
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
            client.DefaultRequestHeaders.Add("api-key", _apiKey);

            var response = await client.PostAsync(requestUri, content);

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
       
            return null;
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

