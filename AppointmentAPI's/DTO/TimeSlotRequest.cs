using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI_s.DTO
{
    public class TimeSlotRequest
    {
        [Required]
        public int TimeSlotID { get; set; }  // TimeSlotID added for referencing

        // Added AppointmentID as a required field
        [Required]
        public int AppointmentID { get; set; }

        [Required]
        public TimeOnly TimeFrom { get; set; }

        [Required]
        public TimeOnly TimeTo { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Calculated Duration
        public int Duration
        {
            get
            {
                var from = TimeFrom.ToTimeSpan();
                var to = TimeTo.ToTimeSpan();
                return (int)(to - from).TotalMinutes;
            }
        }

        public int AppointmentWithID { get; set; }
    }
}
