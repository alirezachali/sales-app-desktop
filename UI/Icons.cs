using System.Drawing.Drawing2D;

namespace sales_app_desktop.UI;

/// <summary>
/// ترسیم آیکون‌های وکتوریال ساده با Graphics (بدون فایل تصویر خارجی).
/// همه در یک مربع w×w از مبدأ (0,0) رسم می‌شوند.
/// </summary>
internal static class Icons
{
    public static void Draw(string name, Graphics g, int s, Color color)
    {
        var pen = new Pen(color, Math.Max(1.6f, s / 12f)) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        var brush = new SolidBrush(color);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var state = g.Save();

        switch (name)
        {
            case "sales": // بارکد/فاکتور
                DrawBarcode(g, pen, s);
                break;
            case "products": // جعبه
                DrawBox(g, pen, s);
                break;
            case "customers": // دو سرخوش
                DrawUsers(g, pen, s);
                break;
            case "search":
                DrawSearch(g, pen, s);
                break;
            case "plus":
                g.DrawLine(pen, s * .5f, s * .22f, s * .5f, s * .78f);
                g.DrawLine(pen, s * .22f, s * .5f, s * .78f, s * .5f);
                break;
            case "minus":
                g.DrawLine(pen, s * .22f, s * .5f, s * .78f, s * .5f);
                break;
            case "trash":
                g.DrawArc(pen, s * .25f, s * .2f, s * .5f, s * .6f, 0, 180);
                g.DrawLine(pen, s * .3f, s * .32f, s * .3f, s * .78f);
                g.DrawLine(pen, s * .5f, s * .32f, s * .5f, s * .78f);
                g.DrawLine(pen, s * .7f, s * .32f, s * .7f, s * .78f);
                g.DrawLine(pen, s * .22f, s * .28f, s * .78f, s * .28f);
                break;
            case "edit":
                g.DrawArc(pen, s * .35f, s * .35f, s * .3f, s * .3f, 90, 270);
                g.DrawLine(pen, s * .62f, s * .42f, s * .8f, s * .24f);
                g.DrawLine(pen, s * .5f, s * .72f, s * .8f, s * .42f);
                g.DrawLine(pen, s * .62f, s * .42f, s * .8f, s * .42f);
                break;
            case "print":
                g.DrawRectangle(pen, s * .2f, s * .35f, s * .6f, s * .35f);
                g.DrawRectangle(pen, s * .32f, s * .2f, s * .36f, s * .2f);
                g.FillRectangle(brush, s * .32f, s * .52f, s * .36f, s * .18f);
                break;
            case "close":
                g.DrawLine(pen, s * .28f, s * .28f, s * .72f, s * .72f);
                g.DrawLine(pen, s * .72f, s * .28f, s * .28f, s * .72f);
                break;
            case "min":
                g.DrawLine(pen, s * .3f, s * .6f, s * .7f, s * .6f);
                break;
            case "restore":
                g.DrawRectangle(pen, s * .3f, s * .3f, s * .4f, s * .4f);
                g.DrawRectangle(pen, s * .42f, s * .42f, s * .4f, s * .4f);
                break;
            case "dash":
                g.DrawLine(pen, s * .45f, s * .3f, s * .45f, s * .7f);
                g.DrawLine(pen, s * .6f, s * .3f, s * .6f, s * .7f);
                break;
            case "back":
                g.DrawArc(pen, s * .4f, s * .3f, s * .4f, s * .4f, -90, 180);
                g.DrawLine(pen, s * .5f, s * .4f, s * .5f, s * .7f);
                g.DrawLine(pen, s * .35f, s * .5f, s * .7f, s * .5f);
                break;
        }

        g.Restore(state);
        pen.Dispose();
        brush.Dispose();
    }

    private static void DrawBarcode(Graphics g, Pen p, int s)
    {
        float[] xs = { .2f, .3f, .34f, .44f, .52f, .58f, .7f };
        float w = s / 24f;
        foreach (var x in xs)
            g.DrawLine(p, x * s, .25f * s, x * s, .8f * s);
        g.DrawLine(p, .2f * s, .85f * s, .7f * s, .85f * s);
    }

    private static void DrawBox(Graphics g, Pen p, int s)
    {
        g.DrawPolygon(p, new[]
        {
            new PointF(.2f * s, .35f * s),
            new PointF(.5f * s, .2f * s),
            new PointF(.8f * s, .35f * s),
            new PointF(.8f * s, .72f * s),
            new PointF(.5f * s, .88f * s),
            new PointF(.2f * s, .72f * s),
        });
        g.DrawLine(p, .2f * s, .35f * s, .5f * s, .5f * s);
        g.DrawLine(p, .8f * s, .35f * s, .5f * s, .5f * s);
        g.DrawLine(p, .5f * s, .5f * s, .5f * s, .88f * s);
    }

    private static void DrawUsers(Graphics g, Pen p, int s)
    {
        g.DrawEllipse(p, .3f * s, .25f * s, .28f * s, .28f * s);
        g.DrawArc(p, .22f * s, .58f * s, .44f * s, .4f * s, 0, 180);
        g.DrawEllipse(p, .55f * s, .3f * s, .24f * s, .24f * s);
        g.DrawArc(p, .5f * s, .6f * s, .4f * s, .35f * s, 0, 180);
    }

    private static void DrawSearch(Graphics g, Pen p, int s)
    {
        g.DrawEllipse(p, .25f * s, .25f * s, .4f * s, .4f * s);
        g.DrawLine(p, .62f * s, .62f * s, .8f * s, .8f * s);
    }
}
