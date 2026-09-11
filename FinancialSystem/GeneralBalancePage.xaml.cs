using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class GeneralBalancePage : Page
    {
        public GeneralBalancePage()
        {
            InitializeComponent();
            dpTransactionDate.SelectedDate = DateTime.Today;
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            date(TransactionDate),
                            Details,
                            TransactionType,
                            Amount,
                            AccountName,
                            DocumentNumber
                        FROM GeneralBalance
                        ORDER BY TransactionDate DESC";
                    
                    var transactions = new List<Transaction>();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        transactions.Add(new Transaction
                        {
                            TransactionDate = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            Details = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Type = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Amount = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                            AccountName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            DocumentNumber = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                    
                    dgTransactions.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری داده‌ها: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dpTransactionDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("لطفاً تاریخ را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(txtAmount.Text, out double amount))
                {
                    MessageBox.Show("مبلغ وارد شده معتبر نیست", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string transactionType = ((ComboBoxItem)cmbTransactionType.SelectedItem).Content.ToString() == "آمد" ? "Income" : "Expense";

                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO GeneralBalance (TransactionDate, Details, Amount, TransactionType, AccountName, DocumentNumber)
                        VALUES (@date, @details, @amount, @type, @account, @docNumber)";
                    
                    cmd.Parameters.AddWithValue("@date", dpTransactionDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@details", txtDetails.Text);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@type", transactionType);
                    cmd.Parameters.AddWithValue("@account", txtAccountName.Text);
                    cmd.Parameters.AddWithValue("@docNumber", txtDocumentNumber.Text);
                    
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("معامله با موفقیت ثبت شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Clear form
                txtAmount.Clear();
                txtAccountName.Clear();
                txtDocumentNumber.Clear();
                txtDetails.Clear();
                
                LoadTransactions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت معامله: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Transaction
    {
        public string TransactionDate { get; set; } = "";
        public string Details { get; set; } = "";
        public string Type { get; set; } = "";
        public double Amount { get; set; }
        public string AccountName { get; set; } = "";
        public string DocumentNumber { get; set; } = "";
    }
}
