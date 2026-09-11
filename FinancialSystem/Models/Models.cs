using System;

namespace FinancialSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
    }

    public class GeneralBalance
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; } = "";
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
        public string DocumentNumber { get; set; } = "";
        public string TransactionType { get; set; } = "";
        public string RelatedAccount { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class JournalEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; } = "";
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
        public string DocumentNumber { get; set; } = "";
        public string RelatedAccount { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerAccount
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        public string Receipts { get; set; } = "";
        public decimal Balance { get; set; }
    }

    public class Shipment
    {
        public int Id { get; set; }
        public string ShipmentNumber { get; set; } = "";
        public string OriginCountry { get; set; } = "";
        public string OriginCity { get; set; } = "";
        public string DestinationCountry { get; set; } = "";
        public string DestinationCity { get; set; } = "";
        public DateTime SendDate { get; set; }
        public string Status { get; set; } = "Registered";
        public string TransportCompany { get; set; } = "";
        public string VehicleNumber { get; set; } = "";
        public string DriverName { get; set; } = "";
        public string GoodsType { get; set; } = "";
        public int PackageCount { get; set; }
        public decimal Weight { get; set; }
        public decimal GoodsValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ShipmentTracking
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public string Status { get; set; } = "";
        public DateTime StatusDate { get; set; }
        public string Notes { get; set; } = "";
    }

    public class ShipmentExpense
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public DateTime Date { get; set; }
        public string ExpenseType { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "AFN";
        public string PaidFromAccount { get; set; } = "";
        public string PaidTo { get; set; } = "";
        public string ReceiptNumber { get; set; } = "";
        public string Details { get; set; } = "";
        public string DocumentReference { get; set; } = "";
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = "";
        public string TableName { get; set; } = "";
        public int? RecordId { get; set; }
        public int? UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = "";
    }
}
