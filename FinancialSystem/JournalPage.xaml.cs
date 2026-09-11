using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class JournalPage : Page
    {
        public JournalPage()
        {
            InitializeComponent();
            LoadJournal();
        }

        private void LoadJournal()
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            date(TransactionDate) as Date,
                            Details,
                            CASE WHEN TransactionType = 'Income' THEN Amount ELSE 0 END as Income,
                            CASE WHEN TransactionType = 'Expense' THEN Amount ELSE 0 END as Expense,
                            Amount as Balance,
                            DocumentNumber,
                            AccountName
                        FROM GeneralBalance
                        ORDER BY TransactionDate DESC";
                    
                    var journalEntries = new List<JournalEntry>();
                    double runningBalance = 0;
                    
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        double income = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
                        double expense = reader.IsDBNull(3) ? 0 : reader.GetDouble(3);
                        
                        runningBalance += income - expense;
                        
                        journalEntries.Add(new JournalEntry
                        {
                            Date = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            Details = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Income = income,
                            Expense = expense,
                            Balance = runningBalance,
                            DocumentNumber = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            AccountName = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                    
                    dgJournal.ItemsSource = journalEntries;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری روزنامه‌چه: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class JournalEntry
    {
        public string Date { get; set; } = "";
        public string Details { get; set; } = "";
        public double Income { get; set; }
        public double Expense { get; set; }
        public double Balance { get; set; }
        public string DocumentNumber { get; set; } = "";
        public string AccountName { get; set; } = "";
    }
}
