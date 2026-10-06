using sales_app_desktop.UI;

namespace sales_app_desktop;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // ساخت دیتابیس و داده اولیه در اولین اجرا
        using (var db = new Data.AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        Application.Run(new ShellWindow());
    }
}