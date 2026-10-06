using System.Drawing.Printing;
using Microsoft.EntityFrameworkCore;
using sales_app_desktop.Data;
using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>
/// پیش‌نمایش فاکتور + چاپ؛ فرم استاندارد ویندوز با TextBox خوانا.
/// </summary>
public class ReceiptForm : Form
{
    private readonly Sale _sale;
    private readonly string _customerName;

    public ReceiptForm(Sale sale, string customerName)
    {
        _sale = sale;
        _customerName = customerName;

        Text = "فاکتور فروش";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(400, 480);
        BackColor = Theme.Surface;
        Font = Theme.F;
        RightToLeft = RightToLeft.Yes;
    }

    private string BuildText()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("فاکتور فروش");
        sb.AppendLine(new string('-', 34));
        sb.AppendLine($"شماره : {Theme.FaDigits(_sale.Number)}");
        sb.AppendLine($"تاریخ : {_sale.Date:yyyy/MM/dd HH:mm}");
        sb.AppendLine($"مشتری : {(string.IsNullOrEmpty(_customerName) ? "عابر" : _customerName)}");
        sb.AppendLine(new string('-', 34));
        foreach (var item in _sale.Items)
        {
            sb.AppendLine($"{item.Product.Name}");
            sb.AppendLine($"   {Theme.FaDigits(item.Quantity.ToString())} × {Theme.Toman(item.UnitPrice)} = {Theme.Toman(item.LineTotal)}");
        }
        sb.AppendLine(new string('-', 34));
        sb.AppendLine($"جمع کل : {Theme.Toman(_sale.Total + _sale.Discount)}");
        sb.AppendLine($"تخفیف : {Theme.Toman(_sale.Discount)}");
        sb.AppendLine($"قابل پرداخت : {Theme.Toman(_sale.Total)}");
        sb.AppendLine(new string('-', 34));
        sb.AppendLine("با تشکر از خرید شما");
        return sb.ToString();
    }

    public static DialogResult ShowFor(Sale sale, string customerName)
    {
        using var f = new ReceiptForm(sale, customerName);
        return f.ShowDialog();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var text = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            BackColor = Theme.Surface,
            ForeColor = Theme.Text,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Consolas", 10f),
            RightToLeft = RightToLeft.Yes,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Text = BuildText(),
        };

        var printBtn = new RButton
        {
            Text = "چاپ فاکتور",
            Width = 120,
            Height = 34,
            Variant = ButtonVariant.Primary,
        };
        var closeBtn = new RButton
        {
            Text = "بستن",
            Width = 100,
            Height = 34,
        };
        closeBtn.Click += (_, _) => Close();
        printBtn.Click += (_, _) => Print();

        var footer = new FlowLayoutPanel
        {
            Height = 54,
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.LeftToRight,
            RightToLeft = RightToLeft.Yes,
            Padding = new Padding(10, 10, 10, 6),
            BackColor = Theme.Base,
        };
        footer.Controls.Add(printBtn);
        footer.Controls.Add(closeBtn);

        Controls.Add(text);
        Controls.Add(footer);

        AcceptButton = printBtn;
        CancelButton = closeBtn;
    }

    private void Print()
    {
        try
        {
            using var doc = new PrintDocument();
            doc.PrintPage += (_, ev) =>
            {
                using var f = new Font("Consolas", 10f);
                var lines = BuildText().Replace("\r\n", "\n").Split('\n');
                float y = ev.MarginBounds.Top;
                foreach (var line in lines)
                {
                    ev.Graphics.DrawString(line, f, Brushes.Black,
                        ev.MarginBounds.Left, y);
                    y += f.GetHeight(ev.Graphics) + 2;
                }
            };
            doc.Print();
            MessageBox.Show("فاکتور ارسال شد.", "چاپ",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("چاپ امکان‌پذیر نیست: " + ex.Message, "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
