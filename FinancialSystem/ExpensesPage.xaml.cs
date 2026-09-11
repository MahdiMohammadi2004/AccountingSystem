using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class ExpensesPage : Page
    {
        public ExpensesPage()
        {
            InitializeComponent();
            LoadExpenses();
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
                            date(ExpenseDate) as ExpenseDate,
                            ExpenseType,
                            Amount,
                            Currency,
                            PaidTo,
                            ReceiptNumber,
                            Details
                        FROM ShipmentExpenses
                        ORDER BY ExpenseDate DESC";
                    
                    var expenses = new List<ExpenseEntry>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        expenses.Add(new ExpenseEntry
                        {
                            ExpenseDate = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            ExpenseType = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Amount = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                            Currency = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            PaidTo = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            ReceiptNumber = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            Details = reader.IsDBNull(6) ? "" : reader.GetString(6)
                        });
                    }
                    
                    dgExpenses.ItemsSource = expenses;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading expenses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ExpenseEntry
    {
        public string ExpenseDate { get; set; } = "";
        public string ExpenseType { get; set; } = "";
        public double Amount { get; set; }
        public string Currency { get; set; } = "";
        public string PaidTo { get; set; } = "";
        public string ReceiptNumber { get; set; } = "";
        public string Details { get; set; } = "";
    }
}
