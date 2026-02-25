using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseClaimSystem.Application.DTOs
{
    public class ExpenseAttachmentDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string AttachmentType { get; set; } = string.Empty;
    }
}
