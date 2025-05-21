using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentAPI_s.DTO
{
    public class UserRequest
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }

        public string Department { get; set; } = null!;
        public string Designation { get; set; } = null!;

        // OTP Fields (Optional for Registration, Required for Verification)
        public string? OTP { get; set; }
        public DateTime? OTPExpiration { get; set; }
        public bool IsEmailVerified { get; set; } = false;


        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }


}
