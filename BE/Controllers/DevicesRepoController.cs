using API_LVTN.DAL;
using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos;
using BE.Data.Dtos.DeviceDto;
using BE.Data.Models;
using BE.Helpers;
using BE.Services.PaginationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: devicesRepos")]
    public class DevicesRepoController : CustomControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IPaginationServices<Devices> _paginationServices;
        private readonly int? IDMODULE;

        public DevicesRepoController(AppDbContext appDbContext, IMapper mapper, IPaginationServices<Devices> paginationServices)
        {
            _appDbContext = appDbContext;
            _unitOfWork = new UnitOfWork(_appDbContext);
            _mapper = mapper;
            _paginationServices = paginationServices;
            IDMODULE = _appDbContext.Menus.FirstOrDefault(x => x.controller.ToLower().Equals("devices"))?.idModule;
        }

        [HttpGet("GetListDevices")]
        [Authorize]
        public async Task<IActionResult> GetListDevices()
        {
            var token = TokenHelper.GetUserId(User);
            return ConvertResponse(await _unitOfWork.DevicesRepository.GetListorGetListWithFilter(token, IDMODULE));
        }

        [HttpPost("GetListDevicesWithPaginate")] /// Chưa sửa phân trang
        public async Task<IActionResult> GetListDevicesWithPaginate([FromBody] Paginate paginate)
        {
            var token = TokenHelper.GetUserId(User);
            var devices = await _unitOfWork.DevicesRepository.GetListorGetListWithFilter(token,IDMODULE);
            if (devices.Data == null)
            {
                return NoContent();
            }
            else
            {
                var result = _paginationServices.paginationListTableAsync(devices.Data.ToList(), paginate.pageIndex, paginate.pageSizeEnum);
                return Ok(result);
            }
        }

        [HttpPost("CreateDevices")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: devicesRepos add: 1")]
        public IActionResult CreateDevices([FromBody] CreateDevicesDto device)
        {
            var token = TokenHelper.GetUserId(User);
            Devices newdevices = _mapper.Map<Devices>(device);
            newdevices.InitAdd(token.User);

            return ConvertResponse(_unitOfWork.DevicesRepository.Insert(newdevices,token,IDMODULE));

        }

        [HttpPut("UpdateDevices/{IdDevices}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: devicesRepos update: 1")]
        public IActionResult UpdateDevices([FromRoute] int IdDevices, [FromBody] CreateDevicesDto request)
        {
            Devices device = _mapper.Map<Devices>(request);

            var token = TokenHelper.GetUserId(User);
            device.InitUpdate(token.User);
            device.IdDevice = IdDevices;

            return ConvertResponse(_unitOfWork.DevicesRepository.Update(device, token, IDMODULE));

        }

        [HttpPut("DeleteDevices/{IdDevices}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: devicesRepos delete: 1")]
        public IActionResult DeleteDevices([FromRoute] int IdDevices)
        {
            var token = TokenHelper.GetUserId(User);
            var device = _unitOfWork.DevicesRepository.GetById(IdDevices, token, IDMODULE);
            if (device.Data == null)
            {
                return ConvertResponse(device);
            }
            else
            {
                device.Data.InitDelete(token.User);
                return ConvertResponse(_unitOfWork.DevicesRepository.Delete(device.Data.IdDevice, token, IDMODULE));
            }
        }
    }
}
