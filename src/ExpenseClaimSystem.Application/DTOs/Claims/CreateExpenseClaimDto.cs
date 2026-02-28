using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Domain.Enums;

namespace ExpenseClaimSystem.Application.DTOs
{
    public class CreateExpenseClaimDto
    {
        public string Department { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; } = 0.0;

        public string Currency { get; set; } = "LKR";

        public PaymentMethod PaymentMethod { get; set; }

        public string? BankNameCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }

        public List<ExpenseItemDto> Items { get; set; } = new();
        public List<ExpenseAttachmentDto> Attachments { get; set; } = new();
    }
}
