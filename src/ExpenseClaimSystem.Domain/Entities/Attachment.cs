using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseClaimSystem.Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ExpenseClaimId { get; set; }
        public ExpenseClaim ExpenseClaim { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string AttachmentType { get; set; } = string.Empty;
    }
}
