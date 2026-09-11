using System.Windows;

namespace FinancialSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize database
            DatabaseHelper.InitializeDatabase();
            
            // Show login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
