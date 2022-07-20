using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveAllocationService : BaseHttpService, ILeaveAllocationService
    {
        private readonly IClient httpClient;
        private readonly ILocalStorageService localStorageService;

        public LeaveAllocationService(IClient httpClient, ILocalStorageService localStorageService) : base (httpClient, localStorageService)
        {
            this.httpClient = httpClient;
            this.localStorageService = localStorageService;
        }

        public async Task<Response<int>> CreateLeaveAllocations(int leaveTypeId)
        {
            try
            {
                var response = new Response<int>();
                var createLeaveAllocation = new CreateLeaveAllocationDto { LeaveTypeId = leaveTypeId };
                AddBearerToken();

                var apiResponse = await httpClient.LeaveAllocationsPOSTAsync(createLeaveAllocation);
                if (apiResponse.Success)
                    response.Success = true;
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
    }
}
