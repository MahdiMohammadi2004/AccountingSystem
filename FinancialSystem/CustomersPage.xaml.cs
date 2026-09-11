using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class CustomersPage : Page
    {
        private int? _selectedCustomerId = null;

        public CustomersPage()
        {
            InitializeComponent();
            dpTransactionDate.SelectedDate = DateTime.Today;
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
                    cmd.CommandText = "SELECT Id, CustomerCode, FirstName, LastName FROM Customers ORDER BY FirstName";
                    
                    var customers = new List<Customer>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            Id = reader.GetInt32(0),
                            CustomerCode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            FirstName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            LastName = reader.IsDBNull(3) ? "" : reader.GetString(3)
                        });
                    }
                    
                    dgCustomers.ItemsSource = customers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری مشتریان: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                _selectedCustomerId = selectedCustomer.Id;
                txtCustomerTitle.Text = $"جزئیات مشتری: {selectedCustomer.FirstName} {selectedCustomer.LastName}";
                
                LoadCustomerDetails(selectedCustomer.Id);
                LoadCustomerTransactions(selectedCustomer.Id);
            }
        }

        private void LoadCustomerDetails(int customerId)
        {
            using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT CustomerCode, FirstName, LastName, PhoneNumber, Address FROM Customers WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", customerId);
                
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtCustomerCode.Text = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    txtFirstName.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    txtLastName.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    txtPhoneNumber.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    txtAddress.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                }
            }
        }

        private void LoadCustomerTransactions(int customerId)
        {
            using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT 
                        date(TransactionDate),
                        Details,
                        Quantity,
                        Price,
                        Price * Quantity as Amount,
                        Balance
                    FROM CustomerTransactions
                    WHERE CustomerId = @customerId
                    ORDER BY TransactionDate DESC";
                cmd.Parameters.AddWithValue("@customerId", customerId);
                
                var transactions = new List<CustomerTransaction>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    transactions.Add(new CustomerTransaction
                    {
                        Date = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Details = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Quantity = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                        Price = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                        Amount = reader.IsDBNull(4) ? 0 : reader.GetDouble(4),
                        Balance = reader.IsDBNull(5) ? 0 : reader.GetDouble(5)
                    });
                }
                
                dgCustomerTransactions.ItemsSource = transactions;
            }
        }

        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            _selectedCustomerId = null;
            txtCustomerTitle.Text = "افزودن مشتری جدید";
            txtCustomerCode.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhoneNumber.Clear();
            txtAddress.Clear();
            dgCustomerTransactions.ItemsSource = null;
        }

        private void BtnSaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    
                    if (_selectedCustomerId.HasValue)
                    {
                        cmd.CommandText = @"
                            UPDATE Customers 
                            SET CustomerCode = @code, FirstName = @firstName, LastName = @lastName,
                                PhoneNumber = @phone, Address = @address
                            WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", _selectedCustomerId.Value);
                    }
                    else
                    {
                        cmd.CommandText = @"
                            INSERT INTO Customers (CustomerCode, FirstName, LastName, PhoneNumber, Address)
                            VALUES (@code, @firstName, @lastName, @phone, @address)";
                    }
                    
                    cmd.Parameters.AddWithValue("@code", txtCustomerCode.Text);
                    cmd.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@lastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhoneNumber.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    
                    cmd.ExecuteNonQuery();
                }
                
                MessageBox.Show("اطلاعات مشتری با موفقیت ذخیره شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره اطلاعات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_selectedCustomerId.HasValue)
                {
                    MessageBox.Show("لطفاً یک مشتری را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!dpTransactionDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("لطفاً تاریخ را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO CustomerTransactions (CustomerId, TransactionDate, Details, Quantity, Price, Balance)
                        VALUES (@customerId, @date, @details, @quantity, @price, @balance)";
                    
                    double quantity = double.TryParse(txtQuantity.Text, out var q) ? q : 0;
                    double price = double.TryParse(txtPrice.Text, out var p) ? p : 0;
                    double amount = quantity * price;
                    
                    // Calculate new balance
                    var balanceCmd = connection.CreateCommand();
                    balanceCmd.CommandText = "SELECT COALESCE((SELECT SUM(Price * Quantity) FROM CustomerTransactions WHERE CustomerId = @customerId), 0)";
                    balanceCmd.Parameters.AddWithValue("@customerId", _selectedCustomerId.Value);
                    double currentBalance = Convert.ToDouble(balanceCmd.ExecuteScalar());
                    double newBalance = currentBalance + amount;
                    
                    cmd.Parameters.AddWithValue("@customerId", _selectedCustomerId.Value);
                    cmd.Parameters.AddWithValue("@date", dpTransactionDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@details", "معامله");
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@balance", newBalance);
                    
                    cmd.ExecuteNonQuery();
                }
                
                MessageBox.Show("معامله با موفقیت ثبت شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCustomerTransactions(_selectedCustomerId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت معامله: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
    }

    public class CustomerTransaction
    {
        public string Date { get; set; } = "";
        public string Details { get; set; } = "";
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
    }
}
