﻿using DailyPlaylist.Services;
using DailyPlaylist.ViewModel;
using DailyPlaylist.View;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace DailyPlaylist;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            // I did this based on this example : https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/alerts/snackbar?tabs=android
            // It basically enables the use of the MAUI snackbar

            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Rajdhani-Light.ttf", "RajdhaniLight");
                fonts.AddFont("Rajdhani-Medium.ttf", "RajdhaniMedium");
                fonts.AddFont("Rajdhani-Regular.ttf", "RajdhaniRegular");
                fonts.AddFont("Rajdhani-SemiBold.ttf", "RajdhaniSemiBold");
                fonts.AddFont("Rajdhani-Bold.ttf", "RajdhaniBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<HttpClient>();

        builder.Services.AddSingleton<AuthService>();

        builder.Services.AddSingleton<AppSessionManager>();

        builder.Services.AddTransient<LoadingPage>();

        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<ISearchViewModel, SearchViewModel>();

        builder.Services.AddTransient<IPlaylistViewModel, PlaylistViewModel>();

        builder.Services.AddTransient<SearchPage>();

        builder.Services.AddTransient<PlaylistPage>();

        builder.Services.AddTransient<LogoutViewModel>();

        return builder.Build();
	}
}
