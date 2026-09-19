using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreManagement.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set;} = string.Empty;
    }

    public class LoginResponseDto {

        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int UserType { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
