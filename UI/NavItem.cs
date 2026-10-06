using System.Drawing.Drawing2D;

namespace sales_app_desktop.UI;

/// <summary>دکمه ناوبری ساده سایدبار: آیکون + متن، با حالت فعال.</summary>
internal class NavItem : Control
{
    private readonly string _icon;
    private bool _active, _hover;

    public NavItem(string key, string icon, string text)
    {
        Key = key;
        _icon = icon;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Theme.Base;
        ForeColor = Theme.Text;
        Font = Theme.F;
        Cursor = Cursors.Hand;
        Text = text;
    }

    public string Key { get; set; } = "";

    public void SetActive(bool on) { _active = on; Invalidate(); }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        if (_active)
        {
            using var bg = new SolidBrush(Theme.SurfaceAlt);
            g.FillRectangle(bg, ClientRectangle);
            using var bar = new SolidBrush(Theme.Accent);
            g.FillRectangle(bar, Width - 4, 4, 4, Height - 8); // نشانگر فعال (RTL: لبه راست)
        }
        else if (_hover)
        {
            using var bg = new SolidBrush(Theme.SurfaceAlt);
            g.FillRectangle(bg, ClientRectangle);
        }

        // آیکون سمت راست (RTL)
        int s = 20;
        using var iconPen = new Pen(_active ? Theme.Accent : Theme.TextDim, 1.8f)
        { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        g.TranslateTransform(Width - 12 - s, (Height - s) / 2f);
        Icons.Draw(_icon, g, s, _active ? Theme.Accent : Theme.TextDim);
        g.ResetTransform();

        // متن کنار آیکون
        TextRenderer.DrawText(g, Text, Font,
            new Rectangle(8, 0, Width - s - 28, Height),
            _active ? Theme.Accent : Theme.Text,
            TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.RightToLeft);
    }
}
