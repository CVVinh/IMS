using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.WikiCateDtos;
using BE.Data.Dtos.WikiDtos;
using BE.Data.Enum.Wiki;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using Microsoft.EntityFrameworkCore;
namespace BE.Services.Wiki
{
    public interface IWikiCategogyService
    {
       
        Task<BaseResponse<Wiki_CateGogy>> addNewWikiCateAsync(addWiki_Categogy addNewWikiCateDto);
        Task<BaseResponse<Wiki_CateGogy>> getWikiCateById(int ID);
        Task<BaseResponse<List<Wiki_CateGogy>>> getAllWikiCate();
        Task<BaseResponse<Wiki_CateGogy>> editWikiCate(editWikiCate editWikiCate, int ID);
        Task<BaseResponse<Wiki_CateGogy>> deleteWikiCate(int ID);

    }
    public class WikiCategogyServices: IWikiCategogyService
    {
        private readonly AppDbContext _appContext;
      
        public WikiCategogyServices(AppDbContext appContext)
        {
            _appContext = appContext;
        }
        public async Task<BaseResponse<Wiki_CateGogy>> addNewWikiCateAsync(addWiki_Categogy addNewWikiCateDto)
        {
            var success = false;
            var message = "";
            var data = new Wiki_CateGogy();
           
            try
            {

                var newWikiCate = new Wiki_CateGogy
                {
                    wikiName= addNewWikiCateDto.wikiName,
                    wiktNote=addNewWikiCateDto.wikiNote
                   
                };
                var checkWikiCate = await _appContext.Wiki_Categogy.Where(t => t.wikiName == addNewWikiCateDto.wikiName).FirstOrDefaultAsync();
                if (checkWikiCate is null)
                {
                    await _appContext.Wiki_Categogy.AddAsync(newWikiCate);
                    await _appContext.SaveChangesAsync();

                    success = true;
                    message = "Add new wiki categogy successfully";
                    data = newWikiCate;
                    return new BaseResponse<Wiki_CateGogy>(success, message,data);
                 
                }
                    message = "WikiCate does not exist";
                    return new BaseResponse<Wiki_CateGogy>(success, message, data);
                
            }
            catch
            {
                message = "Adding new wiki categogy failed!";
                data = null;
                return new BaseResponse<Wiki_CateGogy>(success, message, data);
            }
        }
        public async Task<BaseResponse<Wiki_CateGogy>> getWikiCateById(int ID)
        {
            
            var success = false;
            var message = "";
            var data = new Wiki_CateGogy();
            try
            {
                var checkIdWikiCate = await _appContext.Wiki_Categogy.Where(t => t.idCateWiki == ID).FirstOrDefaultAsync();
                if (checkIdWikiCate is null)
                {
                    message = "Id WikiCate does not exist";
                    return new BaseResponse<Wiki_CateGogy>(success, message, data);
                }

                success = true;
                message = "Getting WikiCategogy by id WikiCate sucessfully";
                data = checkIdWikiCate;
                return new BaseResponse<Wiki_CateGogy>(success, message, data);
            }
            catch
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<Wiki_CateGogy>(success, message, data);
            }
        }
        public async Task<BaseResponse<List<Wiki_CateGogy>>> getAllWikiCate()
        {
            List<Wiki_CateGogy>? listWikiCategogy = null;
            var success = false;
            var message = "";
            var data = listWikiCategogy;
            try
            {

                data = await _appContext.Wiki_Categogy.ToListAsync();
                success = true;
                message = "Getting All WikiCategogy sucessfully";

                return new BaseResponse<List<Wiki_CateGogy>>(success, message, data);
            }
            catch
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<List<Wiki_CateGogy>>(success, message, data);
            }
        }
        public async Task<BaseResponse<Wiki_CateGogy>> editWikiCate(editWikiCate editWikiCate, int ID)
        {
            var success = false;
            var message = "";
            var data = new Wiki_CateGogy();
            try
            {
                var checkIdWikiCate = await _appContext.Wiki_Categogy.Where(t => t.idCateWiki == ID).FirstOrDefaultAsync();
                if (checkIdWikiCate is null)
                {
                    message = "Id WikiCate does not exist";
                    return new BaseResponse<Wiki_CateGogy>(success, message, checkIdWikiCate);
                }
                {                             
                       
                    var checkWikiCate = await _appContext.Wiki_Categogy.Where(t => t.wikiName == editWikiCate.wikiName).FirstOrDefaultAsync();
                    if (checkWikiCate is null)
                    {
                        checkIdWikiCate.wikiName = editWikiCate.wikiName;
                        checkIdWikiCate.wiktNote = editWikiCate.wikiNote;
                       
                         await _appContext.SaveChangesAsync();

                        success = true;
                        message = "Edit new wiki categogy successfully";
                    
                        return new BaseResponse<Wiki_CateGogy>(success, message, checkIdWikiCate);
                    }
                    message = "WikiCate does not exist";
                    return new BaseResponse<Wiki_CateGogy>(success, message, data);
                   

                }

            }
            catch
            {
                message = "Don't connect database !";
               
                return new BaseResponse<Wiki_CateGogy>(success, message, data);
            }

        }
        public async Task<BaseResponse<Wiki_CateGogy>> deleteWikiCate(int ID)
        {
            var success = false;
            var message = "";
            var data = new Wiki_CateGogy();

            try
            {
                var result = await _appContext.Wiki_Categogy.SingleOrDefaultAsync(h => h.idCateWiki == ID);
                if (result is null)
                {
                    message = "Don't find database !";
                    data = null;
                    return new BaseResponse<Wiki_CateGogy>(success, message, data);
                }
                {
                _appContext.Wiki_Categogy.Remove(result);

                await _appContext.SaveChangesAsync();
                success = true;
                message = "Delete wiki categogy successfully";
                return new BaseResponse<Wiki_CateGogy>(success, message, data);
                }
                

            }
            catch (Exception)
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<Wiki_CateGogy>(success, message, data);
            }
        }

    }
}
