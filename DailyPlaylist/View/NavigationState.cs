using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlaylist.View
{
    public static class NavigationState
    {

        public static string LastVisitedPage { get; set; } = "";
        public static string LastPlayerUsed { get; set; } = "";

        public static bool refreshFavoritesNeeded { get; set; } = false;

        public static bool playlistChanged { get; set; } = false;

    }
}
