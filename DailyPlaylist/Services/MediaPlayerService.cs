using MediaManager.Library;

namespace DailyPlaylist.Services
{
    public class MediaPlayerService
    {
        public List<Track> _tracks;
        public List<MediaItem> _mediaItems;
        public int _currentIndex = 0;

        public MediaPlayerService(List<Track> tracks, int index = 0)
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

            _currentIndex = index;
            CrossMediaManager.Current.AutoPlay = false;
            CrossMediaManager.Current.Play(_mediaItems);

            CrossMediaManager.Current.PositionChanged += async (sender, args) =>
            {
                if (args.Position.TotalSeconds >= 29)
                {
                    await PlayNextAsync();
                }
            };
        }

        public async Task PlayPauseTaskAsync(int index)
        {
            if (_currentIndex == index && CrossMediaManager.Current.IsPlaying())
            {
                await CrossMediaManager.Current.Pause();
            }
            else if (_currentIndex == index && !CrossMediaManager.Current.IsPlaying())
            {
                await CrossMediaManager.Current.Play();
            }
            else
            {
                _currentIndex = index;
                await CrossMediaManager.Current.PlayQueueItem(index);
            }
        }

        public async Task<Track> PlayNextAsync()
        {
            _currentIndex++;
            if (_currentIndex >= _mediaItems.Count)
            {
                _currentIndex = 0;
            }
            await CrossMediaManager.Current.PlayQueueItem(_currentIndex);
            return _tracks[_currentIndex];
        }

        public async Task<Track> PlayPreviousAsync()
        {
            if (CrossMediaManager.Current.Position.TotalSeconds < 3)
            {
                _currentIndex--;
                if (_currentIndex < 0) _currentIndex = _mediaItems.Count - 1;
            }
            else
            {
                await CrossMediaManager.Current.SeekTo(TimeSpan.Zero);
            }
            await CrossMediaManager.Current.PlayQueueItem(_currentIndex);
            return _tracks[_currentIndex];
        }
    }
}
