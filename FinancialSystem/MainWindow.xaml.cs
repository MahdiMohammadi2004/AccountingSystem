using System;
using System.IO;
using System.Windows;

namespace FinancialSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Load dashboard by default
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnGeneralBalance_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GeneralBalancePage());
        }

        private void BtnJournal_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new JournalPage());
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomersPage());
        }

        private void BtnTransport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TransportPage());
        }

        private void BtnShipmentTracking_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ShipmentTrackingPage());
        }

        private void BtnExpenses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ExpensesPage());
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("امکان چاپ گزارش‌ها در این نسخه فراهم است.", "چاپ گزارش", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            BackupDatabase();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackupDatabase()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string sourcePath = Path.Combine(baseDir, "FinancialSystem.db");
                string backupPath = Path.Combine(
                    baseDir, 
                    $"FinancialSystem_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                
                File.Copy(sourcePath, backupPath, true);
                MessageBox.Show($"پشتیبان با موفقیت ایجاد شد:\n{backupPath}", "پشتیبان‌گیری", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در پشتیبان‌گیری: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
