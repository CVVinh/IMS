using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.PaidServices
{
    public interface IPaidServices
    {
        Task<BaseResponse<List<Paid>>> GetAllAsync();
        Task<BaseResponse<List<Paid>>> GetPaidWithUserId(int userId);
        Task<BaseResponse<Paid>> CreatePaid(CreatePaidDtos createPaidDtos,string root, string rootPath);
        Task<BaseResponse<Paid>> DeletePaid(int idPaid);
        Task<BaseResponse<Paid>> EditPaid(int id, CreatePaidDtos createPaidDtos, string root, string local);
        Task<BaseResponse<Paid>> GetPaidWithId(int Id);
    }
    public class PaidService : IPaidServices
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public PaidService(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }
        public async Task<BaseResponse<List<Paid>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Paid>();
            try
            {
                var paids = await _appContext.Paids.Include(x => x.paidImages).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(paids);
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
        }
        public async Task<BaseResponse<List<Paid>>> GetPaidWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<Paid>();
            try
            {
                var paids = await _appContext.Paids.Where(x => x.PaidPerson == userId).Include(x => x.paidImages).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(paids);
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
        }
        public async Task<BaseResponse<Paid>> CreatePaid(CreatePaidDtos createPaidDtos, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                if (createPaidDtos == null)
                {
                    throw new ArgumentNullException(nameof(createPaidDtos), "createPaidDtos cannot be null");
                }

                var paid = _mapper.Map<Paid>(createPaidDtos);
                if (createPaidDtos.paidImage != null)
                {
                    foreach (var item in createPaidDtos.paidImage)
                    {
                        paid.paidImages = paid.paidImages ?? new List<PaidImage>();
                        paid.paidImages.Add(new PaidImage()
                        {
                            ImagePath = local + FilesHelper.UploadFileAndReturnPath(item, root, "/PaidPicture/")
                        });
                    }
                }
                else
                {
                    paid.paidImages = new List<PaidImage>();
                }

                if (paid != null)
                {
                    await _appContext.Paids.AddAsync(paid);
                    await _appContext.SaveChangesAsync();

                    success = true;
                    message = "Add new Paid successfully";
                    data = paid;
                }

                return new BaseResponse<Paid>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Adding new Paid failed! {ex.Message}";
                data = null;
                return new BaseResponse<Paid>(success, message, data);
            }
        }


        public async Task<BaseResponse<Paid>> DeletePaid(int idPaid)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paid = await _appContext.Paids.Where(p => p.Id == idPaid).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist !";
                    data = null;
                    return new BaseResponse<Paid>(success, message, data);
                }

                _appContext.Paids.Remove(paid);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Deleting leave off successfully";
                data = paid;
                return new BaseResponse<Paid>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Deleting leave off failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<Paid>(success, message, data);
            }
        }

        public async Task<BaseResponse<Paid>> EditPaid(int id, CreatePaidDtos createPaidDtos, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paid = await _appContext.Paids.Where(p => p.Id == id).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "Paid doesn't exist !";
                    data = null;
                    return new BaseResponse<Paid>(success, message, data);
                }
                if (createPaidDtos.paidImage != null)
                {
                    foreach (var item in createPaidDtos.paidImage)
                    {
                        paid.paidImages = paid.paidImages ?? new List<PaidImage>();
                        paid.paidImages.Add(new PaidImage()
                        {
                            ImagePath = local + FilesHelper.UploadFileAndReturnPath(item, root, "/PaidPicture/")
                        });
                    }
                }
                else
                {
                    paid.paidImages = new List<PaidImage>();
                }
                var paidMap = _mapper.Map<CreatePaidDtos, Paid>(createPaidDtos, paid);
                _appContext.Paids.Update(paidMap);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Editing Paid successfully";
                data = paidMap;
                return new BaseResponse<Paid>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Editing leave off failed! {ex.Message}";
                data = null;
                return new BaseResponse<Paid>(success, message, data);
            }
        }
        public async Task<BaseResponse<Paid>> GetPaidWithId(int Id)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paids = await _appContext.Paids.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (paids is null)
                {
                    message = "paid doesn't exist !";
                    data = null;
                    return new BaseResponse<Paid>(success, message, data);
                }
                success = true;
                data = paids;
                message = "Get all data successfully";
                return (new BaseResponse<Paid>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<Paid>(success, message, data));
            }
        }
    }
}
