using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class ShipmentTrackingPage : Page
    {
        private int? _selectedShipmentId = null;

        public ShipmentTrackingPage()
        {
            InitializeComponent();
            LoadShipments();
        }

        private void LoadShipments()
        {
            using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Id, ShipmentNumber, OriginCountry, DestinationCountry FROM Shipments ORDER BY SendDate DESC";
                
                var shipments = new List<ShipmentInfo>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    shipments.Add(new ShipmentInfo
                    {
                        Id = reader.GetInt32(0),
                        ShipmentNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Display = $"{reader.GetString(1)} - {reader.GetString(2)} به {reader.GetString(3)}"
                    });
                }
                
                cmbShipments.ItemsSource = shipments;
                cmbShipments.DisplayMemberPath = "Display";
                cmbShipments.SelectedValuePath = "Id";
            }
        }

        private void CmbShipments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbShipments.SelectedItem is ShipmentInfo selected)
            {
                _selectedShipmentId = selected.Id;
                LoadShipmentInfo(selected.Id);
                LoadStatusHistory(selected.Id);
            }
        }

        private void LoadShipmentInfo(int shipmentId)
        {
            using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT ShipmentNumber, OriginCountry, OriginCity, DestinationCountry, DestinationCity, Status
                    FROM Shipments WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", shipmentId);
                
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtShipmentInfo.Text = $"محموله: {reader.GetString(0)} | از: {reader.GetString(1)} {reader.GetString(2)} | به: {reader.GetString(3)} {reader.GetString(4)} | وضعیت: {reader.GetString(5)}";
                }
            }
        }

        private void LoadStatusHistory(int shipmentId)
        {
            using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT Status, Notes, date(StatusDate) as StatusDate
                    FROM ShipmentStatus
                    WHERE ShipmentId = @shipmentId
                    ORDER BY StatusDate DESC";
                cmd.Parameters.AddWithValue("@shipmentId", shipmentId);
                
                var statusList = new List<StatusEntry>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    statusList.Add(new StatusEntry
                    {
                        Status = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Notes = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        StatusDate = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }
                
                icStatusHistory.ItemsSource = statusList;
            }
        }

        private void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            // Just a placeholder, actual update happens on Save
        }

        private void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_selectedShipmentId.HasValue)
                {
                    MessageBox.Show("لطفاً یک محموله را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string newStatus = ((ComboBoxItem)cmbNewStatus.SelectedItem)?.Content?.ToString() ?? "";
                string notes = txtStatusNotes.Text;

                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    
                    // Add new status entry
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO ShipmentStatus (ShipmentId, Status, Notes)
                        VALUES (@shipmentId, @status, @notes)";
                    cmd.Parameters.AddWithValue("@shipmentId", _selectedShipmentId.Value);
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes);
                    cmd.ExecuteNonQuery();
                    
                    // Update shipment status
                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = "UPDATE Shipments SET Status = @status WHERE Id = @id";
                    updateCmd.Parameters.AddWithValue("@status", newStatus);
                    updateCmd.Parameters.AddWithValue("@id", _selectedShipmentId.Value);
                    updateCmd.ExecuteNonQuery();
                }

                MessageBox.Show("وضعیت با موفقیت بروزرسانی شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
                
                LoadShipmentInfo(_selectedShipmentId.Value);
                LoadStatusHistory(_selectedShipmentId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بروزرسانی وضعیت: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ShipmentInfo
    {
        public int Id { get; set; }
        public string ShipmentNumber { get; set; } = "";
        public string Display { get; set; } = "";
    }

    public class StatusEntry
    {
        public string Status { get; set; } = "";
        public string Notes { get; set; } = "";
        public string StatusDate { get; set; } = "";
    }
}
