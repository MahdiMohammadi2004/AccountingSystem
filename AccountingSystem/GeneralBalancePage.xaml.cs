using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using FinancialSystem.Database;

namespace FinancialSystem
{
    public partial class GeneralBalancePage : Page
    {
        public GeneralBalancePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM GeneralBalance ORDER BY Date DESC";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    dgGeneralBalance.ItemsSource = null;
                    dgGeneralBalance.ItemsSource = reader;
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Add new transaction logic would go here
            MessageBox.Show("امکان افزودن سند جدید در این نسخه فراهم است.", "افزودن", MessageBoxButton.OK);
        }
    }
}
