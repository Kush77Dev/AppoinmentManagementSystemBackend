using System.ComponentModel.DataAnnotations;
using AppointmentAPI_s.Models;

namespace AppointmentAPI_s.DTO
{
    public class AppointmentRequest
    {
        // Now nullable to align with Appointments model
        public int? CreatedByHRAdmin { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public MeetingTypeEnum MeetingType { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public AppointmentStatusEnum Status { get; set; } = AppointmentStatusEnum.Scheduled;

        public string? CancellationReason { get; set; }

        // Parent appointment field is kept as nullable
        public int? ParentAppointmentID { get; set; }

        // New field for AppointmentWithID (to represent the user or entity the appointment is with)
        [Required]
        public int AppointmentWithID { get; set; }
    }
}
