using BE.Data.Contexts;
using BE.Data.Models;
using BE.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        IHubContext<NotificationHub> _notificationHubs;

        public NotificationController(AppDbContext context, IHubContext<NotificationHub> notificationHubs)
        {
            _context = context;
            _notificationHubs = notificationHubs;
        }
        [HttpGet("GetAllListNotification")]
        public async Task<IActionResult> GetNotificationByPMId()
        {
            var data = await _context.Notifications.OrderByDescending(s => s.dateCreated).ToListAsync();

            return Ok(data);
        }
        [HttpPut("WatchNotification/{id}")]
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

    }
}
