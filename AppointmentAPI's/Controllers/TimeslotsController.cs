using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI_s.Models;
using AutoMapper;
using AppointmentAPI_s.DTO;
using Microsoft.Extensions.Configuration;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeslotsController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IMapper _mapper;

        public TimeslotsController(UserContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Timeslots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Timeslot>>> GetTimeslots()
        {
            return await _context.Timeslots.ToListAsync();
        }

        // GET: api/Timeslots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Timeslot>> GetTimeslot(int id)
        {
            var timeslot = await _context.Timeslots.FindAsync(id);

            if (timeslot == null)
            {
                return NotFound();
            }

            return timeslot;
        }

        // Route: api/Timeslots/by-appointment/5
        [HttpGet("byAppointment/{appointmentID}")]
        public async Task<IActionResult> GetTimeSlotsByAppointment(int appointmentID)
        {
            var timeSlots = await _context.Timeslots
                .Where(t => t.AppointmentID == appointmentID)
                .ToListAsync();

            if (timeSlots == null || !timeSlots.Any())
                return NotFound("No Time Slots Available");

            return Ok(timeSlots);
        }

        // PUT: api/Timeslots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeslot(int id, [FromBody] TimeSlotRequest timeslotDTO)
        {
            if (id != timeslotDTO.TimeSlotID)
            {
                return BadRequest("TimeSlot ID mismatch.");
            }

            var timeslot = await _context.Timeslots.FindAsync(id);
            if (timeslot == null)
            {
                return NotFound("Timeslot not found.");
            }

            // Mapping the DTO to the existing Timeslot entity
            _mapper.Map(timeslotDTO, timeslot);

            // Add the AppointmentWithID if it's part of the request
            timeslot.AppointmentWithID = timeslotDTO.AppointmentWithID; // Add this line

            _context.Entry(timeslot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeslotExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Timeslots
        [HttpPost]
        public async Task<ActionResult<Timeslot>> PostTimeslot([FromBody] TimeSlotRequest timeslotDTO)
        {
            // Ensure the AppointmentID and AppointmentWithID are valid (this can be extended as needed)
            if (timeslotDTO.AppointmentID <= 0)
            {
                return BadRequest("Invalid Appointment ID.");
            }

            if (timeslotDTO.AppointmentWithID <= 0)
            {
                return BadRequest("Invalid Appointment With ID.");
            }

            var timeslot = _mapper.Map<Timeslot>(timeslotDTO);

            // Add the AppointmentWithID if it's part of the request
            timeslot.AppointmentWithID = timeslotDTO.AppointmentWithID; // Add this line

            _context.Timeslots.Add(timeslot);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTimeslot), new { id = timeslot.TimeSlotID }, timeslot);
        }


        // DELETE: api/Timeslots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeslot(int id)
        {
            var timeslot = await _context.Timeslots.FindAsync(id);
            if (timeslot == null)
            {
                return NotFound();
            }

            _context.Timeslots.Remove(timeslot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimeslotExists(int id)
        {
            return _context.Timeslots.Any(e => e.TimeSlotID == id);
        }
    }
}
