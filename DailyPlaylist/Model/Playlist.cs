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

        [MapTo("count")]
        public int Count => Tracks?.Count ?? 0;

        [MapTo("tracks")]
        [Required]
        public List<Track> Tracks { get; set; }

        [MapTo("user")]
        [Required]
        public User user { get; set; }

        [MapTo("dateCreation")]
        public DateTime DateCreation { get; set; }

        [MapTo("dateUpdated")]
        public DateTime DateUpdated { get; set; } = DateTime.Now;

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

