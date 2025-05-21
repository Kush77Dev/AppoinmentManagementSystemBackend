using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI_s.Models;
using AppointmentAPI_s.DTO;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentWithsController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IMapper _mapper;

        public AppointmentWithsController(UserContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/AppointmentWiths
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentWith>>> GetAppointmentWiths()
        {
            var appointmentWiths = await _context.AppointmentWiths.ToListAsync();
            var appointmentWithDTOs = _mapper.Map<List<AppointmentWithRequest>>(appointmentWiths);
            return Ok(appointmentWithDTOs);
        }

        // GET: api/AppointmentWiths/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentWith>> GetAppointmentWith(int id)
        {
            var appointmentWith = await _context.AppointmentWiths.FindAsync(id);

            if (appointmentWith == null)
            {
                return NotFound();
            }

            var appointmentWithDTO = _mapper.Map<AppointmentWithRequest>(appointmentWith);
            return Ok(appointmentWithDTO);
        }

        // PUT: api/AppointmentWiths/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointmentWith(int id, AppointmentWithRequest appointmentWithDTO)
        {
            if (id != appointmentWithDTO.AppointmentWithID)
            {
                return BadRequest();
            }

            var appointmentWith = _mapper.Map<AppointmentWith>(appointmentWithDTO);
            _context.Entry(appointmentWith).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentWithExists(id))
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

        // POST: api/AppointmentWiths
        [HttpPost]
        public async Task<ActionResult<AppointmentWith>> PostAppointmentWith(AppointmentWithRequest appointmentWithDTO)
        {
            var appointmentWith = _mapper.Map<AppointmentWith>(appointmentWithDTO);
            _context.AppointmentWiths.Add(appointmentWith);
            await _context.SaveChangesAsync();

            var createdAppointmentWithDTO = _mapper.Map<AppointmentWithRequest>(appointmentWith);
            return CreatedAtAction("GetAppointmentWith", new { id = appointmentWith.AppointmentWithID }, createdAppointmentWithDTO);
        }

        // DELETE: api/AppointmentWiths/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentWith(int id)
        {
            var appointmentWith = await _context.AppointmentWiths.FindAsync(id);
            if (appointmentWith == null)
            {
                return NotFound();
            }

            _context.AppointmentWiths.Remove(appointmentWith);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentWithExists(int id)
        {
            return _context.AppointmentWiths.Any(e => e.AppointmentWithID == id);
        }
    }
}
