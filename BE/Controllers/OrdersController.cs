using BE.Data.Contexts;
using BE.Data.Dtos.OrderDtos;
using BE.Data.Dtos.OrdersDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: orders")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAllOrder")]
        public IActionResult getAllOrder()
        {
            try
            {
                var query = _context.Orders
                    .Join(_context.Branchs,
                    orders1 => orders1.idBranch, branchs => branchs.idBranch,
                    (orders1, branchs) => new { orders1, branchs })
                    .Join(_context.Devices, orders2 => orders2.orders1.idDevice, devices => devices.IdDevice,
                    (orders2, devices) => new { orders2, devices })
                    .Join(_context.Users, orders3 => orders3.orders2.orders1.userCreated, users => users.id,
                    (orders3, users) => new { orders3, users })
                    .Select(x => new {
                        x.orders3.orders2.branchs.branchName,
                        x.orders3.devices.DeviceName,
                        x.users.lastName,
                        x.users.firstName,
                        x.orders3.orders2.orders1
                    })
                    .ToList();
                if (query.Count() != 0)
                {
                    return Ok(query);
                }
                return NotFound("No Data");
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("addOrders")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: orders add: 1")]
        public IActionResult Create(AddDtos order_Model)
        {
            try
            {
                var p = new Orders
                {
                    idBranch = order_Model.idBranch,
                    idDevice = order_Model.idDevice,
                    amount = order_Model.amount,
                    dateCreated = order_Model.dateCreated,
                    userCreated = order_Model.userCreated,
                    dateUpdated = DateTime.Now,
                    userUpdated = order_Model.userUpdated,
                    isDeleted = order_Model.isDeleted,
                    note = order_Model.note,
                    statusOrder = order_Model.statusOrder,
                    statusDevice = order_Model.statusDevice,

                };

                _context.Orders.Add(p);
                _context.SaveChanges();
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            } 
        }
        
        [HttpPut("updateOrder")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: orders update: 1")]
        public IActionResult updateOrder(EditDtos req)
        {
            try
            {
                var upOrder = _context.Orders.Find(req.idOrder);
                if (upOrder == null)
                {
                    return NotFound("khong tim thay du lieu");
                }
                upOrder.idDevice = req.idDevice;
                upOrder.amount = req.amount;
                upOrder.userUpdated = req.userUpdated;
                upOrder.dateUpdated = DateTime.UtcNow;
                upOrder.note = req.note;
                _context.SaveChangesAsync();
                return Ok("Cap nhap thanh cong");

            }
            catch (Exception ew)
            {
                return BadRequest(ew);
            }
        }

        [HttpPut("deleteOrder/{id}")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: orders delete: 1")]
        public IActionResult deleteOrder(int id, IdUserChangeOrderDto request)
        {
            try
            {
                var dev = _context.Orders.SingleOrDefault(dev => dev.idOrder == id);
                if (dev == null)
                {
                    return NotFound("Khong tim thay du lieu");
                }
                else
                {
                    dev.isDeleted = 1;
                    dev.userUpdated = request.UserId;
                    dev.dateUpdated = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Ok("Success");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /*[HttpPost]
        [Route("getPaginate")]
        [Authorize]
        public async Task<ActionResult> GetAllOrder(PaginateOrder p)
        {
            try
            {
                if (p.recordNum == 0)
                    p.recordNum = 10;
                if (p.page == 0)
                    p.page = 1;
                var ord = await _context.Orders.OrderByDescending(o => o.dateCreated).ToListAsync();
                if (ord == null)
                    return NotFound();
                var result = ord.ToPagedList(p.page, p.recordNum);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }*/
    }
}

