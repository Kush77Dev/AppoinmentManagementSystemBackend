using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAPI_s.Models
{
    public class Timeslot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimeSlotID { get; set; }

        [Required]
        public int AppointmentID { get; set; }

        [ForeignKey("AppointmentID")]
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual Appointments Appointment { get; set; }

        [Required]
        public TimeOnly TimeFrom { get; set; }

        [Required]
        public TimeOnly TimeTo { get; set; }

        [NotMapped]
        public int Duration
        {
            get
            {
                var from = TimeFrom.ToTimeSpan();
                var to = TimeTo.ToTimeSpan();
                return (int)(to - from).TotalMinutes;
            }
        }

        public bool IsAvailable { get; set; } = true;

        // 🔽 NEW FK: AppointmentWith
        [Required]
        public int AppointmentWithID { get; set; }

        [ForeignKey("AppointmentWithID")]
        public virtual AppointmentWith AppointmentWith { get; set; }
    }
}

