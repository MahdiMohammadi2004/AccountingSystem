using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class TransportPage : Page
    {
        public TransportPage()
        {
            InitializeComponent();
            dpSendDate.SelectedDate = DateTime.Today;
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
                            OriginCountry || ' - ' || OriginCity as Origin,
                            DestinationCountry || ' - ' || DestinationCity as Destination,
                            date(SendDate) as SendDate,
                            Status,
                            TransportCompany
                        FROM Shipments
                        ORDER BY SendDate DESC";
                    
                    var shipments = new List<Shipment>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        shipments.Add(new Shipment
                        {
                            ShipmentNumber = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            Origin = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Destination = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            SendDate = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Status = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            TransportCompany = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                    
                    dgShipments.ItemsSource = shipments;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری محموله‌ها: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddShipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dpSendDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("لطفاً تاریخ ارسال را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO Shipments (ShipmentNumber, OriginCountry, OriginCity, DestinationCountry, DestinationCity, 
                                              SendDate, Status, TransportCompany, VehicleNumber, DriverName)
                        VALUES (@number, @originCountry, @originCity, @destCountry, @destCity, 
                                @sendDate, @status, @transportCompany, @vehicleNumber, @driverName)";
                    
                    cmd.Parameters.AddWithValue("@number", txtShipmentNumber.Text);
                    cmd.Parameters.AddWithValue("@originCountry", txtOriginCountry.Text);
                    cmd.Parameters.AddWithValue("@originCity", txtOriginCity.Text);
                    cmd.Parameters.AddWithValue("@destCountry", txtDestinationCountry.Text);
                    cmd.Parameters.AddWithValue("@destCity", txtDestinationCity.Text);
                    cmd.Parameters.AddWithValue("@sendDate", dpSendDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@status", "ثبت محموله");
                    cmd.Parameters.AddWithValue("@transportCompany", txtTransportCompany.Text);
                    cmd.Parameters.AddWithValue("@vehicleNumber", txtVehicleNumber.Text);
                    cmd.Parameters.AddWithValue("@driverName", txtDriverName.Text);
                    
                    cmd.ExecuteNonQuery();
                    
                    // Add initial status
                    var statusCmd = connection.CreateCommand();
                    statusCmd.CommandText = @"
                        INSERT INTO ShipmentStatus (ShipmentId, Status, Notes)
                        SELECT Id, 'ثبت محموله', 'محموله ثبت شد'
                        FROM Shipments WHERE ShipmentNumber = @number";
                    statusCmd.Parameters.AddWithValue("@number", txtShipmentNumber.Text);
                    statusCmd.ExecuteNonQuery();
                }

                MessageBox.Show("محموله با موفقیت ثبت شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Clear form
                txtShipmentNumber.Clear();
                txtOriginCountry.Clear();
                txtOriginCity.Clear();
                txtDestinationCountry.Clear();
                txtDestinationCity.Clear();
                txtTransportCompany.Clear();
                txtVehicleNumber.Clear();
                txtDriverName.Clear();
                
                LoadShipments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت محموله: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Shipment
    {
        public string ShipmentNumber { get; set; } = "";
        public string Origin { get; set; } = "";
        public string Destination { get; set; } = "";
        public string SendDate { get; set; } = "";
        public string Status { get; set; } = "";
        public string TransportCompany { get; set; } = "";
    }
}
