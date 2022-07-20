﻿using AutoMapper;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateLeaveTypeDto, CreateLeaveTypeVM>().ReverseMap();
            CreateMap<CreateLeaveRequestDto, CreateLeaveRequestVM>().ReverseMap();

            CreateMap<LeaveRequestDto, LeaveRequestVM>()
                .ForMember(q => q.DateRequested, opt => opt.MapFrom(x => x.DateRequested.DateTime))
                .ForMember(q => q.StartDate, opt => opt.MapFrom(x => x.StartDate.Date))
                .ForMember(q => q.EndDate, opt => opt.MapFrom(x => x.EndDate.Date))
                .ReverseMap();

            CreateMap<LeaveRequestListDto, LeaveRequestVM>()
                .ForMember(q => q.DateRequested, opt => opt.MapFrom(x => x.DateRequested.DateTime))
                .ForMember(q => q.StartDate, opt => opt.MapFrom(x => x.StartDate.Date))
                .ForMember(q => q.EndDate, opt => opt.MapFrom(x => x.EndDate.Date))
                .ReverseMap();
            
            CreateMap<LeaveTypeDto, LeaveTypeVM>().ReverseMap();
            CreateMap<LeaveAllocationDto, LeaveAllocationVM>().ReverseMap();
            CreateMap<RegisterVM, RegistrationRequest>().ReverseMap();
            CreateMap<EmployeeVM, Employee>().ReverseMap();
        }
    }
}
