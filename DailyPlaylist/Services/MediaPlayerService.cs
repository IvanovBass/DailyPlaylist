using DailyPlaylist.ViewModel;
using MediaManager.Library;
namespace DailyPlaylist.Services
{
    public static class MediaPlayerService
    {
        public static List<Track> _tracks;
        public static List<MediaItem> _mediaItems;
        public static int storedIndex = 0;
        public static Action OnItemChanged;

        static MediaPlayerService()
        {
            CrossMediaManager.Current.PositionChanged += async (sender, args) =>
            {
                if (args.Position.TotalSeconds >= 28)
                {
                    await PlayNextAsync();
                }
            };

            CrossMediaManager.Current.MediaItemChanged += (sender, args) =>
            {
                OnItemChanged?.Invoke();
            };
        }

        public static void Initialize(List<Track> tracks, bool autoPlay)
        {
            _tracks = tracks ?? throw new ArgumentNullException(nameof(tracks));

            _mediaItems = _tracks.Select(t => new MediaItem(t.Link)
            {
                Title = t.Title,
                Artist = t.Artist.Name,
                AlbumImageUri = t.Album.Cover,
                Album = t.Album.Title,
                MediaUri = t.Preview,
            }).ToList();

            CrossMediaManager.Current.Play(_mediaItems);
            if (!autoPlay)
            {
                CrossMediaManager.Current.Pause();
            }
        }

        public static async Task PlayPauseTaskAsync(int index, bool applyIndex = false)
        {
            if (!CrossMediaManager.Current.Queue.HasCurrent)
            {
                await SnackBarVM.ShowSnackBarAsync("Select a track or playlist first...", "Dismiss", () => { });
                return;
            }
            if (!applyIndex)
            {
                if (CrossMediaManager.Current.IsPlaying())
                {
                    await CrossMediaManager.Current.Pause();
                }
                else
                {
                    await CrossMediaManager.Current.Play();
                }
            }
            else
            {
                storedIndex = index;
                await CrossMediaManager.Current.PlayQueueItem(index);
            }
        }

        public static async Task PlayNextAsync()
        {
            if (!CrossMediaManager.Current.Queue.HasCurrent)
            {
                await SnackBarVM.ShowSnackBarAsync("Select a track or playlist first...", "Dismiss", () => { });
                return;
            }
            if (!CrossMediaManager.Current.Queue.HasNext)
            {
                try
                {
                    await CrossMediaManager.Current.PlayQueueItem(0);
                }
                catch
                {
                    await SnackBarVM.ShowSnackBarAsync("No track forward", "Dismiss", () => { });
                    return;
                }
            }
            else
            {
                await CrossMediaManager.Current.PlayNext();
            }
        }

        public static async Task PlayPreviousAsync()
        {
            if (!CrossMediaManager.Current.Queue.HasCurrent)
            {
                await SnackBarVM.ShowSnackBarAsync("Select a track or playlist first...", "Dismiss", () => { });
                return;
            }
            if (CrossMediaManager.Current.Position.TotalSeconds < 3)
            {
                if (!CrossMediaManager.Current.Queue.HasPrevious)
                {
                    try
                    {
                        await CrossMediaManager.Current.PlayQueueItem(_mediaItems.Count - 1);
                    }
                    catch
                    {
                        await SnackBarVM.ShowSnackBarAsync("No previous track to play", "Dismiss", () => { });
                        return;
                    }
                }
                else
                {
                    await CrossMediaManager.Current.PlayPrevious();
                }
            }
            else
            {
                await CrossMediaManager.Current.SeekTo(TimeSpan.Zero);
            }
        }

        public static void ResetProperties()
        {
            _tracks = null;
            _mediaItems = null;
            storedIndex = 0;
            OnItemChanged = null;
        }

    }
}
