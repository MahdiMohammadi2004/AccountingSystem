using System;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using FinancialSystem.Database;

namespace FinancialSystem
{
    public partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Customers ORDER BY CustomerCode";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    dgCustomers.ItemsSource = null;
                    dgCustomers.ItemsSource = reader;
                }
            }
        }
    }
}
