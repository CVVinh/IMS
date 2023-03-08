using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.NotificationDtos;
using BE.Data.Models;
using BE.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        IHubContext<NotificationHub> _notificationHubs;
        private readonly IMapper _mapper;
        IHubContext _hubContext;

        public NotificationController(AppDbContext context, IHubContext<NotificationHub> notificationHubs, IMapper mapper)
        {
            _context = context;
            _notificationHubs = notificationHubs;
            _mapper = mapper;
        }
        [HttpGet("GetAllListNotification")]
        public async Task<IActionResult> GetNotificationByPMId()
        {
            var data = await _context.Notifications.OrderByDescending(s => s.dateCreated).Where(x => x.isRequireDelete == false && x.usercode == null).ToListAsync();

            return Ok(data);
        }
        [HttpPut("WatchNotification/{id}")]
        [Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "module: notifications update: 1")]
        public async Task<IActionResult> isWatchNotification(int id)
        {
            var check = await _context.Notifications.FindAsync(id);
            if (check != null)
            {
                check.isWatched = true;
                _context.Update(check);
                await _context.SaveChangesAsync();
                return Ok(check);
            }
            return BadRequest();
        }
        [HttpPost("RequireDeleteApplication")]
        [Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "module: notifications add: 1")]
        public async Task<IActionResult> CreateRequireDeleteApplication(CreateRequireDeleteApplicationDTO createRequireDeleteApplicationDTO)
        {
            if (ModelState.IsValid)
            {
                var findUser = await _context.Users.SingleOrDefaultAsync(x => x.userCode == createRequireDeleteApplicationDTO.usercode);

                if (findUser == null)
                {
                    return NotFound(); // user with the given usercode does not exist
                }

                var map = _mapper.Map<Notification>(createRequireDeleteApplicationDTO);
                map.isRequireDelete = true;
                map.dateCreated = DateTime.Now;
                _context.Notifications.Add(map);
                await _context.SaveChangesAsync();
                //await _notificationHubs.Clients.Groups(findUser.userCode).SendAsync("DeleteApplication", map.message);
                await _notificationHubs.Clients.All.SendAsync("DeleteApplication", map.message);
                return Ok(map);
            }
            return BadRequest();
        }
        [HttpGet("GetRequireNotification")]
        public async Task<IActionResult> GetRequireDeleteNotification(string userCode)
        {
            var getNoti = await _context.Notifications.Where(x => x.isRequireDelete == true && userCode == userCode).OrderBy(x => x.dateCreated).LastOrDefaultAsync();
            var map = _mapper.Map<CreateRequireDeleteApplicationDTO>(getNoti);
            return Ok(map);
        }

    }
}
