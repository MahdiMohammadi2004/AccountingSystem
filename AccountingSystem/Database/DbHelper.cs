using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace AccountingSystem.Database
{
    public static class DbHelper
    {
        private static string _dbPath = "";
        
        public static string ConnectionString => $"Data Source={_dbPath};";
        
        public static void InitializeDatabase()
        {
            // Set database path to application directory
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            _dbPath = Path.Combine(appPath, "AccountingSystem.db");
            
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                
                // Create Users table
                var createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        FullName TEXT,
                        Role TEXT DEFAULT 'User',
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                
                // Create GeneralBalance table
                var createGeneralBalance = @"
                    CREATE TABLE IF NOT EXISTS GeneralBalance (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date DATETIME NOT NULL,
                        Details TEXT,
                        Income DECIMAL(18,2) DEFAULT 0,
                        Expense DECIMAL(18,2) DEFAULT 0,
                        Balance DECIMAL(18,2),
                        DocumentNumber TEXT,
                        TransactionType TEXT,
                        RelatedAccount TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                
                // Create Journal table
                var createJournal = @"
                    CREATE TABLE IF NOT EXISTS Journal (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date DATETIME NOT NULL,
                        Details TEXT,
                        Income DECIMAL(18,2) DEFAULT 0,
                        Expense DECIMAL(18,2) DEFAULT 0,
                        Balance DECIMAL(18,2),
                        DocumentNumber TEXT,
                        RelatedAccount TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                
                // Create Customers table
                var createCustomers = @"
                    CREATE TABLE IF NOT EXISTS Customers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerCode TEXT UNIQUE NOT NULL,
                        FirstName TEXT NOT NULL,
                        LastName TEXT,
                        PhoneNumber TEXT,
                        Address TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                
                // Create CustomerAccounts table
                var createCustomerAccounts = @"
                    CREATE TABLE IF NOT EXISTS CustomerAccounts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerId INTEGER NOT NULL,
                        Date DATETIME NOT NULL,
                        Details TEXT,
                        Quantity INTEGER DEFAULT 0,
                        Price DECIMAL(18,2),
                        Description TEXT,
                        Receipts TEXT,
                        Balance DECIMAL(18,2),
                        FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
                    );";
                
                // Create Shipments table
                var createShipments = @"
                    CREATE TABLE IF NOT EXISTS Shipments (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ShipmentNumber TEXT UNIQUE NOT NULL,
                        OriginCountry TEXT,
                        OriginCity TEXT,
                        DestinationCountry TEXT,
                        DestinationCity TEXT,
                        SendDate DATETIME,
                        Status TEXT DEFAULT 'Registered',
                        TransportCompany TEXT,
                        VehicleNumber TEXT,
                        DriverName TEXT,
                        GoodsType TEXT,
                        PackageCount INTEGER,
                        Weight DECIMAL(18,2),
                        GoodsValue DECIMAL(18,2),
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                
                // Create ShipmentTracking table
                var createShipmentTracking = @"
                    CREATE TABLE IF NOT EXISTS ShipmentTracking (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ShipmentId INTEGER NOT NULL,
                        Status TEXT NOT NULL,
                        StatusDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Notes TEXT,
                        FOREIGN KEY (ShipmentId) REFERENCES Shipments(Id)
                    );";
                
                // Create ShipmentExpenses table
                var createShipmentExpenses = @"
                    CREATE TABLE IF NOT EXISTS ShipmentExpenses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ShipmentId INTEGER NOT NULL,
                        Date DATETIME NOT NULL,
                        ExpenseType TEXT NOT NULL,
                        Amount DECIMAL(18,2) NOT NULL,
                        Currency TEXT DEFAULT 'AFN',
                        PaidFromAccount TEXT,
                        PaidTo TEXT,
                        ReceiptNumber TEXT,
                        Details TEXT,
                        DocumentReference TEXT,
                        FOREIGN KEY (ShipmentId) REFERENCES Shipments(Id)
                    );";
                
                // Create AuditLog table
                var createAuditLog = @"
                    CREATE TABLE IF NOT EXISTS AuditLog (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Action TEXT NOT NULL,
                        TableName TEXT,
                        RecordId INTEGER,
                        UserId INTEGER,
                        Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Details TEXT
                    );";
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = createUsersTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createGeneralBalance;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createJournal;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createCustomers;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createCustomerAccounts;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createShipments;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createShipmentTracking;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createShipmentExpenses;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createAuditLog;
                    command.ExecuteNonQuery();
                }
                
                // Insert default admin user if not exists
                var checkUser = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
                using (var checkCommand = new SqliteCommand(checkUser, connection))
                {
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                    if (count == 0)
                    {
                        var insertAdmin = @"
                            INSERT INTO Users (Username, Password, FullName, Role) 
                            VALUES ('admin', 'admin123', 'Administrator', 'Admin');";
                        using (var insertCommand = new SqliteCommand(insertAdmin, connection))
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
