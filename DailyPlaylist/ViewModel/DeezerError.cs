namespace DailyPlaylist.ViewModel
{
    public class DeezerError
    {
        public DeezerErrorDetail Error { get; set; }
    }

    public class DeezerErrorDetail
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }

}
