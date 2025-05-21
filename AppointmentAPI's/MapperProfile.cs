using AppointmentAPI_s.DTO;
using AppointmentAPI_s.Models;
using AutoMapper;

namespace AppointmentAPI_s
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserRequest, Users>();
            CreateMap<RoleRequest, Roles>();
            CreateMap<UserHRAdmin, Users>();
            CreateMap<AppointmentRequest,Appointments>();
            CreateMap<TimeSlotRequest, Timeslot>();
            CreateMap<AppointmentBookingRequest, AppointmentBooking>();
            CreateMap<AppointmentWith, AppointmentWithRequest>();
            CreateMap<AppointmentWithRequest, AppointmentWith>();



        }
    }
}
