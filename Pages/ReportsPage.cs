using System.Globalization;
using Microsoft.EntityFrameworkCore;
using sales_app_desktop.Data;
using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// صفحه گزارش فروش: تفکیک روزانه/ماهانه با تعداد فاکتور، فروش، سود و تخفیف.
/// سود هر ردیف = (قیمت فروش لحظه‌ای − قیمت تمام‌شده کالا) × تعداد − سهم تخفیف.
/// </summary>
public class ReportsPage : PageBase
{
    private sealed record PeriodOption(string Key, string Display);

    private sealed class ReportRow
    {
        public string Period { get; set; } = "";
        public int InvoiceCount { get; set; }
        public int ItemCount { get; set; }
        public decimal GrossSales { get; set; }
        public decimal Discount { get; set; }
        public decimal NetSales { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
    }

    private readonly DataGridView _grid = new();
    private readonly ComboBox _period = new();
    private readonly Label _summary = new()
    {
        Text = "",
        Font = Theme.FB,
        ForeColor = Theme.Text,
        AutoSize = true,
        BackColor = Color.Transparent,
        Anchor = AnchorStyles.Top | AnchorStyles.Right,
    };
    private readonly Button _btnRefresh = new()
    {
        Text = "به‌روزرسانی",
        Width = 110,
        Height = 32,
        UseVisualStyleBackColor = true,
    };

    public ReportsPage()
    {
        SetHeader("گزارش فروش و سود", "خلاصه فروش و سود به تفکیک روز یا ماه");

        // ── نوار ابزار ──────────────────────────────────────
        var toolbar = new Panel
        {
            BackColor = Theme.Surface,
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(8),
        };

        _period.DropDownStyle = ComboBoxStyle.DropDownList;
        _period.Font = Theme.F;
        _period.Width = 160;
        _period.DataSource = new List<PeriodOption>
        {
            new("daily", "روزانه"),
            new("monthly", "ماهانه"),
        };
        _period.DisplayMember = nameof(PeriodOption.Display);
        _period.ValueMember = nameof(PeriodOption.Key);

        _btnRefresh.Click += (_, _) => Reload();

        var periodLabel = new Label
        {
            Text = "بازه:",
            AutoSize = true,
            Font = Theme.F,
            Anchor = AnchorStyles.Left,
        };

        var leftRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 340,
            FlowDirection = FlowDirection.LeftToRight,
            RightToLeft = RightToLeft.Yes,
            BackColor = Color.Transparent,
            WrapContents = false,
        };
        periodLabel.Margin = new Padding(3, 8, 3, 0);
        _period.Margin = new Padding(2, 4, 2, 0);
        _btnRefresh.Margin = new Padding(8, 2, 2, 0);
        leftRow.Controls.Add(periodLabel);
        leftRow.Controls.Add(_period);
        leftRow.Controls.Add(_btnRefresh);

        toolbar.Controls.Add(_summary);
        toolbar.Controls.Add(leftRow);
        _summary.Location = new Point(toolbar.Width - 10, 16); // راست‌چین (RTL)

        // ── جدول ────────────────────────────────────────────
        _grid.ReadOnly = true;
        ModernGrid.Apply(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.Columns.Add(ModernGrid.Col("بازه", 130, DataGridViewContentAlignment.MiddleRight, nameof(ReportRow.Period), 16));
        _grid.Columns.Add(ModernGrid.Col("تعداد فاکتور", 90, DataGridViewContentAlignment.MiddleCenter, nameof(ReportRow.InvoiceCount), 10));
        _grid.Columns.Add(ModernGrid.Col("تعداد قلم کالا", 90, DataGridViewContentAlignment.MiddleCenter, nameof(ReportRow.ItemCount), 10));
        _grid.Columns.Add(ModernGrid.Col("فروش ناخالص", 130, DataGridViewContentAlignment.MiddleRight, nameof(ReportRow.GrossSales), 14));
        _grid.Columns.Add(ModernGrid.Col("تخفیف", 110, DataGridViewContentAlignment.MiddleRight, nameof(ReportRow.Discount), 10));
        _grid.Columns.Add(ModernGrid.Col("فروش خالص", 130, DataGridViewContentAlignment.MiddleRight, nameof(ReportRow.NetSales), 14));
        _grid.Columns.Add(ModernGrid.Col("بهای تمام‌شده", 130, DataGridViewContentAlignment.MiddleRight, nameof(ReportRow.Cost), 13));
        _grid.Columns.Add(ModernGrid.Col("سود", 130, DataGridViewContentAlignment.MiddleRight, nameof(ReportRow.Profit), 13));

        _grid.CellFormatting += (_, ev) =>
        {
            if (ev.Value is null) return;
            if (ev.ColumnIndex >= 3 && ev.ColumnIndex <= 7)
            {
                if (decimal.TryParse(ev.Value.ToString(), out decimal v))
                    ev.Value = Theme.Toman(v);
            }
            else if (ev.ColumnIndex is 1 or 2)
            {
                ev.Value = Theme.FaDigits(ev.Value.ToString() ?? "");
            }
        };

        Body.Controls.Add(_grid);
        Body.Controls.Add(toolbar);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Reload();
    }

    private void Reload()
    {
        string key = _period.SelectedValue as string ?? "daily";

        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        var items = db.SaleItems.AsNoTracking()
            .Include(i => i.Sale)
            .Include(i => i.Product)
            .Where(i => i.Sale != null)
            .ToList();

        var rows = items
            .GroupBy(i => key == "daily"
                ? i.Sale.Date.Date
                : new DateTime(i.Sale.Date.Year, i.Sale.Date.Month, 1))
            .Select(g =>
            {
                decimal gross = g.Sum(x => x.LineTotal);
                decimal disc = g.Select(x => x.Sale).Distinct().Sum(s => s.Discount);
                decimal cost = g.Sum(x => x.Product.Cost * x.Quantity);
                decimal net = Math.Max(0, gross - disc);
                return new ReportRow
                {
                    Period = key == "daily"
                        ? Theme.FaDigits(g.Key.ToString("yyyy/MM/dd"))
                        : Theme.FaDigits(g.Key.ToString("yyyy/MM")),
                    InvoiceCount = g.Select(x => x.SaleId).Distinct().Count(),
                    ItemCount = g.Sum(x => x.Quantity),
                    GrossSales = gross,
                    Discount = disc,
                    NetSales = net,
                    Cost = cost,
                    Profit = net - cost,
                };
            })
            .OrderByDescending(r => r.Period)
            .Take(200)
            .ToList();

        _grid.DataSource = rows;

        // خلاصه کلی
        decimal totalProfit = rows.Sum(r => r.Profit);
        decimal totalNet = rows.Sum(r => r.NetSales);
        int totalInvoices = rows.Sum(r => r.InvoiceCount);
        _summary.Text = $"جمع در این بازه: {Theme.FaDigits(totalInvoices.ToString())} فاکتور  |  " +
                        $"فروش خالص: {Theme.Toman(totalNet)}  |  سود: {Theme.Toman(totalProfit)}";
        RightAlignSummary();
    }

    private void RightAlignSummary()
    {
        _summary.Location = new Point(Math.Max(8, Width - _summary.PreferredWidth - 20), 16);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        RightAlignSummary();
    }
}
