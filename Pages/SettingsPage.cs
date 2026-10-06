using System.Globalization;
using sales_app_desktop.Data;
using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// صفحه تنظیمات: تم روشن/تیره، لوگو، منطقه زمانی، واحد پولی و نام فروشگاه.
/// تغییرات با دکمه «ذخیره» اعمال و برنامه مجدداً راه‌اندازی می‌شود.
/// </summary>
public class SettingsPage : PageBase
{
    private readonly AppSettings _s = AppSettings.Current;

    private readonly ComboBox _theme = new();
    private readonly TextBox _storeName = new();
    private readonly TextBox _currency = new();
    private readonly ComboBox _timezone = new();
    private readonly PictureBox _logoPreview = new();
    private string? _pickedLogo;
    private bool _dirty;

    private static readonly (string Name, double Offset)[] Zones =
    {
        ("تهران (UTC+3:30)", 3.5),
        ("دبی (UTC+4)", 4.0),
        ("استانبول (UTC+3)", 3.0),
        ("مسکو (UTC+3)", 3.0),
        ("دهلی (UTC+5:30)", 5.5),
        ("لندن (UTC+0)", 0.0),
        ("برلین (UTC+1)", 1.0),
        ("نیویورک (UTC-5)", -5.0),
        ("لس‌آنجلس (UTC-8)", -8.0),
        ("سیدنی (UTC+10)", 10.0),
        ("توکیو (UTC+9)", 9.0),
    };

    public SettingsPage()
    {
        SetHeader("تنظیمات", "تم، لوگو، منطقه زمانی، واحد پولی و نام فروشگاه");

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            RightToLeft = RightToLeft.Yes,
            Padding = new Padding(10),
            BackColor = Theme.Surface,
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;
        void AddRow(Control label, Control value, int height = 40)
        {
            if (label is Label lbl)
            {
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleRight;
            }
            else
            {
                label.Dock = DockStyle.Fill;
            }
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            form.Controls.Add(value, 1, row);
            form.Controls.Add(label, 0, row);
            row++;
        }

        // تم
        _theme.DropDownStyle = ComboBoxStyle.DropDownList;
        _theme.Font = Theme.F;
        _theme.Width = 220;
        _theme.DataSource = new List<NamedOption>
        {
            new("dark", "تیره"),
            new("light", "روشن (استاندارد ویندوز)"),
        };
        _theme.DisplayMember = nameof(NamedOption.Display);
        _theme.ValueMember = nameof(NamedOption.Key);
        _theme.SelectedValue = _s.IsDark ? "dark" : "light";
        _theme.SelectedIndexChanged += (_, _) => _dirty = true;
        _theme.HandleCreated += (_, _) =>
            _theme.SelectedIndex = _s.IsDark ? 0 : 1;
        AddRow(new Label { Text = "تم برنامه:", Font = Theme.F }, _theme);

        // نام فروشگاه
        _storeName.Text = _s.StoreName;
        _storeName.Font = Theme.F;
        _storeName.Dock = DockStyle.Fill;
        _storeName.TextChanged += (_, _) => _dirty = true;
        AddRow(new Label { Text = "نام فروشگاه:", Font = Theme.F }, _storeName);

        // واحد پولی
        _currency.Text = _s.Currency;
        _currency.Font = Theme.F;
        _currency.Dock = DockStyle.Fill;
        _currency.TextChanged += (_, _) => _dirty = true;
        AddRow(new Label { Text = "واحد پولی:", Font = Theme.F }, _currency);

        // منطقه زمانی
        _timezone.DropDownStyle = ComboBoxStyle.DropDownList;
        _timezone.Font = Theme.F;
        _timezone.Width = 220;
        _timezone.DataSource = Zones.Select(z => new NamedOption(z.Name, z.Offset.ToString(CultureInfo.InvariantCulture))).ToList();
        _timezone.DisplayMember = nameof(NamedOption.Display);
        _timezone.ValueMember = nameof(NamedOption.Key);
        var matchIdx = Array.FindIndex(Zones, z => Math.Abs(z.Offset - _s.TimeZoneOffsetHours) < 0.01);
        _timezone.SelectedIndexChanged += (_, _) => _dirty = true;
        _timezone.HandleCreated += (_, _) =>
            _timezone.SelectedIndex = matchIdx >= 0 ? matchIdx : 0;
        AddRow(new Label { Text = "منطقه زمانی:", Font = Theme.F }, _timezone);

        // لوگو
        _logoPreview.Size = new Size(72, 72);
        _logoPreview.SizeMode = PictureBoxSizeMode.Zoom;
        _logoPreview.BorderStyle = BorderStyle.FixedSingle;
        _logoPreview.BackColor = Theme.Base;
        LoadLogoPreview(_s.LogoPath);
        _pickedLogo = _s.LogoPath;

        var logoButtons = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent,
        };
        var btnPick = new RButton { Text = "انتخاب لوگو…", Width = 130, Height = 32 };
        var btnClear = new RButton { Text = "حذف لوگو", Width = 110, Height = 32 };
        btnPick.Click += (_, _) => PickLogo();
        btnClear.Click += (_, _) => { _pickedLogo = null; LoadLogoPreview(null); _dirty = true; };
        logoButtons.Controls.Add(btnPick);
        logoButtons.Controls.Add(btnClear);

        var logoCell = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent,
            RightToLeft = RightToLeft.Yes,
        };
        logoCell.Controls.Add(_logoPreview);
        logoCell.Controls.Add(logoButtons);
        AddRow(new Label { Text = "لوگو:", Font = Theme.F }, logoCell, 90);

        // دکمه ذخیره
        var btnSave = new RButton { Text = "ذخیره و اعمال", Width = 140, Height = 38, Variant = ButtonVariant.Primary };
        btnSave.Click += (_, _) => SaveAll();
        AddRow(new Label { Text = "" }, new FlowLayoutPanel
        {
            Controls = { btnSave },
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            BackColor = Color.Transparent,
            RightToLeft = RightToLeft.Yes,
        }, 50);

        var filler = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface };
        Body.Controls.Add(filler);
        Body.Controls.Add(form);
    }

    private sealed record NamedOption(string Key, string Display);

    private void LoadLogoPreview(string? path)
    {
        try
        {
            _logoPreview.Image = File.Exists(path ?? "") ? Image.FromFile(path!) : null;
            _logoPreview.BackColor = _logoPreview.Image is null ? Theme.Base : Color.White;
        }
        catch { _logoPreview.Image = null; }
    }

    private void PickLogo()
    {
        using var dlg = new OpenFileDialog
        {
            Title = "انتخاب لوگو",
            Filter = "تصویر|*.png;*.jpg;*.jpeg;*.bmp;*.ico|همه فایل‌ها|*.*",
        };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _pickedLogo = dlg.FileName;
            LoadLogoPreview(_pickedLogo);
            _dirty = true;
        }
    }

    private void SaveAll()
    {
        try
        {
            bool themeChanged = _s.IsDark != Equals(_theme.SelectedValue, "dark");
            bool logoChanged = _s.LogoPath != _pickedLogo;

            _s.IsDark = Equals(_theme.SelectedValue, "dark");
            _s.StoreName = string.IsNullOrWhiteSpace(_storeName.Text) ? "فروشگاه من" : _storeName.Text.Trim();
            _s.Currency = string.IsNullOrWhiteSpace(_currency.Text) ? "تومان" : _currency.Text.Trim();
            if (_timezone.SelectedItem is NamedOption opt)
                _s.TimeZoneOffsetHours = Zones.FirstOrDefault(z => z.Name == opt.Display).Offset;
            _s.TimeZoneName = _timezone.Text;
            _s.LogoPath = _pickedLogo;
            AppSettings.Save();
            _dirty = false;

            string msg = "تنظیمات ذخیره شد.";
            if (themeChanged || logoChanged)
                msg += "\nبرای اعمال تم/لوگو، برنامه مجدداً راه‌اندازی می‌شود.";
            MessageBox.Show(msg, "تنظیمات", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (themeChanged || logoChanged)
                RestartApp();
        }
        catch (Exception ex)
        {
            MessageBox.Show("خطا در ذخیره تنظیمات:\n" + ex.Message, "تنظیمات",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>راه‌اندازی مجدد مطمئن برنامه (بدون وابستگی به Application.Restart).</summary>
    private void RestartApp()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                UseShellExecute = true,
            });
            Application.Exit();
        }
        catch
        {
            // اگر راه‌اندازی مجدد ممکن نبود، کاربر خودش برنامه را باز کند
            MessageBox.Show("لطفاً برنامه را دستی بسته و دوباره باز کنید.", "تنظیمات",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
