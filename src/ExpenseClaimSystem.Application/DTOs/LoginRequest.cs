using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExpenseClaimSystem.Application.DTOs
{

    public class LoginRequest
    {
        //[Required(ErrorMessage = "The Email field is required.")]
        //public string Email { get; set; } = string.Empty; 

        //[Required(ErrorMessage = "The Password field is required.")]
        //public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
