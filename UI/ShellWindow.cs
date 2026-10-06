using sales_app_desktop.Data;
using sales_app_desktop.Pages;

namespace sales_app_desktop.UI;

/// <summary>
/// پنجره اصلی برنامه: فرم استاندارد ویندوز با نوار عنوان خودِ سیستم،
/// منوی کناری ساده و ناحیه محتوا.
/// </summary>
internal class ShellWindow : Form
{
    private readonly Panel _content = new();
    private readonly Panel _sidebar = new();
    private readonly Label _clock = new();
    private readonly List<NavItem> _nav = [];
    private readonly Dictionary<string, UserControl> _pages = new();

    public ShellWindow()
    {
        Text = "سیستم فروش و انبار — فروشگاه من";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new(1100, 700);
        MinimumSize = new(920, 600);
        BackColor = Theme.Base;
        Font = Theme.F;
        RightToLeft = RightToLeft.Yes;

        BuildSidebar();

        _content.Dock = DockStyle.Fill;
        _content.BackColor = Theme.Surface;

        var pages = new UserControl[]
        {
            new SalesPage(),
            new ProductsPage(),
            new CustomersPage(),
            new ReportsPage(),
        };
        _pages["sales"] = pages[0];
        _pages["products"] = pages[1];
        _pages["customers"] = pages[2];
        _pages["reports"] = pages[3];
        _content.Controls.AddRange(pages);

        Controls.Add(_content);
        Controls.Add(_sidebar);

        // ساعت شمسی در نوار عنوان (کنار آیکون‌های خود ویندوز نمایش داده نمی‌شود؛ پایین سایدبار می‌گذاریم)
        _clock.Text = Theme.NowFa();
        _clock.Font = Theme.FS;
        _clock.ForeColor = Theme.TextDim;
        _clock.AutoSize = true;
        _clock.Dock = DockStyle.Bottom;
        _clock.Padding = new Padding(0, 6, 0, 8);
        _clock.TextAlign = ContentAlignment.MiddleCenter;
        _sidebar.Controls.Add(_clock);
        _clock.BringToFront();

        // تایمر ساعت
        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (_, _) => _clock.Text = Theme.NowFa();
        timer.Start();

        ShowPage("sales");
    }

    private void BuildSidebar()
    {
        _sidebar.Width = 210;
        _sidebar.Dock = DockStyle.Right;
        _sidebar.BackColor = Theme.Base;
        _sidebar.Padding = new Padding(8, 12, 8, 8);

        var brand = new Label
        {
            Text = "فروشگاه من",
            Font = Theme.FL,
            ForeColor = Theme.Text,
            AutoSize = true,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0, 8, 0, 2),
        };
        var sub = new Label
        {
            Text = "سیستم فروش و انبار",
            Font = Theme.FS,
            ForeColor = Theme.TextDim,
            AutoSize = true,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0, 0, 0, 10),
        };
        _sidebar.Controls.Add(sub);
        _sidebar.Controls.Add(brand);
        brand.BringToFront();

        var defs = new (string Key, string Icon, string Title)[]
        {
            ("sales", "sales", "ثبت فروش"),
            ("products", "products", "محصولات و انبار"),
            ("customers", "customers", "مشتریان"),
            ("reports", "products", "گزارش فروش و سود"),
        };

        int y = 70;
        foreach (var d in defs)
        {
            var item = new NavItem(d.Key, d.Icon, d.Title) { Height = 40 };
            item.Left = 10;
            item.Top = y;
            item.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            item.Width = _sidebar.Width - 20;
            item.Click += (_, _) => ShowPage(d.Key);
            _sidebar.Controls.Add(item);
            _nav.Add(item);
            y += 46;
        }

        // دکمه خروج در پایین
        var exit = new Button
        {
            Text = "خروج از سیستم",
            Height = 34,
            Dock = DockStyle.Bottom,
            BackColor = Theme.Base,
            UseVisualStyleBackColor = true,
        };
        exit.Click += (_, _) => Close();
        _sidebar.Controls.Add(exit);
        exit.BringToFront();
        _clock.BringToFront();
    }

    public void ShowPage(string key)
    {
        if (!_pages.TryGetValue(key, out var page)) return;
        foreach (Control c in _content.Controls)
            c.Visible = false;
        page.Visible = true;
        page.Dock = DockStyle.Fill;
        page.BringToFront();
        foreach (var n in _nav)
            n.SetActive(n.Key == key);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
    }
}
