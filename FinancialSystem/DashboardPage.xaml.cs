using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadDashboardData(DateTime.Today, DateTime.Today);
        }

        private void LoadDashboardData(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    
                    // Calculate totals
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        SELECT 
                            SUM(CASE WHEN TransactionType = 'Income' THEN Amount ELSE 0 END) as TotalIncome,
                            SUM(CASE WHEN TransactionType = 'Expense' THEN Amount ELSE 0 END) as TotalExpense
                        FROM GeneralBalance
                        WHERE date(TransactionDate) BETWEEN date(@startDate) AND date(@endDate)";
                    
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                    
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        double totalIncome = reader.IsDBNull(0) ? 0 : reader.GetDouble(0);
                        double totalExpense = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                        double netProfit = totalIncome - totalExpense;
                        
                        txtTotalIncome.Text = totalIncome.ToString("N0");
                        txtTotalExpense.Text = totalExpense.ToString("N0");
                        txtNetProfit.Text = netProfit.ToString("N0");
                        txtCurrentBalance.Text = netProfit.ToString("N0");
                    }
                    
                    // Load transaction details
                    cmd.CommandText = @"
                        SELECT 
                            date(TransactionDate) as Date,
                            Details,
                            CASE WHEN TransactionType = 'Income' THEN Amount ELSE 0 END as Income,
                            CASE WHEN TransactionType = 'Expense' THEN Amount ELSE 0 END as Expense,
                            Amount as Balance
                        FROM GeneralBalance
                        WHERE date(TransactionDate) BETWEEN date(@startDate) AND date(@endDate)
                        ORDER BY TransactionDate DESC";
                    
                    var transactions = new List<DashboardTransaction>();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        transactions.Add(new DashboardTransaction
                        {
                            Date = reader.GetString(0),
                            Details = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Income = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                            Expense = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                            Balance = reader.IsDBNull(4) ? 0 : reader.GetDouble(4)
                        });
                    }
                    
                    dgDashboard.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری داده‌ها: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDaily_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData(DateTime.Today, DateTime.Today);
        }

        private void BtnWeekly_Click(object sender, RoutedEventArgs e)
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            LoadDashboardData(startOfWeek, DateTime.Today);
        }

        private void BtnMonthly_Click(object sender, RoutedEventArgs e)
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            LoadDashboardData(startOfMonth, DateTime.Today);
        }

        private void BtnYearly_Click(object sender, RoutedEventArgs e)
        {
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            LoadDashboardData(startOfYear, DateTime.Today);
        }

        private void BtnCustom_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpEndDate.SelectedDate = DateTime.Today;
        }

        private void BtnShowCustom_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                LoadDashboardData(dpStartDate.SelectedDate.Value, dpEndDate.SelectedDate.Value);
            }
            else
            {
                MessageBox.Show("لطفاً تاریخ شروع و پایان را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    public class DashboardTransaction
    {
        public string Date { get; set; } = "";
        public string Details { get; set; } = "";
        public double Income { get; set; }
        public double Expense { get; set; }
        public double Balance { get; set; }
    }
}
