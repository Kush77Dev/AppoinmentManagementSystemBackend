using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentAPI_s.Models
{
    public enum MeetingTypeEnum
    {
        Virtual = 1,
        InPerson = 2
    }

    public enum AppointmentStatusEnum
    {
        Scheduled = 1,
        Booked = 2,
        OnGoing = 3,
        Completed = 4,
        Canceled = 5
    }

    public class Appointments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentID { get; set; }

        // Change this to nullable
        public int? CreatedByHRAdmin { get; set; } // Nullable

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public MeetingTypeEnum MeetingType { get; set; } = MeetingTypeEnum.Virtual;

        [Required]
        public DateTime AppointmentDate { get; set; }

        public AppointmentStatusEnum Status { get; set; }

        public string? CancellationReason { get; set; }

        public int? ParentAppointmentID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // FK: CreatedByHRAdmin (User)
        [ForeignKey("CreatedByHRAdmin")]
        public virtual Users CreatedByUser { get; set; } = null!;

        // FK: AppointmentWith
        [Required]
        public int AppointmentWithID { get; set; }

        [ForeignKey("AppointmentWithID")]
        public virtual AppointmentWith AppointmentWith { get; set; } = null!;

        // Navigation for Timeslots
        public virtual ICollection<Timeslot> Timeslots { get; set; } = new List<Timeslot>();
    }

}
