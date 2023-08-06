using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.Model
{
    public class Artist
    {
        public int Id { get; }

        public string Name { get; }

        public string Url { get; }

        public string PictureURL { get; }
        // Again, you may use 'size' = 'small', 'medium', 'big', 'xl'

        public string PictureSmall { get; }

        public string PictureMedium { get; }

        public string PictureBig { get; }

        public int NbAlbum { get; }

        public int NbFan { get; }

        public string Tracklist { get; }
    }
}