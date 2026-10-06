using System.ComponentModel.DataAnnotations;

namespace sales_app_desktop.Data;

/// <summary>یک کالا/محصول قابل فروش.</summary>
public class Product
{
    public int Id { get; set; }

    /// <summary>بارکد؛ برای اسکن‌شده‌ها یکتا است، برای کالاهای بدون بارکد خالی.</summary>
    [MaxLength(64)]
    public string Barcode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Category { get; set; } = string.Empty;

    /// <summary>قیمت تمام‌شده (برای مشتری) به تومان.</summary>
    public decimal Price { get; set; }

    /// <summary>ارزش کالای ثبت‌شده در انبار، برای محاسبه سود.</summary>
    public decimal Cost { get; set; }

    public int Stock { get; set; }

    public List<SaleItem> Items { get; set; } = [];
}

/// <summary>یک مشتری ثبت‌شده (اختیاری در فروش).</summary>
public class Customer
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(32)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    /// <summary>بدهی/اعتبار مشتری (مثلاً خرید نسیه).</summary>
    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Sale> Sales { get; set; } = [];
}

/// <summary>یک فاکتور/فروش ثبت‌شده.</summary>
public class Sale
{
    public int Id { get; set; }

    /// <summary>شماره خوانا فاکتور (مثلاً F-1042).</summary>
    [MaxLength(32)]
    public string Number { get; set; } = string.Empty;

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    /// <summary>تاریخ ثبت فروش.</summary>
    public DateTime Date { get; set; } = DateTime.Now;

    public decimal Discount { get; set; }

    public decimal Total { get; set; }

    public List<SaleItem> Items { get; set; } = [];
}

/// <summary>یک ردیف کالا داخل فاکتور؛ قیمت لحظه فروش ثبت می‌شود.</summary>
public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    /// <summary>قیمت واحد لحظه فروش (مقایسه با محصول اصلی).</summary>
    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
