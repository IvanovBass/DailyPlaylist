using MediaManager.Library;

namespace DailyPlaylist.Services
{
    public class MediaPlayerService
    {
        public List<Track> _tracks;
        public List<MediaItem> _mediaItems;
        public int storedIndex = 0;

        public MediaPlayerService(List<Track> tracks, bool autoPlay)
        {
            _tracks = tracks ?? throw new ArgumentNullException(nameof(tracks));

            _mediaItems = _tracks.Select(t => new MediaItem(t.Link)
            {
                Title = t.Title,
                Artist = t.Artist.Name,
                AlbumImageUri = t.Album.Cover,
                Album = t.Album.Title,
                MediaUri = t.Preview,
                // ....  Go check the definition by right-clicking <MediaItem> Many metadata/properties may be written in the MediaItem object,
                //  If I was to create a dedicated Page "Player" displaying the Track playing only, and its details, these metadata could be useful
            }).ToList();

            CrossMediaManager.Current.Play(_mediaItems);
            if (!autoPlay)
            {
                CrossMediaManager.Current.Pause();
            }
            //if (storedIndex > 0 && storedIndex < CrossMediaManager.Current.Queue.Count) 
            //{
            //    CrossMediaManager.Current.PlayQueueItem(storedIndex);
            //}
            CrossMediaManager.Current.PositionChanged += async (sender, args) =>
            {
                if (args.Position.TotalSeconds >= 28)
                {
                    await PlayNextAsync();
                }
            };
        }

        public async Task PlayPauseTaskAsync(int index)
        {
  
            storedIndex = index;
            if (CrossMediaManager.Current.Queue.CurrentIndex == index && CrossMediaManager.Current.IsPlaying())
            {
                await CrossMediaManager.Current.Pause();
            }
            else if (CrossMediaManager.Current.Queue.CurrentIndex == index && !CrossMediaManager.Current.IsPlaying())
            {
                await CrossMediaManager.Current.Play();
            }
            else
            {
                await CrossMediaManager.Current.PlayQueueItem(index);
            }
        }

        public async Task PlayNextAsync()
        {
            storedIndex++;
            if (storedIndex >= _mediaItems.Count)
            {
                storedIndex = 0;
            }
            await CrossMediaManager.Current.PlayQueueItem(storedIndex);
        }

        public async Task<int> PlayPreviousAsync()
        {
            if (CrossMediaManager.Current.Position.TotalSeconds < 3)
            {
                storedIndex--;
                if (storedIndex < 0) storedIndex = _mediaItems.Count - 1;
            }
            else
            {
                await CrossMediaManager.Current.SeekTo(TimeSpan.Zero);
            }
            await CrossMediaManager.Current.PlayQueueItem(storedIndex);
            return storedIndex;
        }
    }
}
