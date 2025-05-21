using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentAPI_s.DTO
{
    public class RoleRequest
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
