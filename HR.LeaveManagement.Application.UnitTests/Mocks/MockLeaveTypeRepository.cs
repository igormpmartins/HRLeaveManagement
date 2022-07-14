using HR.LeaveManagement.Application.Contracts.Persistance;
using HR.LeaveManagement.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public static class MockLeaveTypeRepository
    {
        public static Mock<ILeaveTypeRepository> GetLeaveTypeRepository()
        {
            var leaveTypes = new List<LeaveType>
            {
                new LeaveType { Id = 1, Name = "Test Vacation", DefaultDays = 10},
                new LeaveType { Id = 2, Name = "Test Sick", DefaultDays = 15},
                new LeaveType { Id = 2, Name = "Test Maternity", DefaultDays = 180}
            };

            var mockRepo = new Mock<ILeaveTypeRepository>();

            mockRepo.Setup(r => r.GetAll()).ReturnsAsync(leaveTypes);

            mockRepo.Setup(r => r.Add(It.IsAny<LeaveType>()))
                .ReturnsAsync((LeaveType leaveType) =>
                {
                    leaveTypes.Add(leaveType);
                    return leaveType;
                });

            return mockRepo;
        }
    }
}
