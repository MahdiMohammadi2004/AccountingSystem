using System;
using System.Windows;

namespace AccountingSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize database
            Database.DbHelper.InitializeDatabase();
            
            // Show login window
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
    }
}
