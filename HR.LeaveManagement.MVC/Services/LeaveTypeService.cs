using AutoMapper;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveTypeService : BaseHttpService, ILeaveTypeService
    {
        private readonly IMapper mapper;
        private readonly IClient client;
        private readonly ILocalStorageService localStorage;

        public LeaveTypeService(IMapper mapper, IClient client, ILocalStorageService localStorage) : base(client, localStorage)
        {
            this.mapper = mapper;
            this.client = client;
            this.localStorage = localStorage;
        }

        public async Task<Response<int>> CreateLeaveType(CreateLeaveTypeVM leaveType)
        {
            try
            {
                var response = new Response<int>();
                var createLeaveType = mapper.Map<CreateLeaveTypeDto>(leaveType);
                AddBearerToken();
                var apiResponse = await client.LeaveTypesPOSTAsync(createLeaveType);

                if (apiResponse.Success)
                {
                    response.Data = apiResponse.Id;
                    response.Success = true;
                } else
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

        public async Task<Response<int>> DeleteLeaveType(int id)
        {
            try
            {
                AddBearerToken();
                await client.LeaveTypesDELETEAsync(id);
                return new Response<int> { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int> (ex);
            }
        }

        public async Task<LeaveTypeVM> GetLeaveTypeDetails(int id)
        {
            AddBearerToken();
            var leaveType = await client.LeaveTypesGETAsync(id);
            return mapper.Map<LeaveTypeVM>(leaveType);
        }

        public async Task<List<LeaveTypeVM>> GetLeaveTypes()
        {
            AddBearerToken();
            var leaveTypes = await client.LeaveTypesAllAsync();
            return mapper.Map<List<LeaveTypeVM>>(leaveTypes);
        }

        public async Task<Response<int>> UpdateLeaveType(int id, LeaveTypeVM leaveType)
        {
            try
            {
                var leaveTypeDto = mapper.Map<LeaveTypeDto>(leaveType);
                AddBearerToken();
                await client.LeaveTypesPUTAsync(leaveTypeDto);
                return new Response<int> { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }
    }
}
