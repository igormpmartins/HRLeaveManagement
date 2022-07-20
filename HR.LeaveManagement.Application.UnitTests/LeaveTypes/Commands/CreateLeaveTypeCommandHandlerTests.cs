using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistance;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Profiles;
using HR.LeaveManagement.Application.Responses;
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

namespace HR.LeaveManagement.Application.UnitTests.LeaveTypes.Commands
{
    public class CreateLeaveTypeCommandHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> mockRepo;
        private readonly IMapper mapper;
        private readonly CreateLeaveTypeDto leaveTypeDto;
        private readonly CreateLeaveTypeCommandHandler handler;

        public CreateLeaveTypeCommandHandlerTests()
        {
            mockRepo = MockLeaveTypeRepository.GetLeaveTypeRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MappingProfile>();
            });

            mapper = mapperConfig.CreateMapper();

            leaveTypeDto = new CreateLeaveTypeDto
            {
                DefaultDays = 15,
                Name = "Test DTO"
            };

            handler = new CreateLeaveTypeCommandHandler(mockRepo.Object, mapper);

        }

        [Fact]
        public async Task Valid_LeaveType_Added()
        {
            var result = await handler.Handle(new CreateLeaveTypeCommand()
            { 
                LeaveTypeDto = leaveTypeDto
            }, CancellationToken.None);

            var leaveTypes = await mockRepo.Object.GetAll();

            result.ShouldBeOfType<BaseCommandResponse>();
            leaveTypes.Count.ShouldBe(4);
        }        
        
        [Fact]
        public async Task Invalid_LeaveType_Added()
        {
            leaveTypeDto.DefaultDays = -1;

            var result = await handler
                .Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = leaveTypeDto }, CancellationToken.None);

            var leaveTypes = await mockRepo.Object.GetAll();
            leaveTypes.Count.ShouldBe(3);

            result.ShouldBeOfType<BaseCommandResponse>();
        }
    }
}
