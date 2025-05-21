using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI_s.DTO
{
    public class AppointmentWithRequest
    {
        public int AppointmentWithID { get; set; }

        [Required]
        [StringLength(100)]  // You can specify a maximum length if needed
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]  // You can specify a maximum length for the role
        public string Role { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
