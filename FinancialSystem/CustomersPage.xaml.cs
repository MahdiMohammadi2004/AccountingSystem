using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            CustomerCode,
                            FirstName,
                            LastName,
                            PhoneNumber,
                            Address
                        FROM Customers
                        ORDER BY FirstName";
                    
                    var customers = new List<Customer>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerCode = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            FirstName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            LastName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            PhoneNumber = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Address = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        });
                    }
                    
                    dgCustomers.ItemsSource = customers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Customer
    {
        public string CustomerCode { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Address { get; set; } = "";
    }
}
