using System.Drawing;

namespace IDMActClean.UI
{
    public enum AppTheme { Light, Dark, Auto }

    public static class ThemeTokens
    {
        public static readonly Color LightBg      = Color.FromArgb(245, 247, 250);
        public static readonly Color LightSurface = Color.FromArgb(255, 255, 255);
        public static readonly Color LightBorder  = Color.FromArgb(220, 224, 230);
        public static readonly Color LightText    = Color.FromArgb(30,  34,  40);
        public static readonly Color LightSubText = Color.FromArgb(100, 110, 125);
        public static readonly Color LightAccent  = Color.FromArgb(37,  99,  235);
        public static readonly Color LightBtnText = Color.White;
        public static readonly Color LightOk      = Color.FromArgb(2,   132, 199);
        public static readonly Color LightWarn    = Color.FromArgb(217, 119, 6);
        public static readonly Color LightErr     = Color.FromArgb(220, 38,  38);
        public static readonly Color LightInfo    = Color.FromArgb(37,  99,  235);

        public static readonly Color DarkBg       = Color.FromArgb(18,  18,  24);
        public static readonly Color DarkSurface  = Color.FromArgb(28,  30,  38);
        public static readonly Color DarkBorder   = Color.FromArgb(50,  54,  66);
        public static readonly Color DarkText     = Color.FromArgb(230, 232, 240);
        public static readonly Color DarkSubText  = Color.FromArgb(140, 148, 164);
        public static readonly Color DarkAccent   = Color.FromArgb(96,  165, 250);
        // Intentionally dark (= DarkBg), not white: it's the text color on buttons filled with
        // the light-blue DarkAccent, where dark text reads better than white (~5.5:1 contrast,
        // passes WCAG AA). Revisit this if DarkAccent is ever changed to a darker color.
        public static readonly Color DarkBtnText  = Color.FromArgb(18,  18,  24);
        public static readonly Color DarkOk       = Color.FromArgb(56,  189, 248);
        public static readonly Color DarkWarn     = Color.FromArgb(251, 191, 36);
        public static readonly Color DarkErr      = Color.FromArgb(248, 113, 113);
        public static readonly Color DarkInfo     = Color.FromArgb(96,  165, 250);

        public static bool IsDarkMode(AppTheme theme)
        {
            if (theme == AppTheme.Light) return false;
            if (theme == AppTheme.Dark)  return true;
            return IsSystemDarkMode();
        }

        private static bool IsSystemDarkMode()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    var val = key?.GetValue("AppsUseLightTheme");
                    return val is int i && i == 0;
                }
            }
            catch { return false; }
        }

        public static Palette Resolve(AppTheme theme)
        {
            bool dark = IsDarkMode(theme);
            return new Palette
            {
                Bg      = dark ? DarkBg      : LightBg,
                Surface = dark ? DarkSurface : LightSurface,
                Border  = dark ? DarkBorder  : LightBorder,
                Text    = dark ? DarkText    : LightText,
                SubText = dark ? DarkSubText : LightSubText,
                Accent  = dark ? DarkAccent  : LightAccent,
                BtnText = dark ? DarkBtnText : LightBtnText,
                Ok      = dark ? DarkOk      : LightOk,
                Warn    = dark ? DarkWarn    : LightWarn,
                Err     = dark ? DarkErr     : LightErr,
                Info    = dark ? DarkInfo    : LightInfo,
            };
        }
    }

    public class Palette
    {
        public Color Bg      { get; set; }
        public Color Surface { get; set; }
        public Color Border  { get; set; }
        public Color Text    { get; set; }
        public Color SubText { get; set; }
        public Color Accent  { get; set; }
        public Color BtnText { get; set; }
        public Color Ok      { get; set; }
        public Color Warn    { get; set; }
        public Color Err     { get; set; }
        public Color Info    { get; set; }
    }
}
