using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace BE.Services.Customers
{

    public interface ICustomerService
    {
        Task<BaseResponse<List<Customer>>> GetAllAsync();
        Task<BaseResponse<Customer>> GetById(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;

        public CustomerService(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Customer>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Customer>();
            try
            {
                var customers = await _appContext.Customers.Where(s => s.isDeleted == false).OrderByDescending(s => s.dateCreated).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(customers);
                return (new BaseResponse<List<Customer>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Customer>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Customer>> GetById(int customerId)
        {
            var success = false;
            var message = "";
            var data = new Customer();
            try
            {
                var customers = await _appContext.Customers.Where(x => x.isDeleted == false &&  x.id == customerId).OrderByDescending(x => x.dateCreated).FirstOrDefaultAsync();

                if (customers is null)
                {
                    message = "customerId doesn't exist !";
                    data = null;
                    return new BaseResponse<Customer>(success, message, data);
                }
                success = true;
                data = customers;
                message = "Get data successfully";
                return (new BaseResponse<Customer>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<Customer>(success, message, data));
            }
        }


    }
}
