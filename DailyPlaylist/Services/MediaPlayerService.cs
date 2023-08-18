using MediaManager.Library;

namespace DailyPlaylist.Services
{
    public class MediaPlayerService
    {
        public List<Track> _tracks;
        public List<MediaItem> _mediaItems;
        public int storedIndex;

        public MediaPlayerService(List<Track> tracks)
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

            CrossMediaManager.Current.Queue.Clear();
            CrossMediaManager.Current.Play(_mediaItems);
            if (storedIndex > 0 && storedIndex < CrossMediaManager.Current.Queue.Count) 
            {
                CrossMediaManager.Current.PlayQueueItem(storedIndex);
            }
            CrossMediaManager.Current.Stop();
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
                storedIndex = index;
                await CrossMediaManager.Current.PlayQueueItem(index);
            }
        }

        public async Task<Track> PlayNextAsync()
        {
            storedIndex++;
            if (storedIndex >= _mediaItems.Count)
            {
                storedIndex = 0;
            }
            await CrossMediaManager.Current.PlayQueueItem(storedIndex);
            return _tracks[storedIndex];
        }

        public async Task<Track> PlayPreviousAsync()
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
            return _tracks[storedIndex];
        }
    }
}
