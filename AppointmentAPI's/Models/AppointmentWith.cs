using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentAPI_s.Models
{
    public class AppointmentWith
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentWithID { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Add this navigation property for Appointments
        public virtual ICollection<Appointments> Appointments { get; set; } = new List<Appointments>();

        // ✅ Add this navigation property for Timeslots
        public virtual ICollection<Timeslot> Timeslots { get; set; } = new List<Timeslot>();
    }
}
