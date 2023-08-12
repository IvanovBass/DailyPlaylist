using MediaManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.ViewModel
{
    public class TrackViewModel : BaseViewModel
    {
        private Track _currentTrack;

        public Track CurrentTrack
        {
            get => _currentTrack;
            set
            {
                _currentTrack = value;
                OnPropertyChanged();
            }
        }

        //When the page is navigated to, or at some trigger, you can load the track and play its preview:
        //var viewModel = BindingContext as TrackViewModel;
        //await viewModel.LoadTrackByIdAsync(3135556);
        //await viewModel.PlayPreviewAsync();

        public async Task LoadTrackByIdAsync(int trackId)
        {
            var httpClient = DependencyService.Get<HttpClient>();
            var jsonResponse = await httpClient.GetStringAsync($"https://api.deezer.com/track/{trackId}");

            var trackData = JsonConvert.DeserializeObject<Track>(jsonResponse);
            CurrentTrack = trackData;
        }

        public async Task PlayPreviewAsync()
        {
            if (CurrentTrack != null)
            {
                await CrossMediaManager.Current.Play(CurrentTrack.Preview);
            }
        }
    }

}
