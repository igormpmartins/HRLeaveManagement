using AutoMapper;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation.Validators;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistance;
using HR.LeaveManagement.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Responses;
using System.Linq;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Handlers.Commands
{
    public class CreateLeaveAllocationCommandHandler : IRequestHandler<CreateLeaveAllocationCommand, BaseCommandResponse>
    {
        private readonly ILeaveAllocationRepository leaveAllocationRepository;
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public CreateLeaveAllocationCommandHandler(
            ILeaveAllocationRepository leaveAllocationRepository, 
            ILeaveTypeRepository leaveTypeRepository,
            IUserService userService,
            IMapper mapper)
        {
            this.leaveAllocationRepository = leaveAllocationRepository;
            this.leaveTypeRepository = leaveTypeRepository;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<BaseCommandResponse> Handle(CreateLeaveAllocationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new CreateLeaveAllocationDtoValidator(leaveTypeRepository);

            var result = await validator.ValidateAsync(request.LeaveAllocationDto);
            if (!result.IsValid)
            {
                response.Success = false;
                response.Message = "Creation Failed";
                response.Errors = result.Errors.Select(q => q.ErrorMessage).ToList();
            } 
            else
            {
                var leaveType = await leaveTypeRepository.Get(request.LeaveAllocationDto.LeaveTypeId);
                var employees = await userService.GetEmployees();
                var period = DateTime.Now.Year;

                var allocations = new List<LeaveAllocation>();

                foreach (var emp in employees)
                {
                    if (await leaveAllocationRepository.AllocationExists(emp.Id, leaveType.Id, period))
                        continue;

                    allocations.Add(new LeaveAllocation
                    {
                        EmployeeId = emp.Id,
                        LeaveTypeId = leaveType.Id,
                        NumberOfDays = leaveType.DefaultDays,
                        Period = period
                    });
                }

                await leaveAllocationRepository.AddAllocations(allocations);

                response.Success = true;
                response.Message = "Creation Successful";
            }

            return response;
        }
    }
}
