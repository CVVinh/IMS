using BE.Data.Contexts;
using BE.Data.Dtos.Handover;
using BE.Data.Dtos.HandoverDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BE.Data.Enum.Handover.Status;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: handovers")]
    public class HandoversController : ControllerBase
    {
        private readonly AppDbContext _context;
        public HandoversController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("addHandover")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: handovers add: 1")]
        public async Task<IActionResult> addHandover(AddHandover request)
        {
            try
            {
                var result = _context.Handovers.SingleOrDefault(x => x.IdDevice == request.IdDevice && x.UserReceive == request.UserReceive);
                var handover = new Handover();
                handover.IdDevice = request.IdDevice;
                handover.UserReceive = request.UserReceive;
                handover.Amount = request.Amount;
                handover.UserCreated = request.UserCreated;
                handover.DateCreated = request.DateCreated;
                handover.UserUpdated = request.UserCreated;
                handover.DateUpdated = request.DateCreated;
                _context.Handovers.Add(handover);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("updateHandover")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: handovers update: 1")]
        public async Task<IActionResult> updateHandover(UpdateHandover request)
        {
            try
            {
                var handover = _context.Handovers.Find(request.IdHandover);
                if (handover != null)
                {
                    handover.IdDevice = request.IdDevice;
                    handover.UserReceive = request.UserReceive;
                    handover.Amount = request.Amount;
                    handover.UserUpdated = request.UserUpdated;
                    handover.DateUpdated = request.DateUpdated;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound("Handover not exist");

            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("ConfirmHandover")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: handovers confirm: 1")]
        public async Task<IActionResult> confirmHandover(string[] idHandover)
        {
            try
            {
                for (int i = 0; i < idHandover.Length; i++)
                {
                    var result = await _context.Handovers.FindAsync(int.Parse(idHandover[i]));
                    if (result != null)
                    {
                        result.Status = StatusHandover.Done;
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("getListHandover")]
        public async Task<IActionResult> getListHandover()
        {
            try
            {
                var query = await _context.Handovers.Join(_context.Devices, hand1 => hand1.IdDevice, device => device.IdDevice, (hand1, device) => new { hand1, device })
                    .Join(_context.Users, hand2 => hand2.hand1.UserReceive, user => user.id, (hand2, user) => new { hand2, user })
                    .Select(p => new { p.hand2.device.DeviceName, p.user.firstName, p.user.lastName, p.hand2.hand1 })
                    .ToListAsync();
                if (query.Count() != 0)
                {
                    return Ok(query);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return BadRequest("Wrong data enter");
        }

        [HttpPut]
        [Route("deleteHandover/{idhand}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: handovers delete: 1")]
        public async Task<IActionResult> deleteHandover(int idhand, IdUserChangeHandoverDto request)
        {
            try
            {
                var hand = await _context.Handovers.SingleOrDefaultAsync(p => p.IdHandover == idhand);
                if (hand == null)
                {
                    return NotFound();
                }
                else
                {
                    hand.IsDeleted = 1;
                    hand.UserUpdated = request.UserId;
                    hand.DateUpdated = DateTime.Now;
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("getPaginateHandover")]
        public async Task<IActionResult> getPaginateHandover(PaginateHandover paginate)
        {
            try
            {
                if (paginate.recordNum == 0)
                {
                    paginate.recordNum = 10;
                }
                if(paginate.page == 0)
                {
                    paginate.page = 1;
                }

                var hand = await _context.Handovers
                    .OrderByDescending(h => h.DateCreated)
                    .Skip((paginate.page - 1) * paginate.recordNum)
                    .Take(paginate.recordNum).ToListAsync();

                if (hand == null)
                {
                    return NotFound();
                }
                
                var pageCount = Math.Ceiling(hand.Count() / Convert.ToDecimal(paginate.recordNum));              
                
                var response = new HandoverResponse
                {
                    Handover = hand,
                    CurrentPage = paginate.page,
                    Pages = (int)pageCount
                };

                return Ok(response);
            } catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
