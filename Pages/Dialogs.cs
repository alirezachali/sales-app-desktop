using sales_app_desktop.UI;

namespace sales_app_desktop.Pages;

/// <summary>فرم‌های مودال ساده استاندارد (TableLayoutPanel برچسب + فیلد).</summary>
internal static class Dialogs
{
    /// <summary>ساخت فرم مودال استاندارد با نوار عنوان سیستم.</summary>
    public static Form Create(string title, int width, out TableLayoutPanel body, out FlowLayoutPanel footer)
    {
        var f = new Form
        {
            Text = title,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ShowInTaskbar = false,
            BackColor = Theme.Surface,
            ClientSize = new Size(width, 380),
            StartPosition = FormStartPosition.CenterParent,
            Font = Theme.F,
            RightToLeft = RightToLeft.Yes,
        };

        body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RightToLeft = RightToLeft.Yes,
            Padding = new Padding(14, 12, 14, 4),
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        footer = new FlowLayoutPanel
        {
            Height = 56,
            Dock = DockStyle.Bottom,
            BackColor = Theme.Base,
            FlowDirection = FlowDirection.LeftToRight,
            RightToLeft = RightToLeft.Yes,
            Padding = new Padding(12, 10, 12, 10),
        };

        f.Controls.Add(body);
        f.Controls.Add(footer);

        return f;
    }

    /// <summary>افزودن یک ردیف «برچسب + فیلد» به بدنه فرم.</summary>
    public static RField AddField(TableLayoutPanel body, string label, string placeholder,
        RField? field = null, bool multiline = false)
    {
        field ??= new RField { Placeholder = placeholder, Dock = DockStyle.Fill };
        if (multiline)
        {
            field.Multiline = true;
            field.Height = 70;
        }

        var lbl = new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Font = Theme.F,
        };

        int idx = body.Controls.Count / 2;
        body.Controls.Add(field, 1, idx);
        body.Controls.Add(lbl, 0, idx);

        int rows = idx + 1;
        body.RowStyles.Clear();
        for (int i = 0; i < rows; i++)
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, multiline ? 80 : 44));
        body.RowCount = rows;

        return field;
    }

    /// <summary>دکمه‌های تایید/انصراف استاندارد.</summary>
    public static (RButton ok, RButton cancel) Buttons(FlowLayoutPanel footer)
    {
        var ok = new RButton { Text = "ذخیره", Width = 110, Height = 34, Variant = ButtonVariant.Primary };
        var cancel = new RButton { Text = "انصراف", Width = 110, Height = 34 };
        footer.Controls.Add(ok);
        footer.Controls.Add(cancel);
        ok.Margin = new Padding(0, 0, 10, 0);
        f_ok(ok);
        return (ok, cancel);
    }

    private static void f_ok(RButton ok) { ok.DialogResult = DialogResult.None; }
}
