// using MediaManager;
using DailyPlaylist.Services;
using DailyPlaylist.View;
using Microsoft.Extensions.Logging;

namespace DailyPlaylist;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Rajdhani-Light.ttf", "RajdhaniLight");
                fonts.AddFont("Rajdhani-Medium.ttf", "RajdhaniMedium");
                fonts.AddFont("Rajdhani-Regular.ttf", "RajdhaniRegular");
                fonts.AddFont("Rajdhani-SemiBold.ttf", "RajdhaniSemiBold");
                fonts.AddFont("Rajdhani-Bold.ttf", "RajdhaniBold");
                // CrossMediaManager.Current.Init();
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddTransient<AuthService>();

		builder.Services.AddTransient<LoadingPage>();

        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddSingleton(new HttpClient());



        return builder.Build();
	}
}
