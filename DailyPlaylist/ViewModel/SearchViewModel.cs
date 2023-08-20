﻿using DailyPlaylist.Services;
using DailyPlaylist.View;
using MauiAppDI.Helpers;
using System.Linq;

namespace DailyPlaylist.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        private PlaylistViewModel _playlistViewModel;
        private ObservableCollection<Track> _searchResults;
        public int preStoredIndex = 0; 
        public MediaPlayerService mediaPlayerService;
        private string _searchQuery;
        private Track _selectedTrack;
        private string _trackName = "Song";
        private string _artistName = "Artist";
        private string _albumCover = "music_notes2.png";
        private Lazy<HttpClient> _httpClient = new Lazy<HttpClient>();
        private bool _isLoading;

        // PROPERTIES //
        public ObservableCollection<Track> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
            }
        }

        public string TrackName
        {
            get => _trackName;
            set => SetProperty(ref _trackName, string.IsNullOrEmpty(value) ? "Song" : value);
        }

        public string ArtistName
        {
            get => _artistName;
            set => SetProperty(ref _artistName, string.IsNullOrEmpty(value) ? "Artist" : value);
            // By doing so we no longer need :  OnPropertyChanged(nameof(ArtistName));
        }

        public string AlbumCover
        {
            get => _albumCover;
            set => SetProperty(ref _albumCover, string.IsNullOrEmpty(value) ? "music_notes2.png" : value);
        }

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                SetProperty(ref _selectedTrack, value);
                if (value != null)
                {
                    int selectedIndex = SearchResults.IndexOf(value);
                    preStoredIndex = selectedIndex;

                    AlbumCover = value.Album?.CoverMedium;
                    TrackName = value.Title;
                    ArtistName = value.Artist?.Name;
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // COMMANDS //

        public ICommand PlayPauseCommand { get; }
        public ICommand PlayFromCollectionViewCommand { get; }
        public ICommand SetFavoriteCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }  
        public ICommand ItemSelectedCommand { get; }

        public ICommand SearchCommand { get; }


        // CONSTRUCTOR //

        public SearchViewModel(PlaylistViewModel playlistViewModel)
        {
            SearchCommand = new Command(PerformSearch);
            SearchResults = new ObservableCollection<Track>();

            PlayPauseCommand = new Command(async track =>
            {
                if (SearchResults == null || !SearchResults.Any())
                {
                    await SnackBarVM.ShowSnackBarAsync("No track to play", "Dismiss", () => { });
                    return;
                }
                else
                {
                    if (NavigationState.LastVisitedPage != nameof(SearchPage))
                    {
                        CrossMediaManager.Current.Queue.Clear();
                        mediaPlayerService = new MediaPlayerService(SearchResults.ToList(), true);
                        await mediaPlayerService.PlayPauseTaskAsync(SearchResults.IndexOf(SelectedTrack));
                        NavigationState.LastVisitedPage = nameof(SearchPage);
                    }
                    else
                    {
                        await mediaPlayerService.PlayPauseTaskAsync(SearchResults.IndexOf(SelectedTrack));
                    }
                }
            });

            PlayFromCollectionViewCommand = new Command<Track>(async track =>
            {
                SelectedTrack = track;
                preStoredIndex = SearchResults.IndexOf(SelectedTrack);

                if (NavigationState.LastVisitedPage != nameof(SearchPage))
                {
                    CrossMediaManager.Current.Queue.Clear();
                    mediaPlayerService = new MediaPlayerService(SearchResults.ToList(), true);
                    await mediaPlayerService.PlayPauseTaskAsync(preStoredIndex);
                    NavigationState.LastVisitedPage = nameof(SearchPage);
                }
                else
                {
                    await mediaPlayerService.PlayPauseTaskAsync(preStoredIndex);
                }
            });

            _playlistViewModel = playlistViewModel;

            _playlistViewModel.SelectedPlaylistChanged += LoadSelectedFavoriteTrackUris;

            SetFavoriteCommand = new Command<Track>(async track =>
            {
                if (_playlistViewModel.SelectedPlaylist == null)
                {
                    await SnackBarVM.ShowSnackBarShortAsync("No playlist selected. Please select a playlist first.", "OK", () => { });
                    return;
                }

                track.Favorite = !track.Favorite;

                NavigationState.LastVisitedPage = nameof(SearchPage);

                if (track.Favorite)
                {
                    _playlistViewModel.SelectedPlaylist.DeezerTrackIds.Add(track.Id);
                    await SnackBarVM.ShowSnackBarShortAsync("Song succesfully added to playlist '" + _playlistViewModel.SelectedPlaylist.Name + "' !", "OK", () => { });
                }
                else
                {
                    _playlistViewModel.SelectedPlaylist.DeezerTrackIds.Remove(track.Id);
                    await SnackBarVM.ShowSnackBarShortAsync("Song removed from playlist '" + _playlistViewModel.SelectedPlaylist.Name + "' !", "OK", () => { });
                }
            });



            NextCommand = new Command<Track>(async track =>
            {
                if (SearchResults == null || !SearchResults.Any())
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracklist to be forwarded", "Dismiss", () => { });
                    return;
                }
                else
                {
                    var currentIndex = SearchResults.IndexOf(SelectedTrack);
                    preStoredIndex = currentIndex;
                    preStoredIndex++;
                    if (preStoredIndex >= SearchResults.Count)
                    {
                        preStoredIndex = 0;
                    }
                    SelectedTrack = SearchResults[preStoredIndex];

                    if (NavigationState.LastVisitedPage != nameof(SearchPage))
                    {
                        
                    }
                    else
                    {
                        await mediaPlayerService.PlayNextAsync();
                    }  
                }
            });

            PreviousCommand = new Command<Track>(async track =>
            {
                if (SearchResults == null || !SearchResults.Any())
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracklist to be backwarded", "Dismiss", () => { });
                    return;
                } 
                else
                {
                    if (NavigationState.LastVisitedPage != nameof(SearchPage))
                    {
                        preStoredIndex--;
                        if (preStoredIndex < 0)
                        {
                            preStoredIndex = SearchResults.Count - 1;
                        }
                        SelectedTrack = SearchResults[preStoredIndex];
                    }
                    else
                    {
                        preStoredIndex = await mediaPlayerService.PlayPreviousAsync();
                        SelectedTrack = SearchResults[preStoredIndex];
                    }
                }
            });

            ItemSelectedCommand = new Command<Track>(track =>
            {
                SelectedTrack = track;
            });


            CrossMediaManager.Current.PositionChanged += (sender, args) =>
            {
                if (args.Position.TotalSeconds >= 28)
                {
                    if (NavigationState.LastVisitedPage == nameof(SearchPage))
                    {
                        HandleTrackFinishedSVM();
                    }  
                }
            };

            LogoutViewModel.OnLogout += Reset;

        }

        public void HandleTrackFinishedSVM()
        {
            if (SearchResults == null) return;

            var currentIndex = CrossMediaManager.Current.Queue.CurrentIndex;
            currentIndex++;
            if (currentIndex >= SearchResults.Count())
            {
                currentIndex = 0;
            }
            preStoredIndex = currentIndex;
            if (preStoredIndex >= 0 && preStoredIndex < SearchResults.Count())
            {
                SelectedTrack = SearchResults[preStoredIndex];
            }
        }

        private async void PerformSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;

            IsLoading = true;

            var client = _httpClient.Value;

            try
            {
                var jsonResponse = await client.GetStringAsync($"https://api.deezer.com/search/track?q={SearchQuery}&limit=30");
                var searchData = JsonConvert.DeserializeObject<SearchData>(jsonResponse);
                SearchResults = new ObservableCollection<Track>(searchData.Data);
                await CrossMediaManager.Current.Pause();
                CrossMediaManager.Current.Queue.Clear();
                mediaPlayerService = new MediaPlayerService(searchData.Data, false);

                if (SearchResults.Any())
                {
                    SelectedTrack = SearchResults[0];
                    LoadSelectedFavoriteTrackUris();
                }
                else
                {
                    await SnackBarVM.ShowSnackBarAsync("No tracks found for your search query", "Dismiss", () => { });
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while searching: {ex.Message}");
                IsLoading = false;
                await SnackBarVM.ShowSnackBarAsync("Connexion can't be made, please retry", "Dismiss", () => { });
            }
            finally
            {
                await Task.Delay(2000);
                IsLoading = false;
            }
        }

        public class SearchData
        {
            public List<Track> Data { get; set; }
            //... other properties or objects to catch eventually
        }

        public void LoadSelectedFavoriteTrackUris()
        {
            if (_playlistViewModel.SelectedPlaylist == null) { return; }

            var favoriteTrackIds = _playlistViewModel.SelectedPlaylist.DeezerTrackIds;

            foreach (var track in SearchResults)
            {
                if (favoriteTrackIds.Contains(track.Id))
                {
                    track.Favorite = true;
                }
                else
                {
                    track.Favorite = false;
                }
            }
        }

        public void Reset()
        {
            SearchResults = new ObservableCollection<Track>();
            preStoredIndex = 0;
            mediaPlayerService = null;
            SearchQuery = string.Empty;
            SelectedTrack = null;
            TrackName = "Song";
            ArtistName = "Artist";
            AlbumCover = "music_notes2.png";
            IsLoading = false;
        }
    }
}

// https://api.deezer.com/search?q={your_query}
// https://api.deezer.com/search/artist?q={your_query}
//  https://api.deezer.com/search/track?q={your_query}
// ...

