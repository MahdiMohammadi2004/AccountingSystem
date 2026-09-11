using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using FinancialSystem.Database;

namespace FinancialSystem
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        private void BtnDailyReport_Click(object sender, RoutedEventArgs e)
        {
            ShowReport("روزانه", DateTime.Today.AddDays(-1), DateTime.Today);
        }

        private void BtnWeeklyReport_Click(object sender, RoutedEventArgs e)
        {
            ShowReport("هفته‌وار", DateTime.Today.AddDays(-7), DateTime.Today);
        }

        private void BtnMonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            ShowReport("ماه‌وار", DateTime.Today.AddMonths(-1), DateTime.Today);
        }

        private void BtnYearlyReport_Click(object sender, RoutedEventArgs e)
        {
            ShowReport("سالانه", DateTime.Today.AddYears(-1), DateTime.Today);
        }

        private void ShowReport(string reportType, DateTime startDate, DateTime endDate)
        {
            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        SUM(Income) as TotalIncome,
                        SUM(Expense) as TotalExpense,
                        COUNT(*) as TransactionCount
                    FROM GeneralBalance
                    WHERE Date BETWEEN @start AND @end";
                
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@start", startDate);
                    command.Parameters.AddWithValue("@end", endDate);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal income = Convert.ToDecimal(reader["TotalIncome"]);
                            decimal expense = Convert.ToDecimal(reader["TotalExpense"]);
                            int count = Convert.ToInt32(reader["TransactionCount"]);
                            
                            txtReportResult.Text = $"گزارش {reportType}\n" +
                                                   $"تاریخ: {startDate:yyyy/MM/dd} تا {endDate:yyyy/MM/dd}\n\n" +
                                                   $"تعداد معاملات: {count}\n" +
                                                   $"مجموع درآمد: {income:N2}\n" +
                                                   $"مجموع مصارف: {expense:N2}\n" +
                                                   $"سود خالص: {(income - expense):N2}";
                        }
                    }
                }
            }
        }
    }
}
