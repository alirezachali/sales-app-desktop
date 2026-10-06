using Microsoft.EntityFrameworkCore;
using sales_app_desktop.Data;
using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// صفحه محصولات و انبار: لیست، جستجو، افزودن/ویرایش با بارکد و موجودی.
/// </summary>
public class ProductsPage : PageBase
{
    private readonly DataGridView _grid = new();
    private readonly RField _search = new();
    private RButton _btnAdd;
    private RButton _btnEdit;
    private RButton _btnDel;

    public ProductsPage()
    {
        SetHeader("محصولات و انبار", "مدیریت کالا، بارکد و موجودی");

        // ── نوار ابزار ───────────────────────────────
        var toolbar = new Panel
        {
            BackColor = Theme.Surface,
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(8),
        };

        _search.Placeholder = "جستجو: نام یا بارکد محصول…";
        _search.Dock = DockStyle.Fill;

        _btnAdd = new RButton { Text = "افزودن محصول", Width = 130, Height = 32, Variant = ButtonVariant.Primary };
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
        _grid.Dock = DockStyle.Fill;
        _grid.Columns.Add(ModernGrid.Col("کد", 70, DataGridViewContentAlignment.MiddleCenter, nameof(Product.Id), 7));
        _grid.Columns.Add(ModernGrid.Col("نام", 240, DataGridViewContentAlignment.MiddleRight, nameof(Product.Name), 22));
        _grid.Columns.Add(ModernGrid.Col("بارکد", 150, DataGridViewContentAlignment.MiddleLeft, nameof(Product.Barcode), 15));
        _grid.Columns.Add(ModernGrid.Col("دسته", 120, DataGridViewContentAlignment.MiddleRight, nameof(Product.Category), 12));
        _grid.Columns.Add(ModernGrid.Col("قیمت", 110, DataGridViewContentAlignment.MiddleRight, nameof(Product.Price), 12));
        _grid.Columns.Add(ModernGrid.Col("موجودی", 80, DataGridViewContentAlignment.MiddleCenter, nameof(Product.Stock), 8));

        _grid.CellFormatting += (_, ev) =>
        {
            if (ev.Value is null) return;
            switch (ev.ColumnIndex)
            {
                case 0: // کد
                    ev.Value = Theme.FaDigits(ev.Value.ToString() ?? "");
                    break;
                case 4: // قیمت
                    if (decimal.TryParse(ev.Value.ToString(), out decimal p))
                        ev.Value = Theme.Toman(p);
                    break;
                case 5: // موجودی
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

        IQueryable<Product> q = db.Products.AsNoTracking();
        string filter = _search.Text.Trim();
        if (!string.IsNullOrWhiteSpace(filter))
            q = q.Where(p => p.Name.Contains(filter) || p.Barcode.Contains(filter));

        _grid.DataSource = q.OrderByDescending(p => p.Id).Take(500).ToList();
    }

    private Product? Selected()
    {
        if (_grid.SelectedRows.Count == 0) return null;
        return _grid.SelectedRows[0].DataBoundItem as Product;
    }

    private void OpenEditor(Product? existing)
    {
        var f = Dialogs.Create(existing is null ? "افزودن محصول" : "ویرایش محصول", 440,
            out TableLayoutPanel body, out FlowLayoutPanel footer);

        var fBarcode = Dialogs.AddField(body, "بارکد", "بارکد (اختیاری)");
        var fName = Dialogs.AddField(body, "نام کالا", "مثلا قهوه عربیکا ۲۵۰ گرم");
        var fCat = Dialogs.AddField(body, "دسته", "مثلا مواد غذایی");
        var fPrice = Dialogs.AddField(body, "قیمت فروش", "تومان");
        var fCost = Dialogs.AddField(body, "هزینه خریداری", "تومان");
        var fStock = Dialogs.AddField(body, "موجودی", "تعداد");

        if (existing is not null)
        {
            fBarcode.Text = existing.Barcode;
            fName.Text = existing.Name;
            fCat.Text = existing.Category;
            fPrice.Text = existing.Price.ToString();
            fCost.Text = existing.Cost.ToString();
            fStock.Text = existing.Stock.ToString();
        }

        var (ok, cancel) = Dialogs.Buttons(footer);
        cancel.Click += (_, _) => f.Close();

        ok.Click += (_, _) =>
        {
            if (!decimal.TryParse(fPrice.Text, out decimal price))
            {
                MessageBox.Show("قیمت فروش باید عدد باشد.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal.TryParse(fCost.Text, out decimal cost);
            int.TryParse(fStock.Text, out int stock);

            using var db = new AppDbContext();
            var target = existing ?? new Product();
            if (existing is null) db.Products.Add(target);
            target.Barcode = fBarcode.Text.Trim();
            target.Name = string.IsNullOrWhiteSpace(fName.Text) ? "بدون نام" : fName.Text.Trim();
            target.Category = fCat.Text.Trim();
            target.Price = price;
            target.Cost = cost;
            target.Stock = stock;
            db.SaveChanges();

            f.Close();
            Reload();
        };

        f.ShowDialog();
    }

    private void Delete(Product? p)
    {
        if (p is null) return;
        if (MessageBox.Show($"«{p.Name}» حذف شود؟", "حذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        using var db = new AppDbContext();
        var product = db.Products.Include(x => x.Items).First(x => x.Id == p.Id);
        db.SaleItems.RemoveRange(product.Items);
        db.Products.Remove(product);
        db.SaveChanges();
        Reload();
    }
}
