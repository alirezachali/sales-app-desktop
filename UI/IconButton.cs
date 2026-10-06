using System.Drawing.Drawing2D;

namespace sales_app_desktop.UI;

/// <summary>آیکون مربعی برای دکمه‌های title bar و دیالوگ‌ها.</summary>
internal sealed class IconButton : Control
{
    private string _icon;
    public bool Hot { get; set; }
    private bool _hover, _down;

    public IconButton(string icon, int size = 34)
    {
        _icon = icon;
        Size = new Size(size, size);
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
    }

    public void SetIcon(string icon) { _icon = icon; Invalidate(); }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _down = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _down = false; Invalidate(); base.OnMouseUp(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        Color fg;
        if (_down) fg = Hot ? Theme.Danger : Theme.Accent;
        else if (_hover) fg = Hot ? Color.White : Theme.Text;
        else fg = Theme.TextDim;

        if (_hover || _down)
        {
            var p = Theme.MakeRound(Width, Height, 8);
            g.FillPath(new SolidBrush(_down ? Theme.Surface : Theme.SurfaceAlt), p);
            p.Dispose();
        }
        int s = Math.Min(Width, Height) - 10;
        g.TranslateTransform((Width - s) / 2f, (Height - s) / 2f);
        Icons.Draw(_icon, g, s, fg);
        g.ResetTransform();
    }
}
