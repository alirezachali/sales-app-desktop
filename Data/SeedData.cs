namespace sales_app_desktop.Data;

/// <summary>داده اولیه برای اولین اجرای اپ (وقتی دیتابیس خالی است).</summary>
internal static class SeedData
{
    public static List<Product> Products =>
    [
        new Product { Id = 1, Barcode = "8620123450011", Name = "قهوه عربیکا ۲۵۰ گرم", Category = "مواد غذایی", Cost = 420_000, Price = 550_000, Stock = 40 },
        new Product { Id = 2, Barcode = "8620123450028", Name = "چای سیاه ۵۰۰ گرم", Category = "مواد غذایی", Cost = 300_000, Price = 380_000, Stock = 60 },
        new Product { Id = 3, Barcode = "8620123450035", Name = "روغن آفتابگردان ۱ لیتری", Category = "مواد غذایی", Cost = 150_000, Price = 210_000, Stock = 80 },
        new Product { Id = 4, Barcode = "8620123450042", Name = "شامپو ۴۰۰ میلی‌متر", Category = "بهداشتی", Cost = 220_000, Price = 320_000, Stock = 45 },
        new Product { Id = 5, Barcode = "8620123450059", Name = "دستمال کاغذی بسته ۱۰۰ تایی", Category = "بهداشتی", Cost = 80_000, Price = 120_000, Stock = 120 },
        new Product { Id = 6, Barcode = "", Name = "کیف خرید (لوازم جانبی)", Category = "متفرقه", Cost = 50_000, Price = 90_000, Stock = 25 },
        new Product { Id = 7, Barcode = "8620123450073", Name = "لواش سوخاری ۵۰۰ گرم", Category = "متفرقه", Cost = 90_000, Price = 130_000, Stock = 30 },
        new Product { Id = 8, Barcode = "8620123450080", Name = "آب معدنی لیتری", Category = "نوشیدنی", Cost = 30_000, Price = 50_000, Stock = 200 },
    ];

    public static List<Customer> Customers =>
    [
        new Customer { Id = 1, Name = "علی رضایی", Phone = "09123456789", Address = "تهران، سعادت‌آباد", Notes = "مشتری عمده‌فروش", Balance = 0 },
        new Customer { Id = 2, Name = "سارا محمدی", Phone = "09301112223", Address = "کرج، شمال شهر", Notes = "", Balance = 150_000 },
        new Customer { Id = 3, Name = "محمد کریمی", Phone = "09157778899", Address = "البرز", Notes = "پرداخت نسیه ماهانه", Balance = 0 },
    ];
}
