using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Extentions;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Bibliography;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Linq;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Services.LeaveOffServices
{
    public interface ILeaveOffServices
    {
        Task<BaseResponse<List<LeaveOff>>> GetAllAsync();
        Task<BaseResponse<LeaveOff>> AddNewLeaveOffAsync(AddNewLeaveOffDto addNewLeaveOffDto);
        Task<BaseResponse<LeaveOff>> EditRegisterLeaveOffAsync(int id, EditRegisterLeaveOffDtos editRegisterLeaveOffDtos);
        Task<BaseResponse<LeaveOff>> AccepterLeaveOffAsync(int idLeaveOff, int idAccepter);
        Task<BaseResponse<LeaveOff>> NotAccepterLeaveOffAsync(int idLeaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto);
        Task<BaseResponse<LeaveOff>> GetLeaveOffByIdAsync(int idLeaveOff);
        Task<BaseResponse<LeaveOff>> DeleteLeaveOffByIdAsync(int idLeaveOff);
        Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByStatus(StatusLO statusLO);
        Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffOfIdUserByStatus(int idLeaveOffUser, StatusLO statusLO);
        Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByYear(FindByNameStatusDateDtos infoDtos);
        Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByNameStatusDate(FindByNameStatusDateDtos infoDtos);
    }

    public class LeaveOffServices : ILeaveOffServices
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public LeaveOffServices(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<LeaveOff>> AccepterLeaveOffAsync(int idLeaveOff, int idAccepter)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                var leaveOffMap = leaveOff.AccepterLeaveOffExtention(idAccepter, StatusLO.Done);
                _appContext.leaveOffs.Update(leaveOffMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Chấp nhận nghỉ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Chấp nhận nghỉ không thành công! {ex.InnerException}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> AddNewLeaveOffAsync(AddNewLeaveOffDto addNewLeaveOffDto)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = _mapper.Map<LeaveOff>(addNewLeaveOffDto);
                await _appContext.leaveOffs.AddAsync(leaveOff.addNewLeaveOffExtention());
                var getUser = await _appContext.Users.Where(x => x.id.Equals(leaveOff.idLeaveUser)).FirstOrDefaultAsync();

                if (getUser != null)
                {
                    Notification noti = new Notification()
                    {
                        requestUser = leaveOff.idLeaveUser,
                        message = leaveOff.reasons,
                        title = $"Nghỉ phép từ nhân viên '{getUser.FullName}'",
                        isWatched = false,
                        userCreated = leaveOff.idLeaveUser,
                        link = "/leaveoff/acceptregisterlists",
                        dateCreated = DateTime.Now,
                    };
                    _appContext.Notifications.Add(noti);
                }
                else
                {
                    Notification noti = new Notification()
                    {
                        requestUser = leaveOff.idLeaveUser,
                        message = leaveOff.reasons,
                        title = $"Nghỉ phép từ nhân viên 'Ẩn danh'",
                        isWatched = false,
                        userCreated = leaveOff.idLeaveUser,
                        link = "/leaveoff/acceptregisterlists",
                        dateCreated = DateTime.Now,
                    };
                    _appContext.Notifications.Add(noti);
                }

                await _appContext.SaveChangesAsync();

                success = true;
                message = "Thêm ngày nghỉ mới thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Thêm ngày nghỉ mới không thành công! {ex.Message}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> DeleteLeaveOffByIdAsync(int idLeaveOff)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success= false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                _appContext.leaveOffs.Remove(leaveOff);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Xóa nghỉ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Xóa nghỉ phép không thành công! {ex.InnerException}";
                data = null;
                success = false;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> EditRegisterLeaveOffAsync(int id, EditRegisterLeaveOffDtos editRegisterLeaveOffDtos)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == id && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success= false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }
                var leaveOffMap = editRegisterLeaveOffDtos.editRegisterLeaveOffExtention(leaveOff);
                _appContext.leaveOffs.Update(leaveOffMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Chỉnh sửa nghỉ phép thành công";
                data = leaveOffMap;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Không thể chỉnh sửa nghỉ phép! {ex.Message}";
                data = null;
                success= false;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<LeaveOff>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();
            try
            {
                var listLeaveOff = await _appContext.leaveOffs.OrderByDescending(t => t.createTime)
                                                              .ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(listLeaveOff);
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }
        public async Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByStatus(StatusLO statusLO)
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();
            try
            {
                var listLeaveOff = await _appContext.leaveOffs.Where(lo => lo.status == statusLO)
                                                              .OrderByDescending(t => t.createTime)
                                                              .ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(listLeaveOff);
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffOfIdUserByStatus(int idLeaveOffUser, StatusLO statusLO)
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();
            try
            {
                if ((int)statusLO == 0)
                {
                    var listAllLeaveOff = await _appContext.leaveOffs.Where(lo => lo.idLeaveUser == idLeaveOffUser)
                                                              .OrderByDescending(t => t.createTime)
                                                              .ToListAsync();
                    success = true;
                    message = "Lấy tất cả dữ liệu thành công";
                    data.AddRange(listAllLeaveOff);
                    return (new BaseResponse<List<LeaveOff>>(success, message, data));
                }

                var listLeaveOff = await _appContext.leaveOffs.Where(lo => lo.status == statusLO && lo.idLeaveUser == idLeaveOffUser)
                                                              .OrderByDescending(t => t.createTime)
                                                              .ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(listLeaveOff);
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }

        public async Task<BaseResponse<LeaveOff>> GetLeaveOffByIdAsync(int idLeaveOff)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success = false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                success = true;
                message = "Lấy thẻ nghĩ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Lấy thẻ nghĩ phép không thành công! {ex.Message}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<LeaveOff>> NotAccepterLeaveOffAsync(int idLeaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto)
        {
            var success = false;
            var message = "";
            var data = new LeaveOff();
            try
            {
                var leaveOff = await _appContext.leaveOffs.Where(lo => lo.id == idLeaveOff && lo.status == StatusLO.Waiting)
                                                          .FirstOrDefaultAsync();
                if (leaveOff is null)
                {
                    message = "idLeaveOff không tồn tại!";
                    data = null;
                    success= false;
                    return new BaseResponse<LeaveOff>(success, message, data);
                }

                _appContext.leaveOffs.Update(leaveOff.NotAcceptLeaveOffExtention(notAcceptLeaveOffDto, StatusLO.Cancel));
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Không chấp nhận nghỉ phép thành công";
                data = leaveOff;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Không chấp nhận nghỉ phép không thành công! {ex.InnerException}";
                data = null;
                return new BaseResponse<LeaveOff>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByNameStatusDate(FindByNameStatusDateDtos infoDtos)
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();

            if (infoDtos.fullName == null && infoDtos.status == null && infoDtos.date == null)
            {
                success = false;
                message = "Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.";
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }

            try
            {
                var query = _appContext.leaveOffs.AsQueryable();

                if (String.IsNullOrEmpty(infoDtos.fullName) == false)
                {
                    var checkName = _appContext.Users
                        .Where(q => (q.firstName + " " + q.lastName).ToLower().Trim().Contains(infoDtos.fullName.Trim().ToLower()))
                        .ToList();
                    var idList = checkName.Select(item => item.id).ToList();
                    query = query.Where(x => idList.Contains(x.idLeaveUser));
                }

                if (infoDtos.status.Count > 0)
                {
                    var statusList = infoDtos.status.ToList();
                    query = query.Where(x => statusList.Contains((int)x.status));
                }

                if (infoDtos.date != null)
                {
                    query = query.Where(x => (x.startTime.Month <= infoDtos.date.Value.Month && x.startTime.Year == infoDtos.date.Value.Year)
                        && (x.endTime.Month >= infoDtos.date.Value.Month && x.endTime.Year == infoDtos.date.Value.Year));
                }

                data = await query.ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Nghỉ phép không thành công! Đã xảy ra lỗi: " + ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<LeaveOff>>> GetAllLeaveOffByYear(FindByNameStatusDateDtos infoDtos)
        {
            var success = false;
            var message = "";
            var data = new List<LeaveOff>();

            if (infoDtos.fullName == null && infoDtos.status == null && infoDtos.date == null)
            {
                success = false;
                message = "Nghỉ phép không thành công! Thông số đầu vào không được cung cấp.";
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }

            try
            {
                var query = _appContext.leaveOffs.AsQueryable();

                if (String.IsNullOrEmpty(infoDtos.fullName) == false)
                {
                    var checkName = _appContext.Users
                        .Where(q => (q.firstName + " " + q.lastName).ToLower().Trim().Contains(infoDtos.fullName.Trim().ToLower()))
                        .ToList();
                    var idList = checkName.Select(item => item.id).ToList();
                    query = query.Where(x => idList.Contains(x.idLeaveUser));
                }

                if (infoDtos.status.Count > 0)
                {
                    var statusList = infoDtos.status.ToList();
                    query = query.Where(x => statusList.Contains((int)x.status));
                }

                if (infoDtos.date != null)
                {
                    query = query.Where(t => t.startTime.Year <= infoDtos.date.Value.Year && t.endTime.Year >= infoDtos.date.Value.Year);
                }

                data = await query.ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Không thể nghỉ phép! Đã xảy ra lỗi: " + ex.Message;
                return (new BaseResponse<List<LeaveOff>>(success, message, data));
            }
        }
    }
}
