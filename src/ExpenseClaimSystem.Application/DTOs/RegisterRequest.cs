using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseClaimSystem.Application.DTOs
{
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
