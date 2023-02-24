using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Services.PaidServices
{
    public interface IPaidServices
    {
        Task<BaseResponse<List<Paid>>> GetAllAsync();
        Task<BaseResponse<List<Paid>>> GetPaidWithUserId(int userId);
        Task<BaseResponse<Paid>> CreatePaid(CreatePaidDtos createPaidDtos,string root, string rootPath);
        Task<BaseResponse<Paid>> DeletePaid(int idPaid, string root, string local);
        Task<BaseResponse<Paid>> EditPaid(int id, CreatePaidDtos createPaidDtos, string root, string local);
        Task<BaseResponse<Paid>> GetPaidWithId(int Id);
        Task<BaseResponse<List<PaidImage>>> DeleteMutilImgPaid(int idPaid, List<int>? listIdImg, string root, string local);
        Task<BaseResponse<List<Paid>>> SearchPaidByDay(SearchDayPaidDtos searchDayPaidDtos);
        Task<BaseResponse<Paid>> AccepterPayment(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos);
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

        public async Task<BaseResponse<Paid>> GetPaidWithId(int Id)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paids = await _appContext.Paids.Where(x => x.Id == Id).Include(x => x.paidImages).FirstOrDefaultAsync();
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

        public async Task<BaseResponse<List<Paid>>> GetPaidWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<Paid>();
            try
            {
                var paids = await _appContext.Paids.Where(x => x.PaidPerson == userId).Include(x => x.paidImages).ToListAsync();
                if(paids is null)
                {
                    message = "Person do not have payment!";
                    data = null;
                    return new BaseResponse<List<Paid>>(success, message, data);
                }
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

        public async Task<BaseResponse<List<Paid>>> SearchPaidByDay(SearchDayPaidDtos searchDayPaidDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Paid>();

            if (searchDayPaidDtos.startDate == null)
            {
                success = false;
                message = "Getting paid failed! Input startDate argument are not provided!";
                return (new BaseResponse<List<Paid>>(success, message, data));
            }

            if (searchDayPaidDtos.endDate != null && searchDayPaidDtos.startDate > searchDayPaidDtos.endDate)
            {
                success = false;
                message = "startDate is not greater than endDate!";
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
            try
            {
                var getPaidByDay = new List<Paid>();
                if (searchDayPaidDtos.id <= 0)
                {
                    if (searchDayPaidDtos.endDate != null)
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate >= searchDayPaidDtos.startDate && x.PaidDate <= searchDayPaidDtos.endDate).Include(x => x.paidImages).ToListAsync();
                    }
                    else
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate >= searchDayPaidDtos.startDate).Include(x => x.paidImages).ToListAsync();
                    }
                }
                else
                {
                    if (searchDayPaidDtos.endDate != null)
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate >= searchDayPaidDtos.startDate && x.PaidDate <= searchDayPaidDtos.endDate && x.PaidPerson == searchDayPaidDtos.id).Include(x => x.paidImages).ToListAsync();
                    }
                    else
                    {
                        getPaidByDay = await _appContext.Paids.Where(x => x.PaidDate >= searchDayPaidDtos.startDate && x.PaidPerson == searchDayPaidDtos.id).Include(x => x.paidImages).ToListAsync();
                    }
                }
                success = true;
                message = "Get all data successfully";
                data = getPaidByDay;
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = "Getting paid failed! An error occurred: " + ex.Message;
                return (new BaseResponse<List<Paid>>(success, message, data));
            }
        }


        public async Task<BaseResponse<Paid>> DeletePaid(int idPaid, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paid = await _appContext.Paids.Where(p => p.Id == idPaid).Include(x => x.paidImages).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist !";
                    data = null;
                    return new BaseResponse<Paid>(success, message, data);
                }

                _appContext.Paids.Remove(paid);
                
                if (paid.paidImages.Count > 0)
                {
                    foreach (var item in paid.paidImages)
                    {
                        var fileName = item.ImagePath?.Replace($"{local}/PaidPicture/", "");
                        string filePath = System.IO.Path.Combine(root, "PaidPicture", fileName ?? "");
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        _appContext.PaidImages.Remove(item);
                    }
                }

                await _appContext.SaveChangesAsync();
                success = true;
                message = "Deleting paid successfully";
                data = paid;
                return new BaseResponse<Paid>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Deleting paid failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<Paid>(success, message, data);
            }
        }


        public async Task<BaseResponse<List<PaidImage>>> DeleteMutilImgPaid(int idPaid, List<int>? listIdImg, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new List<PaidImage>();
            try
            {
                var paid = await _appContext.PaidImages.Where(p => p.PaidId == idPaid).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist !";
                    data = null;
                    return new BaseResponse<List<PaidImage>>(success, message, data);
                }

                foreach (var item in listIdImg)
                {
                    var itemImg = await _appContext.PaidImages.Where(p => p.PaidId == idPaid && p.ImageId == item).FirstOrDefaultAsync();
                    if (itemImg != null)
                    {
                        var fileName = itemImg.ImagePath?.Replace($"{local}/PaidPicture/", "");
                        string filePath = System.IO.Path.Combine(root, "PaidPicture", fileName ?? "");
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        _appContext.PaidImages.Remove(itemImg);
                        data.Add(itemImg);
                    }
                    else if (itemImg == null)
                    {
                        message += "ImageId " + item +", ";
                    }
                }
                await _appContext.SaveChangesAsync();

                success = true;
                if(message == "")
                {
                    message = "Deleting list image successfully";
                }
                else
                {
                    message += "delete error!";
                }
                return new BaseResponse<List<PaidImage>>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Deleting paid failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<List<PaidImage>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Paid>> EditPaid(int id, CreatePaidDtos createPaidDtos, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paid = await _appContext.Paids.Where(p => p.Id == id).Include(x => x.paidImages).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "Paid doesn't exist !";
                    data = null;
                    return new BaseResponse<Paid>(success, message, data);
                }
                if (createPaidDtos.paidImage != null)
                {
                    //foreach(var item in paid.paidImages)
                    //{
                    //    var fileName = item.ImagePath?.Replace($"{local}/PaidPicture/", "");
                    //    string filePath = System.IO.Path.Combine(root, "PaidPicture", fileName ?? "");
                    //    if (File.Exists(filePath))
                    //    {
                    //        File.Delete(filePath);
                    //        _appContext.PaidImages.Remove(item);
                    //    }
                    //}
                    
                    foreach (var item in createPaidDtos.paidImage)
                    {
                        paid.paidImages = paid.paidImages ?? new List<PaidImage>();

                        paid.paidImages.Add(new PaidImage()
                        {
                            ImagePath = local + FilesHelper.UploadFileAndReturnPath(item, root, "/PaidPicture/")
                        });
                    }
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

        public async Task<BaseResponse<Paid>> AccepterPayment(int idPaid, AcceptPaymentPaidDtos acceptPaymentPaidDtos)
        {
            var success = false;
            var message = "";
            var data = new Paid();
            try
            {
                var paid = await _appContext.Paids.Where(x => x.Id == idPaid && x.IsPaid == false).FirstOrDefaultAsync();
                if (paid is null)
                {
                    message = "idPaid doesn't exist or payment!";
                    data = null;
                    return new BaseResponse<Paid>(success, message, data);
                }

                paid.IsPaid = true;
                paid.PaidPerson = acceptPaymentPaidDtos.PaidPerson;
                paid.PaidDate = DateTime.Now;
                _appContext.Paids.Update(paid);
                await _appContext.SaveChangesAsync();

                success = true;
                message = "Accepting payment successfully";
                data = paid;
                return new BaseResponse<Paid>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Accepting leave off failed! {ex.InnerException}";
                data = null;
                return new BaseResponse<Paid>(success, message, data);
            }
        }
        


    }
}
