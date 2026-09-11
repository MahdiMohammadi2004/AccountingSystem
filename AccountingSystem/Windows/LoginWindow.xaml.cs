using System;
using System.Linq;
using System.Windows;
using Microsoft.Data.Sqlite;
using AccountingSystem.Database;

namespace AccountingSystem
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("لطفاً نام کاربری و رمز عبور را وارد کنید");
                return;
            }

            using (var connection = DbHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    
                    if (count > 0)
                    {
                        // Login successful
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError("نام کاربری یا رمز عبور اشتباه است");
                    }
                }
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
