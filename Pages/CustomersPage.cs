using Microsoft.EntityFrameworkCore;
using sales_app_desktop.Data;
using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// صفحه مشتریان: لیست، جستجو، افزودن/ویرایش با اعتبار/بدهی.
/// </summary>
public class CustomersPage : PageBase
{
    /// <summary>ردیف نمایش‌دادنی: مشتری + شمارش خریدها.</summary>
    private sealed class CustomerRow
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public string Phone { get; init; } = "";
        public string Address { get; init; } = "";
        public decimal Balance { get; init; }
        public int SaleCount { get; init; }
        public Customer Customer { get; init; } = new();
    }

    private readonly DataGridView _grid = new();
    private readonly RField _search = new();
    private RButton _btnAdd;
    private RButton _btnEdit;
    private RButton _btnDel;

    public CustomersPage()
    {
        SetHeader("مشتریان", "مدیریت پرونده مشتریان و اعتبار آن‌ها");

        // â”€â”€ Ù†ÙˆØ§Ø± Ø§Ø¨Ø²Ø§Ø± â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        var toolbar = new Panel
        {
            BackColor = Theme.Surface,
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(8),
        };

        _search.Placeholder = "جستجو: نام یا شماره تماس…";
        _search.Dock = DockStyle.Fill;

        _btnAdd = new RButton { Text = "افزودن مشتری", Width = 130, Height = 32, Variant = ButtonVariant.Primary };
        _btnAdd.Click += (_, _) => OpenEditor(null);
        _btnEdit = new RButton { Text = "ویرایش", Width = 100, Height = 32 };
        _btnEdit.Click += (_, _) => OpenEditor(Selected());
        _btnDel = new RButton { Text = "حذف", Width = 100, Height = 32, Variant = ButtonVariant.Danger };
        _btnDel.Click += (_, _) => Delete(Selected());

        var btnRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 330,
            FlowDirection = FlowDirection.LeftToRight,
            RightToLeft = RightToLeft.Yes,
            BackColor = Color.Transparent,
            WrapContents = false,
        };
        _btnAdd.Margin = _btnEdit.Margin = _btnDel.Margin = new Padding(2, 0, 2, 0);
        btnRow.Controls.Add(_btnAdd);
        btnRow.Controls.Add(_btnEdit);
        btnRow.Controls.Add(_btnDel);

        toolbar.Controls.Add(_search);
        toolbar.Controls.Add(btnRow);
        _grid.ReadOnly = true;
        ModernGrid.Apply(_grid);
        _grid.Columns.Add(ModernGrid.Col("نام", 200, DataGridViewContentAlignment.MiddleRight, nameof(CustomerRow.Name), 20));
        _grid.Columns.Add(ModernGrid.Col("شماره تماس", 150, DataGridViewContentAlignment.MiddleLeft, nameof(CustomerRow.Phone), 16));
        _grid.Columns.Add(ModernGrid.Col("آدرس", 200, DataGridViewContentAlignment.MiddleRight, nameof(CustomerRow.Address), 20));
        _grid.Columns.Add(ModernGrid.Col("اعتبار/بدهی", 130, DataGridViewContentAlignment.MiddleRight, nameof(CustomerRow.Balance), 14));
        _grid.Columns.Add(ModernGrid.Col("تعداد خرید", 100, DataGridViewContentAlignment.MiddleCenter, nameof(CustomerRow.SaleCount), 10));

        _grid.CellFormatting += (_, ev) =>
        {
            if (ev.Value is null) return;
            switch (ev.ColumnIndex)
            {
                case 3:
                    if (decimal.TryParse(ev.Value.ToString(), out decimal b))
                        ev.Value = Theme.Toman(b);
                    break;
                case 4:
                    ev.Value = Theme.FaDigits(ev.Value.ToString() ?? "");
                    break;
            }
        };
        _grid.DoubleClick += (_, _) => OpenEditor(Selected());
        _grid.SelectionChanged += (_, _) =>
        {
            _btnEdit.Enabled = _btnDel.Enabled = Selected() is not null;
        };

        Body.Controls.Add(_grid);
        Body.Controls.Add(toolbar);
        _btnEdit.Enabled = _btnDel.Enabled = false;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Reload();
    }

    private void Reload()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        var customers = db.Customers.AsNoTracking().OrderBy(c => c.Id).ToList();
        var saleCounts = db.Sales.AsNoTracking()
            .Where(s => s.CustomerId != null)
            .GroupBy(s => s.CustomerId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var filter = _search.Text.Trim();
        IEnumerable<CustomerRow> rows = customers
            .Where(c => string.IsNullOrWhiteSpace(filter) ||
                        c.Name.Contains(filter) || c.Phone.Contains(filter))
            .Select(c => new CustomerRow
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Address = c.Address,
                Balance = c.Balance,
                SaleCount = saleCounts.TryGetValue(c.Id, out int n) ? n : 0,
                Customer = c,
            });

        _grid.DataSource = rows.ToList();
    }

    private Customer? Selected()
    {
        if (_grid.SelectedRows.Count == 0) return null;
        if (_grid.SelectedRows[0].DataBoundItem is CustomerRow row) return row.Customer;
        return null;
    }

    private void OpenEditor(Customer? existing)
    {
        var f = Dialogs.Create(existing is null ? "افزودن مشتری" : "ویرایش مشتری", 440,
            out TableLayoutPanel body, out FlowLayoutPanel footer);

        var fName = Dialogs.AddField(body, "نام کامل", "مثلا علی رضایی");
        var fPhone = Dialogs.AddField(body, "شماره تماس", "09123456789");
        var fAddr = Dialogs.AddField(body, "آدرس", "شهر / محله / خیابان");
        var fBalance = Dialogs.AddField(body, "اعتبار/بدهی", "تومان (در صورت نسیه)");
        var fNotes = Dialogs.AddField(body, "یادداشت", "توضیح برای این مشتری", multiline: true);

        if (existing is not null)
        {
            fName.Text = existing.Name;
            fPhone.Text = existing.Phone;
            fAddr.Text = existing.Address;
            fBalance.Text = existing.Balance.ToString();
            fNotes.Text = existing.Notes;
        }

        var (ok, cancel) = Dialogs.Buttons(footer);
        cancel.Click += (_, _) => f.Close();

        ok.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(fName.Text))
            {
                MessageBox.Show("نام مشتری الزامی است.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal.TryParse(fBalance.Text, out decimal balance);

            using var db = new AppDbContext();
            var target = existing ?? new Customer();
            if (existing is null) db.Customers.Add(target);
            target.Name = fName.Text.Trim();
            target.Phone = fPhone.Text.Trim();
            target.Address = fAddr.Text.Trim();
            target.Balance = balance;
            target.Notes = fNotes.Text.Trim();
            db.SaveChanges();

            f.Close();
            Reload();
        };

        f.ShowDialog();
    }

    private void Delete(Customer? c)
    {
        if (c is null) return;
        if (MessageBox.Show($"«{c.Name}» حذف شود؟ فروش‌های مرتبط به «بدون مشتری» منتقل می‌شوند.",
                "حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        using var db = new AppDbContext();
        var customer = db.Customers.First(x => x.Id == c.Id);
        db.Sales.Where(s => s.CustomerId == customer.Id).ToList().ForEach(s => s.CustomerId = null);
        db.Customers.Remove(customer);
        db.SaveChanges();
        Reload();
    }
}
