using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppointmentAPI_s.DTO;
using AppointmentAPI_s.Models;
using AppointmentAPI_s.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly JwtService _jwtService;

        public AuthController(UserContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest1 request)
        {
            var user = await _context.Users     
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            Console.WriteLine("Stored Hashed Password: " + user.Password);
            Console.WriteLine("Entered Password: " + request.Password);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            if (user.Roles == null)
            {
                return BadRequest("User role is not assigned.");
            }

            var token = _jwtService.GenerateToken(user.UserId,user.Email, user.Roles.RoleName, user.FullName);

            return Ok(new
            {
                Token = token,
                Roles = user.Roles.RoleName,
                UserId = user.UserId,           // ✅ Add this line
                FullName = user.FullName,       // (optional, helpful)
                Email = user.Email              // (optional)
            });

        }


        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Roles) // Include role data
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Roles.RoleName,
                    u.Department,
                    u.Designation,
                    u.status,
                    u.CreatedAt
                })
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequest request)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (existingUser)
            {
                return BadRequest("Email is already in use.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId);
            if (role == null)
            {
                return BadRequest("Invalid RoleId provided.");
            }

            var user = new Users
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                RoleId = role.RoleId,  // ✅ Assigning role
                Department = request.Department,
                Designation = request.Designation,
                status = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpGet("decode-token")]
        public IActionResult GetUserFromToken()
        {
            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Token Not Found or Invalid" });
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                Console.WriteLine("auth" + authorizationHeader);

                var user = new
                {
                    UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    Role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                    FullName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                };

                Console.WriteLine("UserId: " + user.UserId);
                Console.WriteLine("FullName: " + user.FullName);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Extract User ID from JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Invalid token.");
                }

                // Find the user in the database
                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Verify the current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
                {
                    return BadRequest("Current password is incorrect.");
                }

                // Hash and update the new password
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while changing the password.");
            }
        }



    }
}

