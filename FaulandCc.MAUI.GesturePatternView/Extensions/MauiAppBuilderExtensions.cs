using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FaulandCc.MAUI.GesturePatternView.Extensions;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseFaulandGesturePattern(this MauiAppBuilder self)
    {
        return self.UseSkiaSharp();
    }
}