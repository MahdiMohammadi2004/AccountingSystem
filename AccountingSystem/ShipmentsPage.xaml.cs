using System;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using FinancialSystem.Database;

namespace FinancialSystem
{
    public partial class ShipmentsPage : Page
    {
        public ShipmentsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Shipments ORDER BY CreatedAt DESC";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    dgShipments.ItemsSource = null;
                    dgShipments.ItemsSource = reader;
                }
            }
        }
    }
}
