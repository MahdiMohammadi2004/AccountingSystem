using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class ShipmentTrackingPage : Page
    {
        public ShipmentTrackingPage()
        {
            InitializeComponent();
            LoadTracking();
        }

        private void LoadTracking()
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            s.ShipmentNumber,
                            ss.Status,
                            date(ss.StatusDate) as StatusDate,
                            ss.Notes
                        FROM ShipmentStatus ss
                        JOIN Shipments s ON ss.ShipmentId = s.Id
                        ORDER BY ss.StatusDate DESC";
                    
                    var trackingList = new List<TrackingEntry>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        trackingList.Add(new TrackingEntry
                        {
                            ShipmentNumber = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            Status = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            StatusDate = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Notes = reader.IsDBNull(3) ? "" : reader.GetString(3)
                        });
                    }
                    
                    dgTracking.ItemsSource = trackingList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tracking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class TrackingEntry
    {
        public string ShipmentNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string StatusDate { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}
