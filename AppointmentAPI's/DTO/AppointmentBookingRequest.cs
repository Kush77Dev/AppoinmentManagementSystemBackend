using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI_s.DTO
{
    public class AppointmentBookingRequest
    {
        [Required]
        public int AppointmentBookingID { get; set; }

        [Required]
        public int TimeSlotID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime BookedOn { get; set; } = DateTime.Now;

        public bool IsBooked { get; set; } = false; // Initially set to false until confirmed

    }
}
