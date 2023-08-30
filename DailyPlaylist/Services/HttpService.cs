using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MauiAppDI.Helpers;
using dotenv.net;
using System.Collections;

namespace DailyPlaylist.Services
{
    public class HttpService
    {
        private string _musicTrack = "97Zg6E2yTLG0KuXjZ1Fb59iMFVQ02zjJDzfwZN6lHjCxNjwDAWsLnrZRR5keyiSR";
        private HttpClient _client;
        private string _requestUri = "https://westeurope.azure.data.mongodb-api.com/app/data-bqkhe/endpoint/data/v1/action/";

        public HttpService() 
        {
            _client = ServiceHelper.GetService<HttpClient>();

            //DotEnv.Load();

            //string musicTrack = Environment.GetEnvironmentVariable("MUSIC_TRACK");

            //// _apiKey = musicTrack;

            //foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            //{
            //    Debug.WriteLine($"Key = {de.Key}, Value = {de.Value}");
            //}
        }

        public async Task<HttpResponseMessage> MakeHttpRequestAsync(string action, object payload)
        {

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "*");
            _client.DefaultRequestHeaders.Add("api-key", _musicTrack);

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await _client.PostAsync(_requestUri+action, content);

        }




    }
}
