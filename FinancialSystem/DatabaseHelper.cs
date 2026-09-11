using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace FinancialSystem
{
    public static class DatabaseHelper
    {
        private static string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FinancialSystem.db");
        
        public static string ConnectionString => $"Data Source={_dbPath}";
        
        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                
                // Users table for login
                CreateUsersTable(connection);
                
                // General Balance table
                CreateGeneralBalanceTable(connection);
                
                // Customers table
                CreateCustomersTable(connection);
                
                // Customer Transactions table
                CreateCustomerTransactionsTable(connection);
                
                // Shipments table
                CreateShipmentsTable(connection);
                
                // Shipment Status table
                CreateShipmentStatusTable(connection);
                
                // Shipment Expenses table
                CreateShipmentExpensesTable(connection);
                
                // Dashboard summary will be calculated from transactions
            }
        }
        
        private static void CreateUsersTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    FullName TEXT,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            cmd.ExecuteNonQuery();
            
            // Insert default admin user
            cmd.CommandText = @"
                INSERT OR IGNORE INTO Users (Username, Password, FullName) 
                VALUES ('admin', 'admin123', 'Administrator')";
            cmd.ExecuteNonQuery();
        }
        
        private static void CreateGeneralBalanceTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS GeneralBalance (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TransactionDate DATETIME NOT NULL,
                    Details TEXT,
                    Amount REAL NOT NULL,
                    TransactionType TEXT NOT NULL, -- 'Income' or 'Expense'
                    AccountName TEXT,
                    DocumentNumber TEXT,
                    TransactionCategory TEXT,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            cmd.ExecuteNonQuery();
        }
        
        private static void CreateCustomersTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerCode TEXT UNIQUE,
                    FirstName TEXT NOT NULL,
                    LastName TEXT,
                    PhoneNumber TEXT,
                    Address TEXT,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            cmd.ExecuteNonQuery();
        }
        
        private static void CreateCustomerTransactionsTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS CustomerTransactions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerId INTEGER NOT NULL,
                    TransactionDate DATETIME NOT NULL,
                    Details TEXT,
                    Quantity REAL,
                    Price REAL,
                    Description TEXT,
                    ReceiptNumber TEXT,
                    Balance REAL,
                    DocumentNumber TEXT,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
                )";
            cmd.ExecuteNonQuery();
        }
        
        private static void CreateShipmentsTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Shipments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShipmentNumber TEXT UNIQUE,
                    OriginCountry TEXT,
                    OriginCity TEXT,
                    DestinationCountry TEXT,
                    DestinationCity TEXT,
                    SendDate DATETIME,
                    Status TEXT,
                    TransportCompany TEXT,
                    VehicleNumber TEXT,
                    DriverName TEXT,
                    CargoType TEXT,
                    PackageCount INTEGER,
                    Weight REAL,
                    CargoValue REAL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            cmd.ExecuteNonQuery();
        }
        
        private static void CreateShipmentStatusTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ShipmentStatus (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShipmentId INTEGER NOT NULL,
                    Status TEXT NOT NULL,
                    StatusDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Notes TEXT,
                    FOREIGN KEY (ShipmentId) REFERENCES Shipments(Id)
                )";
            cmd.ExecuteNonQuery();
        }
        
        private static void CreateShipmentExpensesTable(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ShipmentExpenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShipmentId INTEGER,
                    ExpenseDate DATETIME NOT NULL,
                    ExpenseType TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Currency TEXT,
                    PaidFromAccount TEXT,
                    PaidTo TEXT,
                    ReceiptNumber TEXT,
                    Details TEXT,
                    DocumentNumber TEXT,
                    FOREIGN KEY (ShipmentId) REFERENCES Shipments(Id)
                )";
            cmd.ExecuteNonQuery();
        }
    }
}
