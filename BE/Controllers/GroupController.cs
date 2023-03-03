﻿ using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using Group = BE.Data.Models.Group;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: groups")]
    public class GroupController: ControllerBase
    {
        private readonly AppDbContext _context;
        
        public GroupController(AppDbContext context)
        {
            _context = context;
            
        }
       
        [HttpGet("getListGroup")]
        public ActionResult getListGroup()
        {
            try
            {
                var listGroup = _context.Groups.Where(x => x.IsDeleted == 0).ToList();
                return Ok(listGroup);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getUserByGroup/{idGroup}")]
        public async Task<ActionResult> getUserByGroup(int idGroup) {
            try
            {
               
                var user = await (from u in _context.Users
                            join g in _context.UserGroups
                            on u.id equals g.idUser
                            where g.idGroup == idGroup && g.isDeleted == false
                            select new
                            {
                                firstName = u.firstName,
                                lastName = u.lastName,
                                id =u.id,
                            }).ToListAsync();
                if (user!= null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound("Khong tim thay du lieu");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }                     
        }
        
        [HttpPost("addGroup")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: groups add: 1")]
        public async Task<IActionResult> addGroup(AddGroupDtos req)
        {
            try
            {
                var add_g = await _context.Groups.SingleOrDefaultAsync(g => g.NameGroup!.ToLower() == req.NameGroup!.ToLower());
                if (add_g == null)
                {
                    var g = new Group();
                    g.NameGroup=req.NameGroup;
                    g.Discription=req.Discription;
                    g.userCreated=req.userCreated;
                    g.dateCreated = DateTime.UtcNow;
                    g.Key = g.NameGroup!.ToLower();
                    _context.Groups.Add(g);
                    await _context.SaveChangesAsync();
                    return Ok(g);

                }
                return BadRequest("Da Ton Tai");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

 
        [HttpPut("updateGroup/{id}")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: groups update: 1")]
        public async Task<ActionResult> updateGroup(int id, UpdateGroupDtos req)
        {
            try
            {
                var updateG =  await _context.Groups.Where(x => x.IsDeleted == 0).FirstOrDefaultAsync(x => x.Id == id);
                if (updateG != null)
                {
                    updateG.NameGroup=req.NameGroup;
                    updateG.Discription=req.Discription;
                    updateG.userModified =req.userModified;
                    updateG.dateModified= DateTime.Now;

                    await _context.SaveChangesAsync();
                    return Ok();

                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }     
        }

        [HttpPut("deleteGroup/{id}")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: groups delete: 1")]
        public async Task<ActionResult> deleteGroup(int id)
        {
            try
            {
                var deleteG = await _context.Groups.Where(x => x.IsDeleted == 0).FirstOrDefaultAsync(g => g.Id == id);
                if (deleteG != null)
                {
                    deleteG.IsDeleted = 1;
                    deleteG.dateModified = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok();
                }    
                else return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("exportExcel")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: groups export: 1")]
        public async Task<string> DownloadFile()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            // get data from DB
            var _data = await _context.Groups.ToListAsync();
            var columns_name = typeof(Group).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();
            // table header
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var cell = ws.Cell(1, idx + 1);
                cell.Value = columns_name[idx];
            }
            // table data
            ws.Cells("A2").Value = _data;
            // Apply style to excel
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var col = ws.Column(idx + 1);
                col.AdjustToContents();
            }
            // border for table
            IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(_data.Count() + 1, columns_name.Length).Address);
            data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            // save file to excel folder
            wb.SaveAs("..\\FE\\Excel\\Group_Table.xlsx");
            return "Excel\\Group_Table.xlsx";
        }


    }
}
