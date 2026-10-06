using System.Drawing.Drawing2D;
using sales_app_desktop.Data;

namespace sales_app_desktop.UI;

/// <summary>
/// پالت رنگ و فونت مشترک؛ روشن/تیره از تنظیمات خوانده می‌شود.
/// </summary>
internal static class Theme
{
    public static bool IsDark => AppSettings.Current.IsDark;

    // ── پالت (بر اساس تم انتخابی) ─────────────────────────
    public static Color Base => IsDark ? Color.FromArgb(12, 14, 19) : SystemColors.Control;
    public static Color Surface => IsDark ? Color.FromArgb(21, 25, 33) : SystemColors.ControlLightLight;
    public static Color SurfaceAlt => IsDark ? Color.FromArgb(30, 36, 48) : SystemColors.ControlLight;
    public static Color Border => IsDark ? Color.FromArgb(48, 56, 72) : SystemColors.ControlDark;
    public static Color BorderHi => Border;

    public static Color Accent => Color.FromArgb(0, 120, 212);       // آبی ویندوز
    public static Color AccentViolet => Accent;
    public static Color AccentOn => Color.White;

    public static Color Text => IsDark ? Color.FromArgb(234, 239, 247) : SystemColors.ControlText;
    public static Color TextDim => IsDark ? Color.FromArgb(147, 160, 180) : SystemColors.GrayText;
    public static Color Success => IsDark ? Color.FromArgb(53, 208, 160) : Color.FromArgb(16, 124, 16);
    public static Color Warn => IsDark ? Color.FromArgb(245, 165, 36) : Color.FromArgb(157, 93, 0);
    public static Color Danger => IsDark ? Color.FromArgb(255, 92, 122) : Color.FromArgb(196, 43, 28);

    // ── فونت سیستم ─────────────────────────────────────────
    public static readonly Font F = new("Segoe UI", 9.5f);
    public static readonly Font FS = new("Segoe UI", 9f);
    public static readonly Font FB = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FL = new("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font FXL = new("Segoe UI", 20f, FontStyle.Bold);

    // ── ابزار ──────────────────────────────────────────────

    /// <summary>تبدیل اعداد لاتین به فارسی.</summary>
    public static string FaDigits(string s)
    {
        var map = "0123456789-".ToCharArray();
        var fa = "۰۱۲۳۴۵۶۷۸۹-".ToCharArray();
        var sb = new System.Text.StringBuilder(s.Length);
        foreach (var ch in s)
            sb.Append(Array.IndexOf(map, ch) >= 0 ? fa[Array.IndexOf(map, ch)] : ch);
        return sb.ToString();
    }

    /// <summary>فرمت مبلغ با جداکننده هزارگان + اعداد فارسی + واحد پولی تنظیمات.</summary>
    public static string Toman(decimal v)
    {
        string raw = v.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
        return FaDigits(raw) + " " + AppSettings.Current.Currency;
    }

    /// <summary>تاریخ/ساعت شمسی بر اساس منطقه زمانی انتخابی.</summary>
    public static string NowFa()
    {
        var now = DateTime.UtcNow.AddHours(AppSettings.Current.TimeZoneOffsetHours);
        var cal = new System.Globalization.PersianCalendar();
        string date = $"{cal.GetYear(now)}/{cal.GetMonth(now):00}/{cal.GetDayOfMonth(now):00}";
        string time = now.ToString("HH:mm:ss");
        return FaDigits(date + "  " + time);
    }

    /// <summary>مسیر گوشه‌دار (برای کارت‌های ساده).</summary>
    internal static GraphicsPath MakeRound(int w, int h, int r) =>
        GraphicsPathFactory.Round(w, h, r);

    /// <summary>اعمال گوشه‌های گرد به یک کنترل با Region.</summary>
    public static void Round(Control c, int radius)
    {
        if (radius <= 0) { c.Region = null; return; }
        var g = GraphicsPathFactory.Round(c.Width, c.Height, radius);
        c.Region = new Region(g);
        g.Dispose();
    }

    /// <summary>اعمال فونت و RTL پیش‌فرض به یک کنترل.</summary>
    public static T Style<T>(this T c, Color bg, Color fg) where T : Control
    {
        c.BackColor = bg;
        c.ForeColor = fg;
        c.Font = F;
        c.RightToLeft = RightToLeft.Yes;
        return c;
    }
}
