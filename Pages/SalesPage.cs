using Microsoft.EntityFrameworkCore;
using sales_app_desktop.Data;
using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// صفحه ثبت فروش: اسکن بارکد → سبد خرید → انتخاب مشتری/تخفیف → ثبت → چاپ فاکتور.
/// چیدمان با TableLayoutPanel استاندارد.
/// </summary>
public class SalesPage : PageBase
{
    private readonly DataGridView _cart = new();
    private readonly ComboBox _customer = new();
    private readonly RField _barcode = new();
    private readonly RField _discount = new();
    private readonly Label _totalVal = new()
    {
        Text = "۰ تومان",
        Font = Theme.FXL,
        ForeColor = Theme.Accent,
        AutoSize = true,
        BackColor = Color.Transparent,
    };

    private readonly Dictionary<int, CartLine> _lines = [];
    private Sale? _lastSale;

    private sealed class CartLine
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; } = int.MaxValue;
        public decimal LineTotal => UnitPrice * Quantity;
        public override string ToString() => Name;
    }

    /// <summary>آیتم ComboBox مشتری: Id واقعی دیتابیس + متن نمایشی.</summary>
    private sealed record CustomerOption(int Id, string Display);

    public SalesPage()
    {
        SetHeader("ثبت فروش", "اسکن بارکد، تکمیل سبد و صدور فاکتور");

        // ── پنل کنار (ورود بارکد/مشتری/تخفیف) ───────────────
        var side = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(8), Width = 320 };
        side.Dock = DockStyle.Left; // در RTL یعنی سمت راست

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RightToLeft = RightToLeft.Yes,
            BackColor = Theme.Surface,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _barcode.Placeholder = "اسکن یا تایپ بارکد و Enter…";
        _barcode.BarKey += OnBarKey;
        _barcode.Dock = DockStyle.Fill;

        _customer.DropDownStyle = ComboBoxStyle.DropDownList;
        _customer.DisplayMember = nameof(CustomerOption.Display);
        _customer.ValueMember = nameof(CustomerOption.Id);
        _customer.Font = Theme.F;
        _customer.Width = 280;
        _customer.BackColor = Theme.Surface;
        _customer.ForeColor = Theme.Text;
        _customer.FlatStyle = FlatStyle.Flat;

        _discount.Placeholder = "تخفیف کل سبد (تومان)";
        _discount.Dock = DockStyle.Fill;

        var btnRegister = new RButton { Text = "ثبت فروش و چاپ", Height = 40, Variant = ButtonVariant.Success, Dock = DockStyle.Fill };
        var btnClear = new RButton { Text = "پاک‌سازی سبد", Height = 32, Dock = DockStyle.Fill };
        var btnPrint = new RButton { Text = "چاپ فاکتور قبلی", Height = 32, Dock = DockStyle.Fill };

        btnRegister.Click += (_, _) => RegisterSale();
        btnClear.Click += (_, _) => ClearCart();
        btnPrint.Click += (_, _) => PrintLast();

        void AddRow(Control c, int h)
        {
            int r = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, h));
            layout.Controls.Add(c, 0, r);
        }
        void AddLabel(string t)
        {
            var l = new Label { Text = t, Font = Theme.FB, AutoSize = true, BackColor = Color.Transparent };
            AddRow(l, 26);
        }

        AddLabel("اسکن بارکد");
        AddRow(_barcode, 40);
        AddRow(new Label(), 6);
        AddLabel("مشتری");
        AddRow(_customer, 32);
        AddRow(new Label(), 6);
        AddLabel("تخفیف");
        AddRow(_discount, 34);
        AddRow(new Label(), 8);
        AddRow(new Panel { Height = 60, BackColor = Color.Transparent }, 0); // placeholder

        // جعبه جمع کل
        var totalBox = new Panel { Height = 70, BackColor = Theme.Base, Dock = DockStyle.Fill };
        var totalLbl = new Label { Text = "جمع کل", Font = Theme.FB, ForeColor = Theme.TextDim, AutoSize = true, Location = new Point(12, 8) };
        totalBox.Controls.Add(totalLbl);
        totalBox.Controls.Add(_totalVal);
        _totalVal.Location = new Point(12, 32);
        AddRow(totalBox, 70);

        AddRow(new Label(), 8);
        AddRow(btnRegister, 44);
        AddRow(new Label(), 6);

        var btnsRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Height = 36, BackColor = Color.Transparent };
        btnsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        btnsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        btnsRow.Controls.Add(btnClear, 0, 0);
        btnsRow.Controls.Add(btnPrint, 1, 0);
        AddRow(btnsRow, 38);

        // ردیف انعطاف‌پذیر آخر
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        side.Controls.Add(layout);

        // ── سبد خرید ────────────────────────────────────────
        _cart.ReadOnly = true;
        ModernGrid.Apply(_cart);
        _cart.Columns.Add(ModernGrid.Col("کالا", 200, DataGridViewContentAlignment.MiddleRight, "Name"));
        _cart.Columns.Add(ModernGrid.Col("قیمت واحد", 110, DataGridViewContentAlignment.MiddleRight, "UnitPrice"));
        _cart.Columns.Add(ModernGrid.Col("تعداد", 70, DataGridViewContentAlignment.MiddleCenter, "Quantity"));
        _cart.Columns.Add(ModernGrid.Col("جمع", 120, DataGridViewContentAlignment.MiddleRight, "LineTotal"));
        _cart.CellFormatting += (_, ev) =>
        {
            if (ev.Value is null) return;
            switch (ev.ColumnIndex)
            {
                case 1:
                case 3:
                    if (decimal.TryParse(ev.Value.ToString(), out decimal v))
                        ev.Value = Theme.Toman(v);
                    break;
                case 2:
                    ev.Value = Theme.FaDigits(ev.Value.ToString() ?? "");
                    break;
            }
        };
        _cart.CellEndEdit += OnCellEndEdit;
        _cart.CurrentCellDirtyStateChanged += (_, _) => CommitCart();

        _cart.Dock = DockStyle.Fill;
        Body.Controls.Add(_cart);
        Body.Controls.Add(side);

        LoadCustomers();
    }

    // ── بارگذاری مشتریان ─────────────────────────────────
    private void LoadCustomers()
    {
        using var db = new AppDbContext();
        var options = new List<CustomerOption>
        {
            new(0, "مشتری عابر (بدون پرونده)"),
        };
        options.AddRange(db.Customers.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CustomerOption(c.Id,
                string.IsNullOrEmpty(c.Phone) ? c.Name : $"{c.Name}  ({Theme.FaDigits(c.Phone)})")));

        _customer.DataSource = options;
    }

    /// <summary>Id مشتری انتخاب‌شده؛ صفر یعنی «مشتری عابر».</summary>
    private int SelectedCustomerId =>
        _customer.SelectedValue is int id ? id : 0;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadCustomers(); // لیست مشتریان همیشه به‌روز
        _barcode.Focus();
        _barcode.SelectAll();
    }

    // ── اسکن بارکد ─────────────────────────────────────────
    private void OnBarKey(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        string code = _barcode.Text.Trim();
        _barcode.Text = "";
        if (string.IsNullOrWhiteSpace(code)) return;

        AddByBarcode(code);
        _barcode.Focus();
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void AddByBarcode(string code)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        var p = db.Products.AsNoTracking()
            .FirstOrDefault(x => x.Barcode == code && !string.IsNullOrEmpty(x.Barcode))
            ?? db.Products.AsNoTracking().FirstOrDefault(x => x.Name == code);

        if (p is null)
        {
            MessageBox.Show($"محصولی با بارکد «{Theme.FaDigits(code)}» یافت نشد.\n" +
                            "لطفاً در صفحه محصولات ثبت کنید.", "محصول نامشخص",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        AddToCart(p);
    }

    private void AddToCart(Product p)
    {
        if (p.Stock <= 0)
        {
            MessageBox.Show($"«{p.Name}» موجودی ندارد.", "کاهش موجودی",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (_lines.TryGetValue(p.Id, out var line))
        {
            if (line.Quantity + 1 > p.Stock)
            {
                MessageBox.Show($"موجودی کافی نیست.\n«{p.Name}» تنها {Theme.FaDigits(p.Stock.ToString())} عدد دارد.",
                    "کاهش موجودی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            line.Quantity++;
        }
        else
        {
            _lines[p.Id] = new CartLine
            {
                ProductId = p.Id,
                Name = p.Name,
                UnitPrice = p.Price,
                Quantity = 1,
                MaxStock = p.Stock,
            };
        }
        RefreshCart();
    }

    private void RemoveCartItem(int index)
    {
        if (index < 0 || index >= _cart.Rows.Count) return;
        if (_cart.Rows[index].Tag is not CartLine line) return;
        _lines.Remove(line.ProductId);
        _cart.Rows.RemoveAt(index);
        RefreshCart();
    }

    // ── سبد ────────────────────────────────────────────────
    private void RefreshCart()
    {
        _cart.Rows.Clear();
        foreach (var l in _lines.Values)
        {
            int i = _cart.Rows.Add(l.Name, l.UnitPrice, l.Quantity, l.LineTotal);
            _cart.Rows[i].Tag = l;
        }
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        decimal subtotal = _lines.Values.Sum(v => v.LineTotal);
        decimal.TryParse(_discount.Text, out decimal disc);
        disc = Math.Clamp(disc, 0, subtotal);
        decimal total = subtotal - disc;
        _totalVal.Text = Theme.Toman(total);
        _currentTotal = total;
    }

    private decimal _currentTotal;

    private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != 2) return;
        if (_cart.Rows[e.RowIndex]?.Tag is CartLine line &&
            int.TryParse(_cart.Rows[e.RowIndex].Cells[2].Value?.ToString(), out int qty))
        {
            qty = Math.Clamp(qty, 0, line.MaxStock);
            line.Quantity = qty;
            if (qty == 0) { RemoveCartItem(e.RowIndex); }
            else { _cart.Rows[e.RowIndex].Cells[3].Value = line.LineTotal; }
        }
        CommitCart();
    }

    private void CommitCart()
    {
        if (_cart.IsCurrentCellInEditMode) _cart.CommitEdit(DataGridViewDataErrorContexts.Commit);
        UpdateTotal();
    }

    private void ClearCart()
    {
        _lines.Clear();
        _discount.Text = "";
        _customer.SelectedIndex = 0;
        RefreshCart();
    }

    // ── ثبت فروش ───────────────────────────────────────────
    private void RegisterSale()
    {
        if (_lines.Count == 0)
        {
            MessageBox.Show("سبد خالی است.", "توجه",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        var saleId = NextSaleId(db);
        var sale = new Sale
        {
            Number = $"F-{Theme.FaDigits(saleId.ToString())}",
            Date = DateTime.Now,
            Discount = decimal.TryParse(_discount.Text, out decimal d) ? d : 0,
            Total = _currentTotal,
            CustomerId = SelectedCustomerId > 0 ? SelectedCustomerId : null,
        };

        var products = db.Products.Where(p => _lines.Keys.Contains(p.Id)).ToList();
        if (products.Count != _lines.Count)
        {
            MessageBox.Show("برخی محصولات حذف شده‌اند. سبد را پاک کنید.", "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        foreach (var line in _lines.Values)
        {
            var p = products.First(x => x.Id == line.ProductId);
            if (line.Quantity > p.Stock)
            {
                MessageBox.Show($"موجودی «{p.Name}» کافی نیست.", "کاهش موجودی",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            p.Stock -= line.Quantity;
            sale.Items.Add(new SaleItem
            {
                Product = p,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = line.LineTotal,
            });
        }

        db.Sales.Add(sale);
        db.SaveChanges();

        _lastSale = sale;
        ClearCart();
        MessageBox.Show($"فروش {Theme.FaDigits(sale.Number)} با موفقیت ثبت شد.\nجمع: {Theme.Toman(sale.Total)}",
            "ثبت فروش", MessageBoxButtons.OK, MessageBoxIcon.Information);

        PrintSale(sale);
    }

    private static int NextSaleId(AppDbContext db) =>
        db.Sales.Count() + 1;

    private void PrintLast()
    {
        if (_lastSale is null)
        {
            MessageBox.Show("فیشی برای چاپ وجود ندارد.", "توجه",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        PrintSale(_lastSale);
    }

    // ── چاپ فاکتور ─────────────────────────────────────────
    private void PrintSale(Sale sale)
    {
        using var db = new AppDbContext();
        var full = db.Sales.Include(s => s.Items).ThenInclude(i => i.Product)
            .Where(s => s.Id == sale.Id).First();
        full.Customer = full.CustomerId is not null
            ? db.Customers.AsNoTracking().FirstOrDefault(c => c.Id == full.CustomerId)
            : null;

        ReceiptForm.ShowFor(full, full.Customer?.Name ?? "مشتری عابر");
    }
}
