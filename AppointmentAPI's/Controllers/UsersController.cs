using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI_s.Models;
using AutoMapper;
using AppointmentAPI_s.DTO;
using System.Security.Cryptography;
using System.Text;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        private readonly IMapper _mapper;
        IConfiguration _configuration;

        private readonly EmailService _emailService;

        public UsersController(UserContext context, IMapper mapper, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> Getusers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpGet("GetAllHRAdmins")]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllHRAdmins()
        {
            try
            {
                // Fetch the RoleId for "HRAdmin"
                var hrAdminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "HRAdmin");
                if (hrAdminRole == null)
                {
                    return NotFound("HRAdmin role not found.");
                }

                // Fetch users with the HRAdmin RoleId
                var hrAdmins = await _context.Users.Where(u => u.RoleId == hrAdminRole.RoleId).ToListAsync();

                if (hrAdmins == null || !hrAdmins.Any())
                {
                    return NotFound("No HR Admins found.");
                }

                return Ok(hrAdmins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.UserId)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("AddUser")]
        public async Task<ActionResult<Users>> AddUser(UserRequest usersreq)
        {
            try
            {
                if (usersreq == null || string.IsNullOrEmpty(usersreq.Email) || string.IsNullOrEmpty(usersreq.Password))
                {
                    return BadRequest("Invalid user data.");
                }

                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == usersreq.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Generate OTP
                string otpCode = new Random().Next(100000, 999999).ToString();
                DateTime otpExpiry = DateTime.UtcNow.AddMinutes(5);  // OTP valid for 5 minutes

                // Hash password
                string hashedPassword = UserHRAdmin.HashPassword(usersreq.Password);
                usersreq.Password = hashedPassword;

                // Map user and set OTP details
                Users newUser = _mapper.Map<UserRequest, Users>(usersreq);
                newUser.OTP = otpCode;
                newUser.OTPExpiration = otpExpiry;
                newUser.IsEmailVerified = false;
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Send OTP via Email
                await _emailService.SendOTPEmailAsync(newUser.Email, otpCode);

                return Ok(new { message = "User registered successfully. Please verify your email with the OTP sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





        [HttpPost("UserHRAdmin")]
        public async Task<ActionResult<Users>> PostUsersHRAdmin(UserHRAdmin usersreq)
        {
            try
            {
                if (usersreq == null || string.IsNullOrWhiteSpace(usersreq.Email))
                {
                    return BadRequest("Invalid user data.");
                }

                // Step 1: Generate a random password
                string rawPassword = UserHRAdmin.GenerateRandomPassword(); // generate here

                // Step 2: Send email with the raw password
                await _emailService.SendHRAdminCredentialsEmailAsync(usersreq.Email, rawPassword);

                // Step 3: Hash and assign it back
                usersreq.Password = UserHRAdmin.HashPassword(rawPassword);

                // Step 4: Save user
                Users newUser = _mapper.Map<UserHRAdmin, Users>(usersreq);
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsers), new { id = newUser.UserId }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // Function to generate a secure random password
        //private string GenerateRandomPassword(int length = 6)
        //{
        //    const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //    StringBuilder password = new StringBuilder();
        //    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        //    {
        //        byte[] randomBytes = new byte[length];
        //        rng.GetBytes(randomBytes);

        //        for (int i = 0; i < length; i++)
        //        {
        //            password.Append(validChars[randomBytes[i] % validChars.Length]);
        //        }
        //    }
        //    return password.ToString();
        //}





        [HttpPut("UserHRAdmin/{id}")]
        public async Task<IActionResult> PutUserHRAdmin(int id, UserHRAdmin usersreq)
        {
            try
            {
                if (usersreq == null || id != usersreq.UserId)
                {
                    return BadRequest("Invalid user data.");
                }

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                // Update fields
                existingUser.FullName = usersreq.FullName;
                existingUser.Email = usersreq.Email;
                existingUser.PhoneNumber = usersreq.PhoneNumber;
                existingUser.Designation = usersreq.Designation;
                existingUser.Department = usersreq.Department;

                // Update password only if a new one is provided
                if (!string.IsNullOrEmpty(usersreq.Password))
                {
                    existingUser.Password = UserHRAdmin.HashPassword(usersreq.Password);
                }

                _context.Entry(existingUser).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                existingUser.UpdatedAt = DateTime.UtcNow;

                return Ok(new { message = "HR Admin updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("UserHRAdmin/{id}/{status}")]
        public async Task<IActionResult> UpdateUserStatus(int id, bool status)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Update status
                user.status = status; // Ensure Status field exists in Users model
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPRequest otpRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == otpRequest.Email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Check if OTP is expired
            if (user.OTPExpiration < DateTime.UtcNow)
            {
                return BadRequest("OTP has expired. Please request a new one.");
            }

            if (user.OTP != otpRequest.OTP)
            {
                return BadRequest("Invalid OTP.");
            }

            // Mark email as verified
            user.IsEmailVerified = true;
            user.OTP = null; // Clear OTP
            user.OTPExpiration = null; // Clear expiry
            await _context.SaveChangesAsync();

            return Ok(new { message = "Email verified successfully. You can now log in." });
        }


        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP([FromBody] OTPRequest otpRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == otpRequest.Email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Generate new OTP
            string newOtp = new Random().Next(100000, 999999).ToString();
            DateTime newOtpExpiry = DateTime.UtcNow.AddMinutes(5);

            user.OTP = newOtp;
            user.OTPExpiration = newOtpExpiry;
            await _context.SaveChangesAsync();

            // Send new OTP via Email
            await _emailService.SendOTPEmailAsync(user.Email, newOtp);

            return Ok(new { message = "A new OTP has been sent to your email." });
        }



        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
