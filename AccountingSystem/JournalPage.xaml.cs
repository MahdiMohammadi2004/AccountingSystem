using System;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using AccountingSystem.Database;

namespace AccountingSystem
{
    public partial class JournalPage : Page
    {
        public JournalPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Journal ORDER BY Date DESC";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    dgJournal.ItemsSource = null;
                    dgJournal.ItemsSource = reader;
                }
            }
        }
    }
}
