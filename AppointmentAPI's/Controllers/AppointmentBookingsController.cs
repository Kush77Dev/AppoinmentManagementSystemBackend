using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI_s.Models;
using AutoMapper;
using AppointmentAPI_s.DTO;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentBookingsController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AppointmentBookingsController(UserContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET: api/AppointmentBookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentBooking>>> GetAppointmentBookings()
        {
            var appointmentBookings = await _context.AppointmentBookings
                                                     .Include(ab => ab.TimeSlot)
                                                     .Include(ab => ab.User)
                                                     .ToListAsync();
            return Ok(appointmentBookings);
        }

        // GET: api/AppointmentBookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentBooking>> GetAppointmentBooking(int id)
        {
            var appointmentBooking = await _context.AppointmentBookings
                                                    .Include(ab => ab.TimeSlot)
                                                    .Include(ab => ab.User)
                                                    .FirstOrDefaultAsync(ab => ab.BookingID == id);

            if (appointmentBooking == null)
            {
                return NotFound();
            }

            return Ok(appointmentBooking);
        }

        // GET: api/AppointmentBookings/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AppointmentBooking>>> GetAppointmentsByUser(int userId)
        {
            var userAppointments = await _context.AppointmentBookings
                                                 .Include(ab => ab.TimeSlot)
                                                 .Include(ab => ab.User)
                                                 .Where(ab => ab.UserID == userId)
                                                 .ToListAsync();

            if (!userAppointments.Any())
            {
                return NotFound(new { message = "No appointments found for this user." });
            }

            return Ok(userAppointments);
        }

        // GET: api/AppointmentBookings/checkAvailability/5
        [HttpGet("checkAvailability/{timeSlotID}")]
        public async Task<IActionResult> CheckAvailability(int timeSlotID)
        {
            var booking = await _context.AppointmentBookings
                                         .FirstOrDefaultAsync(ab => ab.TimeSlotID == timeSlotID);

            if (booking != null)
            {
                return Ok(new { isBooked = true, userID = booking.UserID, bookedOn = booking.BookedOn });
            }

            return Ok(new { isBooked = false });
        }

        // PUT: api/AppointmentBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointmentBooking(int id, [FromBody] AppointmentBookingRequest request)
        {
            if (id != request.AppointmentBookingID)
                return BadRequest("AppointmentBookingID mismatch.");

            var appointmentBooking = await _context.AppointmentBookings.FindAsync(id);

            if (appointmentBooking == null)
            {
                return NotFound("AppointmentBooking not found.");
            }

            _mapper.Map(request, appointmentBooking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/AppointmentBookings
        [HttpPost]
        public async Task<ActionResult<AppointmentBooking>> PostAppointmentBooking(AppointmentBookingRequest request)
        {
            // Check if TimeSlot is already booked
            var existingBooking = await _context.AppointmentBookings
                                                 .FirstOrDefaultAsync(ab => ab.TimeSlotID == request.TimeSlotID);

            if (existingBooking != null)
            {
                return Conflict(new { message = "This time slot is already booked." });
            }

            // Map the request to entity
            var appointmentBooking = _mapper.Map<AppointmentBooking>(request);
            appointmentBooking.IsBooked = true;
            appointmentBooking.BookedOn = DateTime.Now;

            _context.AppointmentBookings.Add(appointmentBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointmentBooking), new { id = appointmentBooking.BookingID }, appointmentBooking);
        }

        // DELETE: api/AppointmentBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentBooking(int id)
        {
            var appointmentBooking = await _context.AppointmentBookings.FindAsync(id);
            if (appointmentBooking == null)
            {
                return NotFound();
            }

            _context.AppointmentBookings.Remove(appointmentBooking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentBookingExists(int id)
        {
            return _context.AppointmentBookings.Any(e => e.BookingID == id);
        }
    }
}
