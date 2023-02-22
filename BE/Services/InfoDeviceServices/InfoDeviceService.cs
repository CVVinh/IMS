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
        Task<BaseResponse<InfoDevices>> CreateInfoDevice(CreateInfoDeviceDto createInfoDevice);
        Task<BaseResponse<InfoDevices>> EditInfoDevice(int id, CreateInfoDeviceDto createInfoDeviceDto);
        Task<BaseResponse<List<InfoDevices>>> GetAllAsync();
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


        public async Task<BaseResponse<List<InfoDevices>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<InfoDevices>();
            try
            {
                var infoDevice = await _appContext.InfoDevices.ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(infoDevice);
                return (new BaseResponse<List<InfoDevices>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<InfoDevices>>(success, message, data));
            }
        }
        public async Task<BaseResponse<InfoDevices>> CreateInfoDevice(CreateInfoDeviceDto createInfoDevice)
        {
            var success = false;
            var message = "";
            var data = new InfoDevices();
            try
            {
                var infodevice = _mapper.Map<InfoDevices>(createInfoDevice);
                await _appContext.InfoDevices.AddAsync(infodevice.addOrEditInfoDeviceExtentions());
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Add new Information Device successfully";
                data = infodevice;


                return new BaseResponse<InfoDevices>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Adding new Paid failed! {ex.Message}";
                data = null;
                return new BaseResponse<InfoDevices>(success, message, data);
            }
        }
        public async Task<BaseResponse<InfoDevices>> EditInfoDevice(int id, CreateInfoDeviceDto createInfoDeviceDto)
        {
            var success = false;
            var message = "";
            var data = new InfoDevices();
            try
            {
                var infoDevice = await _appContext.InfoDevices.Where(iD => iD.DeviceId == id).FirstOrDefaultAsync();
                if (infoDevice is null)
                {
                    message = "infoDevice doesn't exist !";
                    data = null;
                    return new BaseResponse<InfoDevices>(success, message, data);
                }
                var map = _mapper.Map<CreateInfoDeviceDto, InfoDevices>(createInfoDeviceDto, infoDevice);
                _appContext.InfoDevices.Update(map.addOrEditInfoDeviceExtentions());
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Editing Information Device successfully";
                data = map;
                return new BaseResponse<InfoDevices>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Editing Information Device failed! {ex.Message}";
                data = null;
                return new BaseResponse<InfoDevices>(success, message, data);
            }
        }
    }
}
