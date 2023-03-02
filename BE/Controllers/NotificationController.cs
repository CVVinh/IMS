using BE.Data.Contexts;
using BE.Hubs;
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
        [HttpGet]
        public async Task<IActionResult> GetNotificationByPMId()
        {
            var data = await _context.Notifications.ToListAsync();

            return Ok(data);
        }

    }
}
