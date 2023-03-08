using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.BlockingWebDto;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Extentions;
using BE.Data.Models;
using BE.Response;
using BE.shared.Interface;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System;

namespace BE.Services.BlockingWebService
{
    public class BlockingWebService : IBlockingWebService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public BlockingWebService(AppDbContext appContext, IMapper mapper)
        {
            _context = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<BlockingWeb>> AddBlockingWebAsync(AddBlockingWebDto addBlocking)
        {
            var success = false;
            var message = "";
            var data = new BlockingWeb();
            try
            {
                var listBlocking = new List<BlockingWeb>();
                foreach (var item in addBlocking.arrayBlockWeb)
                {
                    var blockItem = new BlockingWeb()
                    {
                        dateCreated = DateTime.Now,
                        userCreated = addBlocking.userCreate,
                        linkBlockingWeb = item.Trim(),

                    };
                    if (!_context.BlockingWebs.Any(s => s.linkBlockingWeb.Trim().ToLower() == item.Trim().ToLower()))
                    {
                        listBlocking.Add(blockItem);
                    }
                }
                await _context.BlockingWebs.AddRangeAsync(listBlocking);
                await _context.SaveChangesAsync();

                success = true;
                message = "Thêm thành công các trang web bị chặn";
                return new BaseResponse<BlockingWeb>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Có sự cố gì xảy ra đối với hệ thống ! {ex.Message}";
                data = null;
                success = false;
                return new BaseResponse<BlockingWeb>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<BlockingWeb>>> GetAllBlockWebAsync()
        {
            var success = false;
            var message = "";
            var data = new List<BlockingWeb>();
            try
            {
                var listBlocking = await _context.BlockingWebs.Where(s => s.isDeleted == false).OrderByDescending(s => s.dateCreated).ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(listBlocking);
                return (new BaseResponse<List<BlockingWeb>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<BlockingWeb>>(success, message, data));
            }
        }
    }
}
