using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseClaimSystem.Domain.Entities
{
    public class ExpenseItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ExpenseClaimId { get; set; }

        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public bool IsGstApplicable { get; set; }
    }
}
