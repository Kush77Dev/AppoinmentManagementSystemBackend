using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentAPI_s.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("superadmin-data")]
        public IActionResult GetSuperAdminData()
        {
            return Ok("This is protected data for SuperAdmin.");
        }

        [Authorize(Roles = "HRAdmin")]
        [HttpGet("hradmin-data")]
        public IActionResult GetHRAdminData()
        {
            return Ok("This is protected data for HRAdmin.");
        }

        [Authorize(Roles = "User")]
        [HttpGet("user-data")]
        public IActionResult GetUserData()
        {
            return Ok("This is protected data for User.");
        }
    }
}
