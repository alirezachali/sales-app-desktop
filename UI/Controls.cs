using System.Drawing.Drawing2D;

namespace sales_app_desktop.UI;

// ─────────────────────────────────────────────────────────────
//  کنترل‌های ساده استاندارد
// ─────────────────────────────────────────────────────────────

/// <summary>پنل ساده با رنگ پس‌زمینه (بدون Region و ترسیم سفارشی).</summary>
internal class CardPanel : Panel
{
    public int Radius { get; set; }
    public Color Fill { get; set; } = Theme.Surface;
    public Color Line { get; set; } = Theme.Border;
    public bool ShowBorder { get; set; } = true;

    public CardPanel()
    {
        BackColor = Theme.Surface;
    }
}

/// <summary>سبک دکمه.</summary>
internal enum ButtonVariant { Primary, Ghost, Danger, Success }

/// <summary>دکمه استاندارد ویندوز با رنگ‌بندی بر اساس نقش.</summary>
internal class RButton : Button
{
    public ButtonVariant Variant { get; set; } = ButtonVariant.Ghost;
    public int Radius { get; set; }

    public RButton()
    {
        UseVisualStyleBackColor = true;
        FlatStyle = FlatStyle.System; // ظاهر استاندارد ویندوز
        Font = Theme.F;
        Cursor = Cursors.Hand;
        Height = 32;
        ApplyVariant();
    }

    private void ApplyVariant()
    {
        switch (Variant)
        {
            case ButtonVariant.Primary:
                BackColor = Theme.Accent;
                ForeColor = Theme.AccentOn;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                break;
            case ButtonVariant.Success:
                BackColor = Theme.Success;
                ForeColor = Color.White;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                break;
            case ButtonVariant.Danger:
                BackColor = Theme.Danger;
                ForeColor = Color.White;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                break;
            default:
                BackColor = Theme.Base;
                ForeColor = Theme.Text;
                UseVisualStyleBackColor = true;
                FlatStyle = FlatStyle.System;
                break;
        }
    }
}

/// <summary>فیلد ورود ساده: TextBox با ظاهر استاندارد.</summary>
internal class RField : Panel
{
    private readonly TextBox _box;
    private readonly Label _ph;
    public string Placeholder { get; set; } = "";
    public bool FocusOnMouseClick { get; set; }

    /// <summary>رویداد کلیدهای فیلد؛ برای اسکنر بارکد (تایپ + Enter) استفاده می‌شود.</summary>
    public event KeyEventHandler? BarKey;

    public RField()
    {
        BackColor = Theme.Surface;
        Height = 34;

        _box = new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Dock = DockStyle.Fill,
            ForeColor = Theme.Text,
            Font = Theme.F,
            RightToLeft = RightToLeft.Yes,
        };
        _ph = new Label
        {
            Font = Theme.FS,
            ForeColor = Theme.TextDim,
            AutoSize = true,
            BackColor = Color.Transparent,
            Location = new Point(10, 9),
            Text = "",
        };
        _box.TextChanged += (_, _) => _ph.Visible = string.IsNullOrEmpty(_box.Text);
        _box.KeyDown += (_, e) => BarKey?.Invoke(this, e);

        Controls.Add(_box);
        Controls.Add(_ph);

        MouseClick += (_, _) => _box.Focus();
        Resize += (_, _) => _ph.Location = new Point(10, (Height - _ph.Height) / 2);
    }

    public string Text
    {
        get => _box.Text;
        set => _box.Text = value;
    }

    public bool ReadOnly
    {
        get => _box.ReadOnly;
        set => _box.ReadOnly = value;
    }

    public int MaxLength
    {
        get => _box.MaxLength;
        set => _box.MaxLength = value;
    }

    public bool Multiline
    {
        get => _box.Multiline;
        set
        {
            _box.Multiline = value;
            if (value) _box.Dock = DockStyle.Fill;
        }
    }

    public new void SelectAll() => _box.SelectAll();

    public new bool Focus() => _box.Focus();

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (_box is null || _ph is null) return;
        _ph.Visible = string.IsNullOrEmpty(_box.Text);
        _ph.Location = new Point(10, (Height - _ph.Height) / 2);
    }
}

/// <summary>برچسب راهنمای فیلد.</summary>
internal class LabelField : Label
{
    public LabelField()
    {
        Font = Theme.FS;
        ForeColor = Theme.TextDim;
        AutoSize = true;
    }
}
