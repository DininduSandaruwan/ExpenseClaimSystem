using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseClaimSystem.Application.DTOs
{
    public class ExpenseItemDto
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsGstApplicable { get; set; }
    }
}
