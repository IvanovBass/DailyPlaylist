﻿namespace DailyPlaylist.Services
{
    public class MediaPlayerService
    {
        private readonly List<Track> _tracks;
        private readonly List<string> _trackUris;
        private int _currentIndex = 0;

        public MediaPlayerService(List<Track> tracks, int index=0)
        {
            _tracks = tracks ?? throw new ArgumentNullException(nameof(tracks));
            _trackUris = _tracks.Select(t => t.Preview).ToList();

            _currentIndex = index;

            CrossMediaManager.Current.Play(_trackUris);
            CrossMediaManager.Current.Stop();  
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
            if (_currentIndex >= _trackUris.Count)
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
                if (_currentIndex < 0) _currentIndex = _trackUris.Count - 1;
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
