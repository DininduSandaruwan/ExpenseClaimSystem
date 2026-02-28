using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Domain.Enums;

namespace ExpenseClaimSystem.Application.DTOs
{
    public class ExpenseClaimFilterDto
    {
        public ClaimStatus? Status { get; set; }
        public string? Department { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
