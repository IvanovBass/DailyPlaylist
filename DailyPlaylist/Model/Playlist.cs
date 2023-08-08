using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Playlist : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("name")]
        [Required]
        public string Name { get; set; }

        [MapTo("description")]
        public string Description { get; set; }

        public int Count => Tracks?.Count() ?? 0;

        // Realm-backed collections should only have a getter
        [MapTo("tracks")]
        public IList<int> Tracks { get; }

        [MapTo("user")]
        [Required]
        public string userId { get; set; }

        [MapTo("dateCreation")]
        public DateTimeOffset DateCreation { get; set; }

        [MapTo("dateUpdated")]
        public DateTimeOffset DateUpdated { get; set; } = DateTimeOffset.Now;

        [MapTo("_partition")]
        [Required]
        public string Partition { get; set; }
    }
}
//public class Playlist
//{
//    public Guid Id { get; private set; }

//    public User User { get; set; }
//    public string Name { get; set; }
//    public string Description { get; set; }
//    public int Count { get; set; }
//    public List<Track> Tracks { get; set; }
//    public DateTime DateCreation { get; set; }

//    public DateTime DateUpdated { get; set; } = DateTime.Now;

//    public Playlist()
//    {
//        Id = Guid.NewGuid();
//    }
//} //   Hello World !!!

