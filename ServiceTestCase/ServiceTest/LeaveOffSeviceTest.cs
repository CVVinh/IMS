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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BE.Data.Enum.LeaveOff.Status;
using Users = BE.Data.Models.Users;

namespace ServiceTestCase.ServiceTest
{
    [TestFixture]
    public class LeaveOffSeviceTest
    {
        private ILeaveOffServices _leaveOffServices;
        private AppDbContext _appDbContext;
        private IMapper _mapper;

        #region Setup môi trường
        [SetUp]
        public void Setup()
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
        [TearDown]
        public void TearDown()
        {
            _appDbContext.Dispose();
        }
        #endregion

        #region function AccepterLeaveOffAsync
        [Test]
        public async Task AccepterLeaveOffAsync_WithInvalidIdLeaveOff_ShouldReturnErrorMessage()
        {
            var idLeaveOff = -1;
            var dataAccept = new AccepterLeaveOffDto()
            {
                idAcceptUser = 1,
                ReasonAccept = "thích thì duyệt"
            };

            var result = await _leaveOffServices.AccepterLeaveOffAsync(idLeaveOff, dataAccept);

            Assert.IsFalse(result._success);
            Assert.AreEqual("idLeaveOff không tồn tại!", result._Message);
            Assert.IsNull(result._Data);
        }
        [Test]
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

            Assert.IsTrue(result._success);
            Assert.AreEqual("Chấp nhận nghỉ phép thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(StatusLO.Done, result._Data.status);
        }
        #endregion

        #region function AddNewLeaveOffAsync
        [Test]
        public async Task AddNewLeaveOffAsync_WhenCalledWithValidData_ShouldAddNewLeaveOffAndNotification()
        {
            var addNewLeaveOffDto = new AddNewLeaveOffDto
            {
                idLeaveUser = 1,
                reasons = "Test leave off",
                startTime = DateTime.Now.AddDays(1),
                endTime = DateTime.Now.AddDays(3),
                idCompanyBranh = 1,
            };

            var result = await _leaveOffServices.AddNewLeaveOffAsync(addNewLeaveOffDto);

            Assert.True(result._success);
            Assert.AreEqual("Thêm ngày nghỉ mới thành công", result._Message);
            Assert.IsNotNull(result._Data);

            // Kiểm tra xem nghỉ phép mới đã được thêm vào cơ sở dữ liệu chưa
            var leaveOff = await _appDbContext.leaveOffs.FindAsync(result._Data.id);
            Assert.IsNotNull(leaveOff);
            Assert.AreEqual(addNewLeaveOffDto.idLeaveUser, leaveOff.idLeaveUser);
            Assert.AreEqual(addNewLeaveOffDto.reasons, leaveOff.reasons);
            Assert.AreEqual(addNewLeaveOffDto.startTime, leaveOff.startTime);
            Assert.AreEqual(addNewLeaveOffDto.endTime, leaveOff.endTime);

            // Kiểm tra xem có thông báo mới được thêm vào cơ sở dữ liệu không
            var notification = await _appDbContext.Notifications
                .Where(n => n.requestUser == addNewLeaveOffDto.idLeaveUser)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(notification);
            Assert.AreEqual($"Nghỉ phép từ nhân viên 'Ẩn danh'", notification.title);
            Assert.AreEqual(addNewLeaveOffDto.reasons, notification.message);
            Assert.AreEqual(false, notification.isWatched);
            Assert.AreEqual(addNewLeaveOffDto.idLeaveUser, notification.userCreated);
            Assert.AreEqual("/leaveoff/acceptregisterlists", notification.link);
            Assert.IsNotNull(notification.dateCreated);
        }
        [Test]
        public async Task AddNewLeaveOffAsync_WhenCalledWithInvalidData_ShouldReturnFailure()
        {
            var addNewLeaveOffDto = new AddNewLeaveOffDto
            {
                //idLeaveUser = 1,
                //reasons = "Test leave off",
                startTime = DateTime.Now.AddDays(3),
                endTime = DateTime.Now.AddDays(1),
                idCompanyBranh = 1,
            };

            var result = await _leaveOffServices.AddNewLeaveOffAsync(addNewLeaveOffDto);

            Assert.False(result._success);
            Assert.IsNull(result._Data);
        }
        #endregion

        #region function DeleteLeaveOffByIdAsync
        [Test]
        public async Task DeleteLeaveOffByIdAsync_WhenCalledWithValidId_ShouldDeleteLeaveOffAndReturnErrorResultByIdLeaveOff()
        {
            var idLeaveOff = 1;
            var leaveOff = new LeaveOff()
            {
                id = 2,
                status = StatusLO.Waiting,
                reasons = "Thích thì nghỉ",
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(1),
                idCompanyBranh = 1,
                idLeaveUser = 1,
            };
            _appDbContext.leaveOffs.Add(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.DeleteLeaveOffByIdAsync(idLeaveOff);

            Assert.IsFalse(result._success);
            Assert.AreEqual("idLeaveOff không tồn tại!", result._Message);
            Assert.IsNull(result._Data);
        }
        [Test]
        public async Task DeleteLeaveOffByIdAsync_WhenCalledWithValidId_ShouldDeleteLeaveOffAndReturnSuccessResult()
        {
            var idLeaveOff = 1;
            var leaveOff = new LeaveOff()
            {
                id = 1,
                status = StatusLO.Waiting,
                reasons = "Thích thì nghỉ",
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(1),
                idCompanyBranh = 1,
                idLeaveUser = 1,
            };
            _appDbContext.leaveOffs.Add(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.DeleteLeaveOffByIdAsync(idLeaveOff);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Xóa nghỉ phép thành công", result._Message);
            Assert.AreEqual(leaveOff, result._Data);

            var deletedLeaveOff = await _appDbContext.leaveOffs.FindAsync(idLeaveOff);
            Assert.IsNull(deletedLeaveOff);
        }
        #endregion

        #region function EditRegisterLeaveOffAsync
        [Test]
        public async Task EditRegisterLeaveOffAsync_WhenCalledWithValidData_ShouldEditLeaveOffSuccessResult()
        {
            var leaveOff = new LeaveOff
            {
                id = 1,
                idLeaveUser = 1,
                reasons = "Test leave off",
                startTime = DateTime.Now.AddDays(1),
                endTime = DateTime.Now.AddDays(3),
                idCompanyBranh = 1,
                status = StatusLO.Waiting
            };
            await _appDbContext.leaveOffs.AddAsync(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var editRegisterLeaveOffDtos = new EditRegisterLeaveOffDtos
            {
                reasons = "Updated leave off",
                startTime = DateTime.Now.AddDays(2),
                endTime = DateTime.Now.AddDays(4)
            };

            var result = await _leaveOffServices.EditRegisterLeaveOffAsync(leaveOff.id, editRegisterLeaveOffDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Chỉnh sửa nghỉ phép thành công", result._Message);
            Assert.IsNotNull(result._Data);

            // Kiểm tra xem ngày nghỉ đã được cập nhật trong cơ sở dữ liệu chưa
            var updatedLeaveOff = await _appDbContext.leaveOffs.FindAsync(leaveOff.id);
            Assert.IsNotNull(updatedLeaveOff);
            Assert.AreEqual(editRegisterLeaveOffDtos.reasons, updatedLeaveOff.reasons);
            Assert.AreEqual(editRegisterLeaveOffDtos.startTime, updatedLeaveOff.startTime);
            Assert.AreEqual(editRegisterLeaveOffDtos.endTime, updatedLeaveOff.endTime);
        }
        [Test]
        public async Task EditRegisterLeaveOffAsync_WhenCalledWithValidData_ShouldEditLeaveOffNotFoundResult()
        {
            var idleaveOff = 1;
            var leaveOff = new LeaveOff
            {
                id = 2,
                idLeaveUser = 1,
                reasons = "Test leave off",
                startTime = DateTime.Now.AddDays(1),
                endTime = DateTime.Now.AddDays(3),
                idCompanyBranh = 1,
                status = StatusLO.Waiting
            };
            await _appDbContext.leaveOffs.AddAsync(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var editRegisterLeaveOffDtos = new EditRegisterLeaveOffDtos
            {
                reasons = "Updated leave off",
                startTime = DateTime.Now.AddDays(2),
                endTime = DateTime.Now.AddDays(4)
            };

            var result = await _leaveOffServices.EditRegisterLeaveOffAsync(idleaveOff, editRegisterLeaveOffDtos);

            Assert.IsFalse(result._success);
            Assert.AreEqual("idLeaveOff không tồn tại!", result._Message);
            Assert.IsNull(result._Data);
        }
        [Test]
        public async Task EditRegisterLeaveOffAsync_WhenCalledWithValidData_ShouldEditLeaveOffErrorResult()
        {
            var leaveOff = new LeaveOff
            {
                id = 2,
                idLeaveUser = 1,
                reasons = "Test leave off",
                startTime = DateTime.Now.AddDays(1),
                endTime = DateTime.Now.AddDays(3),
                idCompanyBranh = 1,
                status = StatusLO.Waiting
            };
            await _appDbContext.leaveOffs.AddAsync(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var editRegisterLeaveOffDtos = new EditRegisterLeaveOffDtos();

            var result = await _leaveOffServices.EditRegisterLeaveOffAsync(1, editRegisterLeaveOffDtos);

            Assert.IsFalse(result._success);
            Assert.IsNull(result._Data);
        }
        #endregion

        #region function GetAllAsync
        [Test]
        public async Task GetAllAsync_ShouldGetAllLeaveOffSucessResult()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 1,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 1,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.GetAllAsync();

            Assert.True(result._success);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.AreEqual(leaveOff.Count, result._Data.Count);
            Assert.AreEqual(leaveOff[0].id, result._Data[0].id);
            Assert.AreEqual(leaveOff[1].id, result._Data[1].id);
            Assert.AreEqual(leaveOff[2].id, result._Data[2].id);
            _appDbContext.Database.EnsureDeleted();
        }
        #endregion

        #region function GetAllLeaveOffByStatus
        [Test]
        public async Task GetAllLeaveOffByStatus_ShouldGetAllLeaveOffByStatusSucessResult()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Cancel
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 1,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 1,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.GetAllLeaveOffByStatus(StatusLO.Done);

            Assert.True(result._success);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.AreEqual(1, result._Data.Count);
            _appDbContext.Database.EnsureDeleted();
        }
        [Test]
        public async Task GetAllLeaveOffByStatus_ShouldGetAllLeaveOffByStatusNotFoundResult()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Cancel
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 1,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 1,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.GetAllLeaveOffByStatus(0);

            Assert.True(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.AreEqual(0, result._Data.Count);
        }
        #endregion

        #region function GetLeaveOffByIdAsync
        [Test]
        public async Task GetLeaveOffByIdAsync_ShouldGetLeaveOffByIdAsyncSucessResult()
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

            await _appDbContext.leaveOffs.AddAsync(data);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.GetLeaveOffByIdAsync(idLeaveOff);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy thẻ nghĩ phép thành công", result._Message);
            Assert.IsNotNull(result._Data);
        }
        public async Task GetLeaveOffByIdAsync_ShouldGetLeaveOffByIdAsyncNotFoundResult()
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

            var idLeaveOff = 2;

            await _appDbContext.leaveOffs.AddAsync(data);
            await _appDbContext.SaveChangesAsync();

            var result = await _leaveOffServices.GetLeaveOffByIdAsync(idLeaveOff);

            Assert.IsFalse(result._success);
            Assert.AreEqual("idLeaveOff không tồn tại!", result._Message);
            Assert.IsNull(result._Data);
        }
        #endregion

        #region function NotAccepterLeaveOffAsync
        [Test]
        public async Task NotAccepterLeaveOffAsync_WhenCalledWithValidData_ShouldNotAccepterLeaveOffAsyncSuccessResult()
        {
            var idLeaveOff = 1;
            var leaveOff = new LeaveOff()
            {
                id = 1,
                status = StatusLO.Waiting,
                reasons = "Thích thì nghỉ thôi",
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(1),
                idCompanyBranh = 1,
                idLeaveUser = 1,
            };
            _appDbContext.leaveOffs.Add(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var notAcceptLeaveOffDto = new NotAcceptLeaveOffDto()
            {
                idAcceptUser = 1,
                ReasonNotAccept = "Không duyệt cho nghĩ đó rồi sao!!!!"
            };

            var result = await _leaveOffServices.NotAccepterLeaveOffAsync(idLeaveOff, notAcceptLeaveOffDto);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Không chấp nhận nghỉ phép thành công", result._Message);
            Assert.AreEqual(leaveOff, result._Data);
            Assert.IsNotNull(result._Data);
        }
        [Test]
        public async Task NotAccepterLeaveOffAsync_WhenCalledWithValidData_ShouldNotAccepterLeaveOffAsyncNotFoundResult()
        {
            var idLeaveOff = 2;
            var leaveOff = new LeaveOff()
            {
                id = 1,
                status = StatusLO.Waiting,
                reasons = "Thích thì nghỉ thôi",
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddDays(1),
                idCompanyBranh = 1,
                idLeaveUser = 1,
            };
            _appDbContext.leaveOffs.Add(leaveOff);
            await _appDbContext.SaveChangesAsync();

            var notAcceptLeaveOffDto = new NotAcceptLeaveOffDto()
            {
                idAcceptUser = 1,
                ReasonNotAccept = "Không duyệt cho nghĩ đó rồi sao!!!!"
            };

            var result = await _leaveOffServices.NotAccepterLeaveOffAsync(idLeaveOff, notAcceptLeaveOffDto);

            Assert.IsFalse(result._success);
            Assert.AreEqual("idLeaveOff không tồn tại!", result._Message);
            Assert.AreNotEqual(leaveOff, result._Data);
            Assert.IsNull(result._Data);
        }
        #endregion

        #region function GetAllLeaveOffByNameStatusDate
        [Test]
        public async Task GetAllLeaveOffByNameStatusDate_WhenInputIsNull_ReturnsErrorResponse()
        {
            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = null,
                status = null,
                date = null
            };

            var result = await _leaveOffServices.GetAllLeaveOffByNameStatusDate(infoDtos);

            Assert.IsFalse(result._success);
            Assert.AreEqual("Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.", result._Message);
            Assert.IsEmpty(result._Data);
        }
        [Test]
        public async Task GetAllLeaveOffByNameStatusDate_WhenFullNameIsValid_ReturnsLeaveOffs()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 2,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(2),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 2,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(4),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };

            var user = new Users()
            {
                id = 1,
                userCode = "Admin1234",
                userPassword = "password",
                dateCreated = DateTime.Now,
                firstName = "nguyen",
                lastName = "thanh",
                phoneNumber = "1234567890",
                dOB = DateTime.Now,
                gender = 2,
                address = "XxXXXX",
                university = "CTU",
                yearGraduated = 3000,
                email = "tanh12412@gmail.com",
                emailPassword = "password",
                skype = "string",
                skypePassword = "password",
                workStatus = 3,
                dateStartWork = DateTime.Now,
                maritalStatus = 3,
                reasonResignation = "",
                identitycard = "12222222222",
                IdGroup = 1
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = "nguyen",
                status = new List<int>(),
                date = null
            };

            var result = await _leaveOffServices.GetAllLeaveOffByNameStatusDate(infoDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(1, result._Data.Count);
            Assert.AreEqual(1, result._Data[0].id);
        }
        [Test]
        public async Task GetAllLeaveOffByNameStatusDate_WhenStatusIsValid_ReturnsLeaveOffs()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 2,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(2),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 2,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(4),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };

            var user = new Users()
            {
                id = 1,
                userCode = "Admin1234",
                userPassword = "password",
                dateCreated = DateTime.Now,
                firstName = "nguyen",
                lastName = "thanh",
                phoneNumber = "1234567890",
                dOB = DateTime.Now,
                gender = 2,
                address = "XxXXXX",
                university = "CTU",
                yearGraduated = 3000,
                email = "tanh12412@gmail.com",
                emailPassword = "password",
                skype = "string",
                skypePassword = "password",
                workStatus = 3,
                dateStartWork = DateTime.Now,
                maritalStatus = 3,
                reasonResignation = "",
                identitycard = "12222222222",
                IdGroup = 1
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = null,
                status = new List<int> { (int)StatusLO.Waiting },
                date = null
            };

            var result = await _leaveOffServices.GetAllLeaveOffByNameStatusDate(infoDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(2, result._Data.Count);
            Assert.AreEqual(1, result._Data[0].id);
            Assert.AreEqual(2, result._Data[1].id);
        }
        [Test]
        public async Task GetAllLeaveOffByNameStatusDate_WhenDateValid_ReturnsLeaveOffs()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddMonths(1),
                        endTime = DateTime.Now.AddMonths(2),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 2,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddMonths(-1),
                        endTime = DateTime.Now.AddMonths(1),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 2,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddMonths(-2),
                        endTime = DateTime.Now.AddMonths(-1),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };

            var user = new Users()
            {
                id = 1,
                userCode = "Admin1234",
                userPassword = "password",
                dateCreated = DateTime.Now,
                firstName = "nguyen",
                lastName = "thanh",
                phoneNumber = "1234567890",
                dOB = DateTime.Now,
                gender = 2,
                address = "XxXXXX",
                university = "CTU",
                yearGraduated = 3000,
                email = "tanh12412@gmail.com",
                emailPassword = "password",
                skype = "string",
                skypePassword = "password",
                workStatus = 3,
                dateStartWork = DateTime.Now,
                maritalStatus = 3,
                reasonResignation = "",
                identitycard = "12222222222",
                IdGroup = 1
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = null,
                status = new List<int>(),
                date = DateTime.Now.AddMonths(1)
            };

            var result = await _leaveOffServices.GetAllLeaveOffByNameStatusDate(infoDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(2, result._Data.Count);
        }
        #endregion

        #region function GetAllLeaveOffByYear
        [Test]
        public async Task GetAllLeaveOffByYear_WhenInputIsNull_ReturnsErrorResponse()
        {
            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = null,
                status = null,
                date = null
            };

            var result = await _leaveOffServices.GetAllLeaveOffByYear(infoDtos);

            Assert.IsFalse(result._success);
            Assert.AreEqual("Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.", result._Message);
            Assert.IsEmpty(result._Data);
        }
        [Test]
        public async Task GetAllLeaveOffByYear_WhenFullNameIsValid_ReturnsLeaveOffs()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 2,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(2),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 2,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(4),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };

            var user = new Users()
            {
                id = 1,
                userCode = "Admin1234",
                userPassword = "password",
                dateCreated = DateTime.Now,
                firstName = "nguyen",
                lastName = "thanh",
                phoneNumber = "1234567890",
                dOB = DateTime.Now,
                gender = 2,
                address = "XxXXXX",
                university = "CTU",
                yearGraduated = 3000,
                email = "tanh12412@gmail.com",
                emailPassword = "password",
                skype = "string",
                skypePassword = "password",
                workStatus = 3,
                dateStartWork = DateTime.Now,
                maritalStatus = 3,
                reasonResignation = "",
                identitycard = "12222222222",
                IdGroup = 1
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = "nguyen",
                status = new List<int>(),
                date = null
            };

            var result = await _leaveOffServices.GetAllLeaveOffByYear(infoDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(1, result._Data.Count);
            Assert.AreEqual(1, result._Data[0].id);
        }
        [Test]
        public async Task GetAllLeaveOffByYear_WhenStatusIsValid_ReturnsLeaveOffs()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(3),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 2,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(2),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 2,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddDays(1),
                        endTime = DateTime.Now.AddDays(4),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };

            var user = new Users()
            {
                id = 1,
                userCode = "Admin1234",
                userPassword = "password",
                dateCreated = DateTime.Now,
                firstName = "nguyen",
                lastName = "thanh",
                phoneNumber = "1234567890",
                dOB = DateTime.Now,
                gender = 2,
                address = "XxXXXX",
                university = "CTU",
                yearGraduated = 3000,
                email = "tanh12412@gmail.com",
                emailPassword = "password",
                skype = "string",
                skypePassword = "password",
                workStatus = 3,
                dateStartWork = DateTime.Now,
                maritalStatus = 3,
                reasonResignation = "",
                identitycard = "12222222222",
                IdGroup = 1
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = null,
                status = new List<int> { (int)StatusLO.Waiting },
                date = null
            };

            var result = await _leaveOffServices.GetAllLeaveOffByYear(infoDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(2, result._Data.Count);
            Assert.AreEqual(1, result._Data[0].id);
            Assert.AreEqual(2, result._Data[1].id);
        }
        [Test]
        public async Task GetAllLeaveOffByYear_WhenDateValid_ReturnsLeaveOffs()
        {
            var leaveOff = new List<LeaveOff>()
            {
                new LeaveOff
                 {
                        id = 1,
                        idLeaveUser = 1,
                        reasons = "Test leave off",
                        startTime = DateTime.Now.AddYears(-1),
                        endTime = DateTime.Now.AddYears(1),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                 },
                new LeaveOff
                    {
                        id = 2,
                        idLeaveUser = 2,
                        reasons = "Test leave off 1",
                        startTime = DateTime.Now.AddYears(-2),
                        endTime = DateTime.Now.AddYears(-1),
                        idCompanyBranh = 1,
                        status = StatusLO.Waiting
                    },
                 new LeaveOff
                     {
                        id = 3,
                        idLeaveUser = 2,
                        reasons = "Test leave off 2",
                        startTime = DateTime.Now.AddYears(-2),
                        endTime = DateTime.Now.AddYears(-1),
                        idCompanyBranh = 1,
                        status = StatusLO.Done
                    },
            };

            var user = new Users()
            {
                id = 1,
                userCode = "Admin1234",
                userPassword = "password",
                dateCreated = DateTime.Now,
                firstName = "nguyen",
                lastName = "thanh",
                phoneNumber = "1234567890",
                dOB = DateTime.Now,
                gender = 2,
                address = "XxXXXX",
                university = "CTU",
                yearGraduated = 3000,
                email = "tanh12412@gmail.com",
                emailPassword = "password",
                skype = "string",
                skypePassword = "password",
                workStatus = 3,
                dateStartWork = DateTime.Now,
                maritalStatus = 3,
                reasonResignation = "",
                identitycard = "12222222222",
                IdGroup = 1
            };
            _appDbContext.leaveOffs.AddRange(leaveOff);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var infoDtos = new FindByNameStatusDateDtos
            {
                fullName = null,
                status = new List<int>(),
                date = DateTime.Now.AddYears(1),
            };

            var result = await _leaveOffServices.GetAllLeaveOffByYear(infoDtos);

            Assert.IsTrue(result._success);
            Assert.AreEqual("Lấy tất cả dữ liệu thành công", result._Message);
            Assert.IsNotNull(result._Data);
            Assert.AreEqual(1, result._Data.Count);
        }
        #endregion
    }
}
