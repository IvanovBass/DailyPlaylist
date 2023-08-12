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

        public async Task<bool> IsAuthenticatedAsync(string usernam, string password)
        {

            await Task.Delay(2000);

            var authState = Preferences.Default.Get<bool>(AuthStateKey, false);
            // on essaye d'aller chercher la valeur bool de la clé authstate dans les préférences,
            // si on ne l'obtient pas, ça retourne false par défaut

            return authState;
        }

        public void Login()
        {

            // IPreference Service, check compatibilité avec différentes platformes

            Preferences.Default.Set<bool>(AuthStateKey, true);

        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}

