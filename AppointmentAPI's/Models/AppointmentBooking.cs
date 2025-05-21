using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentAPI_s.Models
{
    public class AppointmentBooking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingID { get; set; }

        [Required]
        public int TimeSlotID { get; set; }

        [ForeignKey("TimeSlotID")]
        public virtual Timeslot TimeSlot { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual Users User { get; set; }

        public DateTime BookedOn { get; set; } = DateTime.Now;

        public bool IsBooked { get; set; } = false; // Initially set to false until confirmed

    }
}
