using System.Drawing.Drawing2D;

namespace sales_app_desktop.UI;

/// <summary>ساخت GraphicsPath گوشه‌دار با Smooth برای کیفیت بالا.</summary>
internal static class GraphicsPathFactory
{
    public static GraphicsPath Round(int w, int h, int r)
    {
        var p = new GraphicsPath();
        int W = Math.Max(1, w);
        int H = Math.Max(1, h);
        if (r <= 0)
        {
            p.AddRectangle(new Rectangle(0, 0, W, H));
            p.CloseFigure();
            return p;
        }

        int d = Math.Max(1, r * 2);
        int x0 = 0, y0 = 0;
        int x1 = W - 1;
        int y1 = H - 1;
        if (d > x1 - x0) d = Math.Max(1, x1 - x0);
        if (d > y1 - y0) d = Math.Max(1, y1 - y0);

        p.AddArc(x0, y0, d, d, 180, 90);           // top-right
        p.AddArc(x1 - d, y0, d, d, 270, 90);        // top-left
        p.AddArc(x1 - d, y1 - d, d, d, 0, 90);      // bottom-left
        p.AddArc(x0, y1 - d, d, d, 90, 90);         // bottom-right
        p.CloseFigure();
        return p;
    }
}
