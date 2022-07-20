using HR.LeaveManagement.Application.Constants;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LeaveManagementDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private ILeaveAllocationRepository leaveAllocationRepository;
        private ILeaveRequestRepository leaveRequestRepository;
        private ILeaveTypeRepository leaveTypeRepository;

        public UnitOfWork(LeaveManagementDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ILeaveAllocationRepository LeaveAllocationRepository => leaveAllocationRepository ??= new LeaveAllocationRepository(dbContext);

        public ILeaveRequestRepository LeaveRequestRepository => leaveRequestRepository ??= new LeaveRequestRepository(dbContext);

        public ILeaveTypeRepository LeaveTypeRepository => leaveTypeRepository ??= new LeaveTypeRepository(dbContext);

        public void Dispose()
        {
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            var userName = httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.Uid)?.Value;
            await dbContext.SaveChangesAsync(userName);
        }
    }
}
