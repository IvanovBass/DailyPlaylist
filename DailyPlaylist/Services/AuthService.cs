using DailyPlaylist.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Services
{
    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private const string ApiKey = "k5a64x19bpSZVMWCj2HgKBUJzp5CGkQSKrJy3TnDNtdMTdaTdpTVNxgvAUQtM0Zy";
        private const string BaseUrl = "https://eu-central-1.aws.data.mongodb-api.com/app/data-httpe/endpoint/data/v1/";
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
            _httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
        }

        public async Task<bool> IsAuthenticatedAsync(string username, string password)
        {
            // TODO: You will need an endpoint and logic that can check if a user with the given credentials exists.
            // This is a sample request; adapt it to your needs.
            var response = await _httpClient.PostAsync($"{BaseUrl}action/findOne",
                new StringContent(JsonConvert.SerializeObject(new
                {
                    collection = "User",
                    database = "DailyPlaylistDB",
                    dataSource = "DailyPlaylistMongoDB",
                    filter = new
                    {
                        username = username,
                        password = password  // NOTE: Never store passwords in plaintext, always use hashed & salted values.
                    },
                    projection = new { _id = 1 }
                }), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            return user != null;
        }

        public void Login()
        {
            Preferences.Default.Set<bool>(AuthStateKey, true);
        }

        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}

//public class AuthService
//{
//    private const string AuthStateKey = "AuthState";

//    public async Task<bool> IsAuthenticatedAsync()
//    {

//        await Task.Delay(2000);

//        var authState = Preferences.Default.Get<bool>(AuthStateKey, false); 
//        // on essaye d'aller chercher la valeur bool de la clé authstate dans les préférences,
//        // si on ne l'obtient pas, ça retourne false par défaut

//        return authState;
//    }

//    public void Login ( )
//    {

//        // IPreference Service, check compatibilité avec différentes platformes

//        Preferences.Default.Set<bool>(AuthStateKey, true );

//    }
//    public void Logout ( )
//    {
//        Preferences.Default.Remove(AuthStateKey);
//    }
//}


// je pense que c'est la police importée qui ne s'affiche pas dans les labels et sous Android ...
