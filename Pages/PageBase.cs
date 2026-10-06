using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// پایه مشترک صفحه‌ها: نوار سربرگ (عنوان + شرح) و ناحیه بدنه برای محتوا.
/// </summary>
public abstract class PageBase : UserControl
{
    protected Panel Body { get; }
    internal CardPanel Header { get; } = new();
    protected Label TitleLabel { get; }
    protected Label SubtitleLabel { get; }

    protected PageBase()
    {
        BackColor = Theme.Surface;
        RightToLeft = RightToLeft.Yes;

        Header = new CardPanel
        {
            BackColor = Theme.Base,
            Height = 60,
            ShowBorder = false,
        };

        TitleLabel = new Label
        {
            Font = Theme.FL,
            ForeColor = Theme.Text,
            AutoSize = true,
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        SubtitleLabel = new Label
        {
            Font = Theme.FS,
            ForeColor = Theme.TextDim,
            AutoSize = true,
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };

        Header.Controls.Add(TitleLabel);
        Header.Controls.Add(SubtitleLabel);

        Body = new Panel
        {
            BackColor = Theme.Surface,
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
        };

        Controls.Add(Body);
        Controls.Add(Header);
    }

    protected void SetHeader(string title, string subtitle)
    {
        TitleLabel.Text = title;
        SubtitleLabel.Text = subtitle;
        RightAlignHeader();
    }

    /// <summary>برچسب‌های سربرگ را به لبه راست (RTL) می‌چسباند.</summary>
    private void RightAlignHeader()
    {
        int right = Header.Width - 16;
        TitleLabel.Location = new Point(Math.Max(8, right - TitleLabel.PreferredWidth), 8);
        SubtitleLabel.Location = new Point(Math.Max(8, right - SubtitleLabel.PreferredWidth), 36);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (Header is null || Body is null) return;
        Header.SetBounds(0, 0, Width, 60);
        Body.SetBounds(0, 60, Width, Math.Max(0, Height - 60));
        RightAlignHeader();
    }
}
