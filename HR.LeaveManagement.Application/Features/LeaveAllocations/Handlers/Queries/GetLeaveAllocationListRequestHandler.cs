using AutoMapper;
using HR.LeaveManagement.Application.DTOs;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Queries;
using HR.LeaveManagement.Application.Contracts.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Identity;
using Microsoft.AspNetCore.Http;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Application.Constants;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Handlers.Queries
{
    public class GetLeaveAllocationListRequestHandler : IRequestHandler<GetLeaveAllocationListRequest, List<LeaveAllocationDto>>
    {
        private readonly ILeaveAllocationRepository leaveAllocationRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public GetLeaveAllocationListRequestHandler(
            ILeaveAllocationRepository leaveAllocationRepository, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            this.leaveAllocationRepository = leaveAllocationRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }

        public async Task<List<LeaveAllocationDto>> Handle(GetLeaveAllocationListRequest request, CancellationToken cancellationToken)
        {
            var leaveRequests = new List<LeaveAllocation>();
            var allocations = new List<LeaveAllocationDto>();

            if (request.IsLoggedInUser)
            {
                var userId = httpContextAccessor.HttpContext.User
                    .FindFirst(q => q.Type == CustomClaimTypes.Uid)?.Value;

                leaveRequests = await leaveAllocationRepository.GetLeaveAllocationsWithDetails(userId);
                var employee = await userService.GetEmployee(userId);
                allocations = mapper.Map<List<LeaveAllocationDto>>(leaveRequests);

                foreach (var allocation in allocations)
                    allocation.Employee = employee;

            }
            else
            {
                var list = await leaveAllocationRepository.GetLeaveAllocationsWithDetails();
                allocations = mapper.Map<List<LeaveAllocationDto>>(list);

                foreach (var allocation in allocations)
                    allocation.Employee = await userService.GetEmployee(allocation.EmployeeId);
            }

            return allocations;
        }
    }
}
