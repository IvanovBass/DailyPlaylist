using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DailyPlaylist.Model
{
    public class User : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("email")]
        [Required]
        public string Email { get; set; }

        [MapTo("username")]
        [Required]
        public string Username { get; set; }

        [MapTo("password")]
        [Required]
        public string Password { get; set; }

        // Realm-backed collections should only have a getter
        [MapTo("playlists")]
        public IList<int> Playlists { get; }

        // Realm-backed collections should only have a getter
        [MapTo("favorites")]
        public IList<int> Favorites { get; }

        [MapTo("_partition")]
        [Required]
        public string Partition { get; set; }
    }
}

//public class User
//{
//    public Guid Id { get; private set; }
//    public string Username { get; set; }
//    public string Email { get; set; }
//    public string Password { get; set; }

//    public List<Track> Favorites { get; set; }

//    public List<Playlist> Playlists { get; set; }

//    public User()
//    {
//        Id = Guid.NewGuid();
//    }
//}