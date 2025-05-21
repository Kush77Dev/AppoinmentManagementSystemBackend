using Microsoft.EntityFrameworkCore;
using AppointmentAPI_s.Models;

namespace AppointmentAPI_s.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<Timeslot> Timeslots { get; set; }
        public DbSet<AppointmentBooking> AppointmentBookings { get; set; }

        // 🔽 NEW: Add DbSet for AppointmentWith
        public DbSet<AppointmentWith> AppointmentWiths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed roles
            modelBuilder.Entity<Roles>().HasData(
                new Roles { RoleId = 1, RoleName = "SuperAdmin" },
                new Roles { RoleId = 2, RoleName = "HRAdmin" },
                new Roles { RoleId = 3, RoleName = "User" }
            );

            // Configure Appointments → Users
            modelBuilder.Entity<Appointments>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByHRAdmin)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Timeslot → Appointments
            modelBuilder.Entity<Timeslot>()
                .HasOne(t => t.Appointment)
                .WithMany(a => a.Timeslots)
                .HasForeignKey(t => t.AppointmentID)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            // Configure AppointmentBooking → Timeslot
            modelBuilder.Entity<AppointmentBooking>()
                .HasOne(ab => ab.TimeSlot)
                .WithMany()
                .HasForeignKey(ab => ab.TimeSlotID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure AppointmentBooking → User
            modelBuilder.Entity<AppointmentBooking>()
                .HasOne(ab => ab.User)
                .WithMany()
                .HasForeignKey(ab => ab.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔽 NEW: Configure Appointments → AppointmentWith
            modelBuilder.Entity<Appointments>()
                .HasOne(a => a.AppointmentWith)
                .WithMany(w => w.Appointments)
                .HasForeignKey(a => a.AppointmentWithID)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔽 NEW: Configure Timeslot → AppointmentWith
            modelBuilder.Entity<Timeslot>()
                .HasOne(t => t.AppointmentWith)
                .WithMany(w => w.Timeslots)
                .HasForeignKey(t => t.AppointmentWithID)
                .OnDelete(DeleteBehavior.Cascade); // Keep Cascade for AppointmentWith

            base.OnModelCreating(modelBuilder);
        }
    }
}
