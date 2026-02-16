using ExpenseClaimSystem.Domain.Enums;

namespace ExpenseClaimSystem.Domain.Entities
{
    public class ExpenseClaim
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestNumber { get; set; } = string.Empty;

        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public string Currency { get; set; } = "LKR";

        public PaymentMethod PaymentMethod { get; set; }

        public string? BankNameCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }

        public decimal SubTotal { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<ExpenseItem> Items { get; set; } = new List<ExpenseItem>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
