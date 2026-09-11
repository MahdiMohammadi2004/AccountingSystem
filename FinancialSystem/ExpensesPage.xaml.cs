using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class ExpensesPage : Page
    {
        public ExpensesPage()
        {
            InitializeComponent();
            dpExpenseDate.SelectedDate = DateTime.Today;
            LoadShipments();
            LoadExpenses();
        }

        private void LoadShipments()
        {
            using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Id, ShipmentNumber FROM Shipments ORDER BY SendDate DESC";
                
                var shipments = new List<ShipmentSimple>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    shipments.Add(new ShipmentSimple
                    {
                        Id = reader.GetInt32(0),
                        ShipmentNumber = reader.IsDBNull(1) ? "" : reader.GetString(1)
                    });
                }
                
                cmbShipments.ItemsSource = shipments;
                cmbShipments.DisplayMemberPath = "ShipmentNumber";
                cmbShipments.SelectedValuePath = "Id";
            }
        }

        private void LoadExpenses()
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            date(ExpenseDate),
                            ExpenseType,
                            Amount,
                            Currency,
                            PaidTo,
                            ReceiptNumber
                        FROM ShipmentExpenses
                        ORDER BY ExpenseDate DESC";
                    
                    var expenses = new List<Expense>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        expenses.Add(new Expense
                        {
                            ExpenseDate = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            ExpenseType = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Amount = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                            Currency = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            PaidTo = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            ReceiptNumber = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                    
                    dgExpenses.ItemsSource = expenses;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری مصارف: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dpExpenseDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("لطفاً تاریخ را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(txtAmount.Text, out double amount))
                {
                    MessageBox.Show("مبلغ وارد شده معتبر نیست", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string expenseType = ((ComboBoxItem)cmbExpenseType.SelectedItem)?.Content?.ToString() ?? "";
                int? shipmentId = cmbShipments.SelectedValue as int?;

                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO ShipmentExpenses (ShipmentId, ExpenseDate, ExpenseType, Amount, Currency, 
                                                     PaidFromAccount, PaidTo, ReceiptNumber, Details)
                        VALUES (@shipmentId, @date, @type, @amount, @currency, @paidFrom, @paidTo, @receipt, @details)";
                    
                    cmd.Parameters.AddWithValue("@shipmentId", shipmentId.HasValue ? (object)shipmentId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@date", dpExpenseDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@type", expenseType);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@currency", txtCurrency.Text);
                    cmd.Parameters.AddWithValue("@paidFrom", txtPaidFrom.Text);
                    cmd.Parameters.AddWithValue("@paidTo", txtPaidTo.Text);
                    cmd.Parameters.AddWithValue("@receipt", txtReceiptNumber.Text);
                    cmd.Parameters.AddWithValue("@details", "مصرف گمرک/ترانسپورت");
                    
                    cmd.ExecuteNonQuery();
                    
                    // Also add to General Balance as expense
                    var balanceCmd = connection.CreateCommand();
                    balanceCmd.CommandText = @"
                        INSERT INTO GeneralBalance (TransactionDate, Details, Amount, TransactionType, AccountName)
                        VALUES (@date, @details, @amount, 'Expense', @account)";
                    balanceCmd.Parameters.AddWithValue("@date", dpExpenseDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    balanceCmd.Parameters.AddWithValue("@details", $"مصرف گمرک: {expenseType}");
                    balanceCmd.Parameters.AddWithValue("@amount", amount);
                    balanceCmd.Parameters.AddWithValue("@account", txtPaidFrom.Text);
                    balanceCmd.ExecuteNonQuery();
                }

                MessageBox.Show("مصرف با موفقیت ثبت شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Clear form
                txtAmount.Clear();
                txtPaidFrom.Clear();
                txtPaidTo.Clear();
                txtReceiptNumber.Clear();
                
                LoadExpenses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت مصرف: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ShipmentSimple
    {
        public int Id { get; set; }
        public string ShipmentNumber { get; set; } = "";
    }

    public class Expense
    {
        public string ExpenseDate { get; set; } = "";
        public string ExpenseType { get; set; } = "";
        public double Amount { get; set; }
        public string Currency { get; set; } = "";
        public string PaidTo { get; set; } = "";
        public string ReceiptNumber { get; set; } = "";
    }
}
