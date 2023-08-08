using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DailyPlaylist.Model
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<Track> Favorites { get; set; }

        public List<Playlist> Playlists { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}