using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using AccountingSystem.Database;

namespace AccountingSystem
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                
                // Calculate totals from GeneralBalance
                var query = @"
                    SELECT 
                        COALESCE(SUM(Income), 0) as TotalIncome,
                        COALESCE(SUM(Expense), 0) as TotalExpense,
                        COALESCE(SUM(CASE WHEN Income > 0 THEN Income ELSE 0 END), 0) as TotalComing,
                        COALESCE(SUM(CASE WHEN Expense > 0 THEN Expense ELSE 0 END), 0) as TotalGoing
                    FROM GeneralBalance";
                
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal totalIncome = Convert.ToDecimal(reader["TotalIncome"]);
                        decimal totalExpense = Convert.ToDecimal(reader["TotalExpense"]);
                        decimal totalComing = Convert.ToDecimal(reader["TotalComing"]);
                        decimal totalGoing = Convert.ToDecimal(reader["TotalGoing"]);
                        decimal netIncome = totalIncome - totalExpense;
                        
                        txtTotalComing.Text = totalComing.ToString("N2");
                        txtTotalGoing.Text = totalGoing.ToString("N2");
                        txtTotalIncome.Text = totalIncome.ToString("N2");
                        txtTotalExpense.Text = totalExpense.ToString("N2");
                        txtNetIncome.Text = netIncome.ToString("N2");
                        
                        if (netIncome >= 0)
                        {
                            txtNetIncome.Foreground = System.Windows.Media.Brushes.Green;
                        }
                        else
                        {
                            txtNetIncome.Foreground = System.Windows.Media.Brushes.Red;
                        }
                    }
                }
                
                // Get current balance
                var balanceQuery = "SELECT COALESCE(SUM(Income - Expense), 0) FROM GeneralBalance";
                using (var command = new SqliteCommand(balanceQuery, connection))
                {
                    var balance = Convert.ToDecimal(command.ExecuteScalar());
                    txtCurrentBalance.Text = balance.ToString("N2");
                }
            }
        }
    }
}
