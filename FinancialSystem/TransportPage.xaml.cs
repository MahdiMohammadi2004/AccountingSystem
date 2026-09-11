using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class TransportPage : Page
    {
        public TransportPage()
        {
            InitializeComponent();
            LoadShipments();
        }

        private void LoadShipments()
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            ShipmentNumber,
                            OriginCity,
                            DestinationCity,
                            Status,
                            TransportCompany,
                            DriverName
                        FROM Shipments
                        ORDER BY SendDate DESC";
                    
                    var shipments = new List<Shipment>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        shipments.Add(new Shipment
                        {
                            ShipmentNumber = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            OriginCity = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            DestinationCity = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Status = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            TransportCompany = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            DriverName = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                    
                    dgTransport.ItemsSource = shipments;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading shipments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Shipment
    {
        public string ShipmentNumber { get; set; } = "";
        public string OriginCity { get; set; } = "";
        public string DestinationCity { get; set; } = "";
        public string Status { get; set; } = "";
        public string TransportCompany { get; set; } = "";
        public string DriverName { get; set; } = "";
    }
}
