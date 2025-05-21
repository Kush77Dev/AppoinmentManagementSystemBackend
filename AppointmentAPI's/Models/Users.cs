using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentAPI_s.Models
{
    public class Users
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Column]
        public string FullName { get; set; } = null!;
        [Column]
        public string Email { get; set; } = null!;
        [Column]
        public string PhoneNumber { get; set; } = null!;
        [Column]
        public string Password { get; set; }
        [Column]
        [ForeignKey("Roles")]
        public int RoleId { get; set; }
        [Column]
        public string Department { get; set; }
        [Column]
        public string Designation { get; set; }

        [Column]
        public bool status { get; set; } = false;
        [Column]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } 

        public Roles? Roles { get; set; }


        // OTP Fields
        public string? OTP { get; set; }
        public DateTime? OTPExpiration { get; set; }
        public bool IsEmailVerified { get; set; } = false;



    }
}
