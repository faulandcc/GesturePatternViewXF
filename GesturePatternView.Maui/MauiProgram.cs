using SkiaSharp.Views.Maui.Controls.Hosting;

namespace GesturePatternView.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
            .UseSkiaSharp()
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
			});

		return builder.Build();
	}
}
