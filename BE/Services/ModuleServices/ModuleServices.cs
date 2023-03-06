using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.ModuleServices
{
    public interface IModuleServices
    {
        Task<BaseResponse<List<Module>>> GetAllModuleAsync();
        Task<BaseResponse<Module>> GetModuleById(int id);
        Task<BaseResponse<Module>> CreateModule(ModuleDtos moduleDtos);
        Task<BaseResponse<Module>> UpdateModule(int id, ModuleDtos moduleDtos);
        Task<BaseResponse<Module>> DeleteModule(int id);
    }

    public class ModuleServices : IModuleServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ModuleServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Module>>> GetAllModuleAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Module>();
            try
            {
                var module = await _db.modules.Where(s => s.isDeleted == 0).OrderByDescending(s => s.idSort).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(module);
                return (new BaseResponse<List<Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Module>> GetModuleById(int id)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.OrderByDescending(s => s.idSort).Where(s => s.isDeleted == 0 && s.id.Equals(id)).FirstOrDefaultAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<Module>(success, message, module));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<Module>(success, message, new Module()));
            }
        }

        public async Task<BaseResponse<Module>> CreateModule(ModuleDtos moduleDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var module = _mapper.Map<Module>(moduleDtos);
                module.isDeleted = 0;
                await _db.modules.AddAsync(module);
                await _db.SaveChangesAsync();

                success = true;
                message = "Add new Module successfully";
                return new BaseResponse<Module>(success, message, module);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Module failed! {ex.Message}";
                return new BaseResponse<Module>(success, message, new Module());
            }
        }

        public async Task<BaseResponse<Module>> UpdateModule(int id, ModuleDtos moduleDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.Where(s => s.isDeleted == 0 && s.id.Equals(id)).FirstOrDefaultAsync();
                if (module is null)
                {
                    message = "Module doesn't exist !";
                    return new BaseResponse<Module>(success, message, new Module());
                }
                var moduleMapData = _mapper.Map<ModuleDtos, Module>(moduleDtos, module);

                _db.modules.Update(moduleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Module successfully";
                return new BaseResponse<Module>(success, message, moduleMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Module failed! {ex.Message}";
                return new BaseResponse<Module>(success, message, new Module());
            }
        }

        public async Task<BaseResponse<Module>> DeleteModule(int id)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.Where(s => s.isDeleted == 0 && s.id.Equals(id)).FirstOrDefaultAsync();
                if (module is null)
                {
                    message = "Group doesn't exist !";
                    return new BaseResponse<Module>(success, message, new Module());
                }

                module.isDeleted = 1;
                _db.modules.Update(module);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Module successfully";
                return new BaseResponse<Module>(success, message, module);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting UserGroup failed! {ex.InnerException}";
                return new BaseResponse<Module>(success, message, new Module());
            }
        }


    }

}
