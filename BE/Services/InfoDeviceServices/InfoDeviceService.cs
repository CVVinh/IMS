using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.InfoDeviceDtos;
using BE.Data.Extentions;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;


namespace BE.Services.InfoDeviceServices
{
    public interface IInfoDeviceService
    {
        Task<BaseResponse<List<AppInDevice>>> GetAllAsync();
        Task<BaseResponse<AppInDevice>> CreateInfoDevice(CreateInfoDeviceDto createInfoDevice);
        Task<BaseResponse<AppInDevice>> EditInfoDeviceWithDeviceId(int id, CreateInfoDeviceDto createInfoDeviceDto);
        //Task<BaseResponse<List<InfoDevices>>> GetAllAsync();
        Task<BaseResponse<List<AppInDevice>>> FindWithName(string name);
        Task<BaseResponse<List<AppInDevice>>> FindWithOperatingSystem(int winBuildNumber);
        Task<BaseResponse<AppInDevice>> FindWithUserCode(string userCode);
        Task<BaseResponse<AppInDevice>> FindWithUserId(int UserId);
        Task<BaseResponse<AppInDevice>> EditInfoDeviceWithUserId(int id, CreateInfoDeviceDto createInfoDeviceDto);
    }

    public class AppInDevice
    {
        public InfoDevices DeviceInfo { get; set; }
        public List<DeviceInstalledApps> Applications { get; set; }
    }

    public class InfoDeviceService : IInfoDeviceService
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public InfoDeviceService(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }


        public async Task<BaseResponse<List<AppInDevice>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<AppInDevice>();
            try
            {
                var infoDevice = await _appContext.InfoDevices.ToListAsync();
                var installApp = await _appContext.DeviceInstalledApps.ToListAsync();

                var info = infoDevice
                    .GroupJoin(
                    installApp,
                    info => info.DeviceId,
                    app => app.DeviceId,
                    (info, app) => new
                    {
                        app,
                        info
                    });

                foreach (var item in info)
                {
                    var map = new AppInDevice()
                    {
                        DeviceInfo = item.info,
                        Applications = item.app.ToList(),
                    };
                    data.Add(map);
                }

                success = true;
                message = "Get all data successfully";
                return new BaseResponse<List<AppInDevice>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                data = new List<AppInDevice>(); ;
                return (new BaseResponse<List<AppInDevice>>(success, message, data));
            }
        }
        public async Task<BaseResponse<AppInDevice>> CreateInfoDevice(CreateInfoDeviceDto createInfoDevice)
        {
            try
            {
                //var now = DateTime.Now;
                var now = DateTime.Now;


                var infoDevice = _mapper.Map<InfoDevices>(createInfoDevice);
                await _appContext.InfoDevices.AddAsync(infoDevice.addOrEditInfoDeviceExtentions());
                await _appContext.SaveChangesAsync();

                var installedApps = createInfoDevice.Application
                    .Select(app => new DeviceInstalledApps
                    {
                        DeviceId = infoDevice.DeviceId,
                        ApplicationName = app.ApplicationName,
                        ApplicationLocation = app.ApplicationLocation,
                        UpdateAt = now
                    }).ToList();

                await _appContext.DeviceInstalledApps.AddRangeAsync(installedApps);
                await _appContext.SaveChangesAsync();

                var success = true;
                var message = "Add new Information Device successfully";
                var data = new AppInDevice { DeviceInfo = infoDevice, Applications = installedApps };

                return new BaseResponse<AppInDevice>(success, message, data);
            }
            catch (Exception ex)
            {
                var success = false;
                var message = $"Adding new Info Device failed! {ex.Message}";
                var data = new AppInDevice();

                return new BaseResponse<AppInDevice>(success, message, data);
            }
        }
        public async Task<BaseResponse<AppInDevice>> EditInfoDeviceWithDeviceId(int id, CreateInfoDeviceDto createInfoDeviceDto)
        {
            var success = false;
            var message = "";
            var data = new AppInDevice();
            try
            {
                var checkInfoDevice = await _appContext.InfoDevices.FindAsync(id);
                if (checkInfoDevice == null)
                {
                    message = "InfoDevice doesn't exist!";
                    return new BaseResponse<AppInDevice>(success, message, data);
                }

                _mapper.Map(createInfoDeviceDto, checkInfoDevice);
                checkInfoDevice.UpdateAt = DateTime.Now;

                var existingApplications = await _appContext.DeviceInstalledApps.Where(a => a.DeviceId == id).ToListAsync();
                _appContext.DeviceInstalledApps.RemoveRange(existingApplications);
                existingApplications.Clear();
                if (createInfoDeviceDto.Application != null && createInfoDeviceDto.Application.Any())
                {
                    var appsToBeAdded = createInfoDeviceDto.Application.Select(app => new DeviceInstalledApps
                    {
                        DeviceId = id,
                        ApplicationName = app.ApplicationName,
                        ApplicationLocation = app.ApplicationLocation,
                        UpdateAt = DateTime.Now
                    });
                    await _appContext.DeviceInstalledApps.AddRangeAsync(appsToBeAdded);
                }

                await _appContext.SaveChangesAsync();
                success = true;
                message = "Information Device updated successfully";
                data = new AppInDevice
                {
                    DeviceInfo = checkInfoDevice,
                    Applications = await _appContext.DeviceInstalledApps.Where(a => a.DeviceId == id).ToListAsync()
                };
            }
            catch (Exception ex)
            {
                message = $"Editing Information Device failed! {ex.Message}";
            }
            return new BaseResponse<AppInDevice>(success, message, data);
        }
        public async Task<BaseResponse<AppInDevice>> EditInfoDeviceWithUserId(int id, CreateInfoDeviceDto createInfoDeviceDto)
        {
            var success = false;
            var message = "";
            var data = new AppInDevice();
            try
            {
                var checkInfoDevice = await _appContext.InfoDevices.Where(x => x.UserId == id).FirstOrDefaultAsync();
                if (checkInfoDevice == null)
                {
                    message = "InfoDevice doesn't exist!";
                    return new BaseResponse<AppInDevice>(success, message, data);
                }
                _mapper.Map(createInfoDeviceDto, checkInfoDevice);
                checkInfoDevice.UpdateAt = DateTime.Now;
                var existingApplications = await _appContext.DeviceInstalledApps.Where(a => a.DeviceId == checkInfoDevice.DeviceId).ToListAsync();
                _appContext.DeviceInstalledApps.RemoveRange(existingApplications);
                existingApplications.Clear();
                if (createInfoDeviceDto.Application != null && createInfoDeviceDto.Application.Any())
                {
                    var appsToBeAdded = createInfoDeviceDto.Application.Select(app => new DeviceInstalledApps
                    {
                        DeviceId = checkInfoDevice.DeviceId,
                        ApplicationName = app.ApplicationName,
                        ApplicationLocation = app.ApplicationLocation,
                        UpdateAt = DateTime.Now
                    });
                    await _appContext.DeviceInstalledApps.AddRangeAsync(appsToBeAdded);
                }

                await _appContext.SaveChangesAsync();
                success = true;
                message = "Information Device updated successfully";
                data = new AppInDevice
                {
                    DeviceInfo = checkInfoDevice,
                    Applications = await _appContext.DeviceInstalledApps.Where(a => a.DeviceId == id).ToListAsync()
                };
            }
            catch (Exception ex)
            {

                message = $"Editing Information Device failed! {ex.Message}";
            }
            return new BaseResponse<AppInDevice>(success, message, data);
        }
        public async Task<BaseResponse<List<AppInDevice>>> FindWithName(string name)
        {
            try
            {
                var users = await _appContext.Users
                    .Where(u => u.firstName.ToLower().Trim().Contains(name.Trim().ToLower()) || u.lastName.ToLower().Trim().Contains(name.Trim().ToLower()))
                    .ToListAsync();

                if (users.Count == 0)
                {
                    return new BaseResponse<List<AppInDevice>>(true, $"No username with name '{name}'", new List<AppInDevice>());
                }

                var userIds = users.Select(u => u.id).ToList();

                var devices = await _appContext.InfoDevices
                    .Where(d => userIds.Contains(d.UserId.Value))
                    .ToListAsync();

                if (devices.Count == 0)
                {
                    return new BaseResponse<List<AppInDevice>>(true, $"No device owned by username '{name}'", new List<AppInDevice>());
                }

                var installApp = await _appContext.DeviceInstalledApps.ToListAsync();

                var data = devices.GroupJoin(installApp,
                    info => info.DeviceId,
                    app => app.DeviceId,
                    (info, app) => new AppInDevice
                    {
                        DeviceInfo = info,
                        Applications = app.ToList()
                    }).ToList();

                return new BaseResponse<List<AppInDevice>>(true, "Get all data successfully", data);
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AppInDevice>>(false, $"Get Info Device failed! {ex.Message}", new List<AppInDevice>());
            }
        }
        public async Task<BaseResponse<List<AppInDevice>>> FindWithOperatingSystem(int winBuildNumber)
        {
            try
            {
                if (winBuildNumber >= 22000)
                {
                    var win11 = _appContext.InfoDevices
                    .AsEnumerable()
                    .Where(d => winBuildNumber <= Convert.ToInt32(d.OperatingSystem.Split('.')[2])).ToList();
                    var installApp = await _appContext.DeviceInstalledApps.ToListAsync();
                    var data = win11.GroupJoin(installApp,
                    info => info.DeviceId,
                    app => app.DeviceId,
                    (info, app) => new AppInDevice
                    {
                        DeviceInfo = info,
                        Applications = app.ToList()
                    }).ToList();
                    return new BaseResponse<List<AppInDevice>>(true, "Get all device with Windows 11 successfully", data);
                }
                else
                {
                    var win10andElder = _appContext.InfoDevices
                   .AsEnumerable()
                   .Where(d => Convert.ToInt32(d.OperatingSystem.Split('.')[2]) >= winBuildNumber && Convert.ToInt32(d.OperatingSystem.Split('.')[2]) < 22000).ToList();
                    var installApp = await _appContext.DeviceInstalledApps.ToListAsync();
                    var data = win10andElder.GroupJoin(installApp,
                    info => info.DeviceId,
                    app => app.DeviceId,
                    (info, app) => new AppInDevice
                    {
                        DeviceInfo = info,
                        Applications = app.ToList()
                    }).ToList();
                    return new BaseResponse<List<AppInDevice>>(true, "Get all device with Windows 10 and older successfully", data);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AppInDevice>>(false, $"Get Info Device failed! {ex.Message}", new List<AppInDevice>());
            }
        }
        public async Task<BaseResponse<AppInDevice>> FindWithUserCode(string userCode)
        {
            try
            {
                var user = await _appContext.Users.Where(x => x.userCode == userCode).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new BaseResponse<AppInDevice>(true, $"No username with user code '{userCode}'", null);
                }
                int userIds = user.id;
                var devices = await _appContext.InfoDevices.Where(x => x.UserId == userIds).FirstOrDefaultAsync();
                if (devices == null)
                {
                    return new BaseResponse<AppInDevice>(true, $"User {user.FullName} no have device", null);
                }
                var installApp = await _appContext.DeviceInstalledApps.ToListAsync();

                var data = installApp.Where(x => devices.DeviceId == x.DeviceId).ToList();
                return new BaseResponse<AppInDevice>(false, $"Get Info Device Success!", new AppInDevice() { Applications = installApp, DeviceInfo = devices });
            }
            catch (Exception ex)
            {
                return new BaseResponse<AppInDevice>(false, $"Get Info Device failed! {ex.Message}", null);
            }
        }
        public async Task<BaseResponse<AppInDevice>> FindWithUserId(int UserId)
        {
            try
            {
                var device = await _appContext.InfoDevices.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
                if (device == null)
                {
                    return new BaseResponse<AppInDevice>(true, $"No device with userId '{UserId}'", null);
                }
                var installApp = await _appContext.DeviceInstalledApps.ToListAsync();

                var data = installApp.Where(x => device.DeviceId == x.DeviceId).ToList();
                return new BaseResponse<AppInDevice>(true, $"Get Info Device Success!", new AppInDevice() { Applications = installApp, DeviceInfo = device });
            }
            catch (Exception ex)
            {
                return new BaseResponse<AppInDevice>(false, $"Get Info Device failed! {ex.Message}", null);
            }
        }
    }

}

