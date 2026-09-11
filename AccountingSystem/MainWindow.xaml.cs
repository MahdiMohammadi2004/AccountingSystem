using System;
using System.Windows;
using System.Windows.Controls;

namespace AccountingSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Show dashboard by default
            MainFrame.Content = new DashboardPage();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DashboardPage();
        }

        private void GeneralBalance_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new GeneralBalancePage();
        }

        private void Journal_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new JournalPage();
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CustomersPage();
        }

        private void Shipments_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ShipmentsPage();
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ReportsPage();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("????? ??? ???????? ?? ??? ???????? ????? ???.", "???", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            BackupDatabase();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackupDatabase()
        {
            try
            {
                string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AccountingSystem.db");
                string backupPath = $"AccountingSystem_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                
                if (System.IO.File.Exists(dbPath))
                {
                    System.IO.File.Copy(dbPath, backupPath, true);
                    MessageBox.Show($"????? ?? ?????? ????? ??:\n{backupPath}", "?????", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("???? ??????? ???? ???.", "???", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"??? ?? ????? ?????: {ex.Message}", "???", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
