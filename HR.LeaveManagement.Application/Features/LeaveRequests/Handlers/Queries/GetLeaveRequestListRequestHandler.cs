using AutoMapper;
using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using HR.LeaveManagement.Application.Contracts.Persistance;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Constants;
using HR.LeaveManagement.Domain;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Queries
{
    public class GetLeaveRequestListRequestHandler : IRequestHandler<GetLeaveRequestListRequest, List<LeaveRequestListDto>>
    {
        private readonly ILeaveRequestRepository leaveRequestRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;

        public GetLeaveRequestListRequestHandler(ILeaveRequestRepository leaveRequestRepository, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            this.leaveRequestRepository = leaveRequestRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }

        public async Task<List<LeaveRequestListDto>> Handle(GetLeaveRequestListRequest request, CancellationToken cancellationToken)
        {
            var leaveRequests = new List<LeaveRequest>();
            var requests = new List<LeaveRequestListDto>();

            if (request.IsLoggedInUser)
            {
                var userId = httpContextAccessor.HttpContext.User
                    .FindFirst(q => q.Type == CustomClaimTypes.Uid)?.Value;

                leaveRequests = await leaveRequestRepository.GetLeaveRequestsWithDetails(userId);
                var employee = await userService.GetEmployee(userId);
                requests = mapper.Map<List<LeaveRequestListDto>>(leaveRequests);

                foreach (var req in requests)
                    req.Employee = employee;

            } 
            else
            {
                var list = await leaveRequestRepository.GetLeaveRequestsWithDetails();
                requests = mapper.Map<List<LeaveRequestListDto>>(list);

                foreach (var req in requests)
                    req.Employee = await userService.GetEmployee(req.RequestingEmployeeId);

            }

            return requests;
        }
    }
}
