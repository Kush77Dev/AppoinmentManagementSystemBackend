using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI_s.Models;
using AutoMapper;
using AppointmentAPI_s.DTO;
using System.Security.Claims;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AppointmentsController(UserContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            // Removed user authentication check

            // Fetch appointments for all users (no login check)
            var appointments = await _context.Appointments.ToListAsync();

            return Ok(appointments);
        }




        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointments>> GetAppointments(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // GET: api/Appointments/ByHRAdmin/{hrAdminID}
        [HttpGet("ByHRAdmin/{hrAdminID}")]
        public async Task<ActionResult<IEnumerable<Appointments>>> GetAppointmentsByHRAdmin(int hrAdminID)
        {
            var appointments = await _context.Appointments
                                             .Where(a => a.CreatedByHRAdmin == hrAdminID)
                                             .ToListAsync();

            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("No appointments found for this HR Admin.");
            }

            return appointments;
        }


        // PUT: api/Appointments/update
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointments(int id, [FromBody] AppointmentRequest request)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            // Mapping the request DTO to the appointment entity
            _mapper.Map(request, appointment);

            // Add the AppointmentWithID (if it's being updated from the request)
            appointment.AppointmentWithID = request.AppointmentWithID; // Add this line

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointments>> PostAppointments(AppointmentRequest request)
        {
            // Map the request DTO to the appointment entity
            var appointment = _mapper.Map<Appointments>(request);

            // Assuming AppointmentWithID is part of the request and is passed along with the appointment
            appointment.AppointmentWithID = request.AppointmentWithID; // Add this line

            // Adding the appointment to the database
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.AppointmentID }, appointment);
        }


        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointments(int id)
        {
            var appointment = await _context.Appointments.Include(a => a.Timeslots)
                                                           .FirstOrDefaultAsync(a => a.AppointmentID == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // First, delete related timeslot records
            _context.Timeslots.RemoveRange(appointment.Timeslots);

            // Then, delete the appointment
            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool AppointmentsExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentID == id);
        }
    }
}
