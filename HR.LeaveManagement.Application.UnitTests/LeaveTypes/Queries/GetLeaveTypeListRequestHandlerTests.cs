using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistance;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Queries;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Queries;
using HR.LeaveManagement.Application.Profiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.LeaveTypes.Queries
{
    public class GetLeaveTypeListRequestHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> mockRepo;
        private readonly IMapper mapper;

        public GetLeaveTypeListRequestHandlerTests()
        {
            mockRepo = MockLeaveTypeRepository.GetLeaveTypeRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MappingProfile>();
            });

            mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetLeaveTypeListTest()
        {
            var handler = new GetLeaveTypeListRequestHandler(mockRepo.Object, mapper);
            var result = await handler.Handle(new GetLeaveTypeListRequest(), CancellationToken.None);

            result.ShouldBeOfType<List<LeaveTypeDto>>();
            result.Count.ShouldBe(3);
            //result.ShouldBeEquivalentTo();
        }

    }
}
