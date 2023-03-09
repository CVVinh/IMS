using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Enum.Handover;
using BE.Data.Models;
using BE.Data.Profiles.LeaveOffProfile;
using BE.Mapper;
using BE.Services.LeaveOffServices;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using static BE.Data.Enum.LeaveOff.Status;
using Users = BE.Data.Models.Users;

namespace XUnitServiceTestCase.ServiceTest
{
    [Collection("ServiceLeaveOff")]
    public class LeaveOffSeviceTest
    {
        private ILeaveOffServices _leaveOffServices;
        private AppDbContext _appDbContext;
        private IMapper _mapper;
        public LeaveOffSeviceTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "Database_Test")
        .Options;
            var appContext = new AppDbContext(options);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
                cfg.AddProfile<AddNewLeaveOffProfile>();
                cfg.AddProfile<EditResgisterLeaveOffProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            _leaveOffServices = new LeaveOffServices(appContext, mapper);
            _appDbContext = appContext;
            _mapper = mapper;
        }

        public void TearDown()
        {
            _appDbContext.Dispose();
        }
        #region function AccepterLeaveOffAsync
        [Fact]
        public async Task AccepterLeaveOffAsync_WithInvalidIdLeaveOff_ShouldReturnErrorMessage()
        {
            var idLeaveOff = -1;
            var dataAccept = new AccepterLeaveOffDto()
            {
                idAcceptUser = 1,
                ReasonAccept = "thích thì duyệt"
            };

            var result = await _leaveOffServices.AccepterLeaveOffAsync(idLeaveOff, dataAccept);

            Assert.False(result._success);
            Assert.Equal("idLeaveOff không tồn tại!", result._Message);
            Assert.Null(result._Data);
            TearDown();
        }
        [Fact]
        public async Task AccepterLeaveOffAsync_WithValidInputs_ShouldUpdateLeaveOffAndReturnSuccessResult()
        {

            var data = new LeaveOff()
            {
                id = 1,
                status = StatusLO.Waiting,
                reasons = "Thích thì nghỉ",
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(1),
                idCompanyBranh = 1,
                idLeaveUser = 1,
            };

            var idLeaveOff = 1;

            var dataAccept = new AccepterLeaveOffDto()
            {
                idAcceptUser = 1,
                ReasonAccept = "thích thì duyệt"
            };

            await _appDbContext.leaveOffs.AddAsync(data);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.AccepterLeaveOffAsync(idLeaveOff, dataAccept);

            Assert.True(result._success);
            Assert.Equal("Chấp nhận nghỉ phép thành công", result._Message);
            Assert.NotNull(result._Data);
            Assert.Equal(StatusLO.Done, result._Data.status);
            TearDown();
        }
        #endregion

    }
}
