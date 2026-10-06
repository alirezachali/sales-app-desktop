using System.Windows.Forms;

namespace sales_app_desktop.UI;

/// <summary>
/// ظاهر استاندارد و روشن برای DataGridView.
/// </summary>
internal static class ModernGrid
{
    public static void Apply(DataGridView g)
    {
        g.BackgroundColor = Theme.Surface;
        g.BorderStyle = BorderStyle.FixedSingle;
        g.GridColor = Theme.SurfaceAlt;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Theme.Surface,
            SelectionBackColor = Theme.Accent,
            SelectionForeColor = Color.White,
            ForeColor = Theme.Text,
            Font = Theme.F,
            Padding = new Padding(4, 3, 4, 3),
        };
        g.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Theme.Base,
            ForeColor = Theme.Text,
        };
        g.RowHeadersVisible = false;
        g.ColumnHeadersHeight = 36;
        g.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Theme.Base,
            ForeColor = Theme.TextDim,
            Font = Theme.FB,
            SelectionBackColor = Theme.Base,
            SelectionForeColor = Theme.TextDim,
            Padding = new Padding(4, 4, 4, 4),
        };
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        g.EnableHeadersVisualStyles = true;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.MultiSelect = false;
        g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        g.Font = Theme.F;
        g.RightToLeft = RightToLeft.Yes;
        g.ShowEditingIcon = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToResizeRows = false;
        g.ScrollBars = ScrollBars.Vertical;
    }

    /// <summary>ساخت یک ستون ساده با DataPropertyName دلخواه (fillWeight برای پهنای نسبی).</summary>
    public static DataGridViewTextBoxColumn Col(string header, int width,
        DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleRight,
        string? dataProperty = null,
        float fillWeight = 10)
    {
        var c = new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            Width = width,
            FillWeight = fillWeight,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DataPropertyName = dataProperty ?? header,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = align,
                Padding = new Padding(4, 3, 4, 3),
            },
        };
        return c;
    }
}
