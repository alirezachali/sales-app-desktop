using System.Drawing.Drawing2D;

namespace sales_app_desktop.UI;

/// <summary>
/// پالت رنگ و فونت مشترک — تم روشن و استاندارد ویندوز، بدون تزئینات.
/// </summary>
internal static class Theme
{
    // ── پالت روشن ──────────────────────────────────────────
    public static readonly Color Base        = SystemColors.Control;
    public static readonly Color Surface     = SystemColors.ControlLightLight; // سفید
    public static readonly Color SurfaceAlt  = SystemColors.ControlLight;
    public static readonly Color Border      = SystemColors.ControlDark;
    public static readonly Color Accent      = Color.FromArgb(0, 120, 212);    // آبی ویندوز
    public static readonly Color AccentOn    = Color.White;
    public static readonly Color Text        = SystemColors.ControlText;
    public static readonly Color TextDim     = SystemColors.GrayText;
    public static readonly Color Success     = Color.FromArgb(16, 124, 16);
    public static readonly Color Danger      = Color.FromArgb(196, 43, 28);

    // سازگاری با کدهای قبلی
    public static readonly Color BorderHi = Border;
    public static readonly Color Warn = Color.FromArgb(157, 93, 0);
    public static readonly Color AccentViolet = Accent;

    // ── فونت سیستم ─────────────────────────────────────────
    public static readonly Font F   = new("Segoe UI", 9.5f);
    public static readonly Font FS  = new("Segoe UI", 9f);
    public static readonly Font FB  = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FL  = new("Segoe UI", 14f, FontStyle.Bold);
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

    /// <summary>فرمت تومان با جداکننده هزارگان + اعداد فارسی.</summary>
    public static string Toman(decimal v)
    {
        string raw = v.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
        return FaDigits(raw) + " تومان";
    }

    /// <summary>تاریخ/ساعت شمسی برای سربرگ.</summary>
    public static string NowFa()
    {
        var cal = new System.Globalization.PersianCalendar();
        string date = $"{cal.GetYear(DateTime.Now)}/{cal.GetMonth(DateTime.Now):00}/{cal.GetDayOfMonth(DateTime.Now):00}";
        string time = DateTime.Now.ToString("HH:mm:ss");
        return FaDigits(date + "  " + time);
    }

    /// <summary>مسیر گوشه‌دار (فقط برای کارت‌های ساده که هنوز استفاده می‌شود).</summary>
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
