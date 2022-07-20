using AutoMapper;
using HR.LeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistance;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Infrastructure;
using HR.LeaveManagement.Application.Models;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HR.LeaveManagement.Application.Constants;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, BaseCommandResponse>
    {
        private readonly ILeaveRequestRepository leaveRequestRepository;
        private readonly ILeaveAllocationRepository leaveAllocationRepository;
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly IEmailSender emailSender;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public CreateLeaveRequestCommandHandler(
            ILeaveRequestRepository leaveRequestRepository, 
            ILeaveAllocationRepository leaveAllocationRepository,
            ILeaveTypeRepository leaveTypeRepository,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.leaveRequestRepository = leaveRequestRepository;
            this.leaveAllocationRepository = leaveAllocationRepository;
            this.leaveTypeRepository = leaveTypeRepository;
            this.emailSender = emailSender;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<BaseCommandResponse> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new CreateLeaveRequestDtoValidator(leaveTypeRepository);

            var result = await validator.ValidateAsync(request.LeaveRequestDto);

            var userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(q => q.Type == CustomClaimTypes.Uid)?.Value;
            var allocation = await leaveAllocationRepository.GetUserAllocations(userId, request.LeaveRequestDto.LeaveTypeId);
            int daysRequested = (int)(request.LeaveRequestDto.EndDate - request.LeaveRequestDto.StartDate).TotalDays;

            if (daysRequested > allocation.NumberOfDays)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.LeaveRequestDto.EndDate), "You do not have enough days for this request"));

            if (!result.IsValid)
            {
                response.Success = false;
                response.Message = "Creation Failed";
                response.Errors = result.Errors.Select(q => q.ErrorMessage).ToList();
                return response;
            }

            var leaveRequest = mapper.Map<LeaveRequest>(request.LeaveRequestDto);
            leaveRequest.RequestingEmployeeId = userId;
            leaveRequest = await leaveRequestRepository.Add(leaveRequest);

            response.Success = true;
            response.Message = "Creation Successful";
            response.Id = leaveRequest.Id;

            try
            {
                var emailAddress = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                var email = new Email
                {
                    To = emailAddress,
                    Body = $"Your leave request for {request.LeaveRequestDto.StartDate:D} to {request.LeaveRequestDto.EndDate:D}" +
                    " has been submitted successfully.",
                    Subject = "Leave Request Submitted"
                };
                await emailSender.SendEmail(email);
            }
            catch (Exception ex)
            {

                //TODO: log, etc
                //throw;
            }

            return response;
        }
    }
}
