using Microsoft.EntityFrameworkCore;

namespace sales_app_desktop.Data;

/// <summary>
/// DbContext برنامه؛ فایل SQLite در کنار اپلیکیشن ساخته می‌شود.
/// </summary>
public class AppDbContext : DbContext
{
    private static readonly string DbPath = Path.Combine(
        AppContext.BaseDirectory, "sales.db");

    /// <summary>مسیر کامل فایل دیتابیس (برای نمایش/بکاپ).</summary>
    public static string FullDbPath => DbPath;

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // بارکد به‌طور منطقی یکتا؛ اگر خالی است (کالای بدون بارکد) محدودیت اعمال نمی‌شود.
        mb.Entity<Product>().HasIndex(p => p.Barcode);

        mb.Entity<Product>().HasData(SeedData.Products);
        mb.Entity<Customer>().HasData(SeedData.Customers);
    }
}
