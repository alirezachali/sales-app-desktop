using System.Text.Json;

namespace sales_app_desktop.Data;

/// <summary>
/// تنظیمات برنامه؛ در فایل settings.json کنار دیتابیس ذخیره می‌شود.
/// </summary>
public class AppSettings
{
    /// <summary>تم تیره؟ (پیش‌فرض: روشن)</summary>
    public bool IsDark { get; set; } = false;

    /// <summary>واحد پولی (پسوند مبالغ).</summary>
    public string Currency { get; set; } = "تومان";

    /// <summary>اختلاف ساعت با UTC (مثلاً ۳٫۵ برای تهران).</summary>
    public double TimeZoneOffsetHours { get; set; } = 3.5;

    /// <summary>نام نمایشی منطقه زمانی.</summary>
    public string TimeZoneName { get; set; } = "تهران (UTC+3:30)";

    /// <summary>مسیر فایل لوگوی سفارشی (اختیاری).</summary>
    public string? LogoPath { get; set; }

    /// <summary>نام فروشگاه (در سایدبار و عنوان پنجره).</summary>
    public string StoreName { get; set; } = "فروشگاه من";

    private static readonly string FilePath =
        System.IO.Path.Combine(AppContext.BaseDirectory, "settings.json");

    private static AppSettings? _current;

    /// <summary>تنظیمات جاری (بارگذاری تنبل از فایل).</summary>
    public static AppSettings Current => _current ??= Load();

    /// <summary>ذخیره تنظیمات جاری روی دیسک.</summary>
    public static void Save()
    {
        var json = JsonSerializer.Serialize(Current,
            new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        File.WriteAllText(FilePath, json);
    }

    private static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var s = JsonSerializer.Deserialize<AppSettings>(json);
                if (s is not null) return s;
            }
        }
        catch
        {
            // فایل خراب → تنظیمات پیش‌فرض
        }
        return new AppSettings();
    }
}
