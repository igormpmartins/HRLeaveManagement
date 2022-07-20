using AutoMapper;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveRequestService : BaseHttpService, ILeaveRequestService
    {
        private readonly IMapper mapper;
        private readonly IClient client;
        private readonly ILocalStorageService localStorageService;

        public LeaveRequestService(IMapper mapper, IClient client, ILocalStorageService localStorageService) : base (client, localStorageService)
        {
            this.mapper = mapper;
            this.client = client;
            this.localStorageService = localStorageService;
        }

        public async Task ApproveLeaveRequest(int id, bool approved)
        {
            AddBearerToken();
            try
            {
                var request = new ChangeLeaveRequestApprovalDto { Id = id, Approved = approved };
                await client.ChangeapprovalAsync(id, request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Response<int>> CreateLeaveRequest(CreateLeaveRequestVM leaveRequest)
        {
            try
            {
                var response = new Response<int>();
                var createLeaveRequest = mapper.Map<CreateLeaveRequestDto>(leaveRequest);
                AddBearerToken();

                var apiResponse = await client.LeaveRequestsPOSTAsync(createLeaveRequest);
                if (apiResponse.Success)
                {
                    response.Data = apiResponse.Id;
                    response.Success = true;
                }
                else
                {
                    foreach (var error in apiResponse.Errors)
                        response.ValidationErrors += error + Environment.NewLine;
                }
                return response;
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

        public Task DeleteLeaveRequest(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
        {
            AddBearerToken();
            var leaveRequests = await client.LeaveRequestsAllAsync(false);

            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequests.Count,
                ApprovedRequests = leaveRequests.Count(q => q.Approved.GetValueOrDefault() == true),
                PendingRequests = leaveRequests.Count(q => !q.Approved.HasValue),
                RejectedRequests = leaveRequests.Count(q => q.Approved.HasValue && !q.Approved.Value),
                LeaveRequests = mapper.Map<List<LeaveRequestVM>>(leaveRequests)
            };

            return model;
        }

        public async Task<LeaveRequestVM> GetLeaveRequest(int id)
        {
            AddBearerToken();
            var leaveRequest = await client.LeaveRequestsGETAsync(id);
            return mapper.Map<LeaveRequestVM>(leaveRequest);
        }

        public async Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests()
        {
            AddBearerToken();
            var leaveRequests = await client.LeaveRequestsAllAsync(true);
            var leaveAllocations = await client.LeaveAllocationsAllAsync(true);

            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = mapper.Map<List<LeaveAllocationVM>>(leaveAllocations),
                LeaveRequests = mapper.Map<List<LeaveRequestVM>>(leaveRequests)
            };

            return model;
        }
    }
}
