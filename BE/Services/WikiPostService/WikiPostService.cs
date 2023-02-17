namespace BE.Services.WikiPost;
using BE.Data.Contexts;
using BE.Data.Dtos.WikiPost;
using BE.Data.Enum.Wiki;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using Microsoft.EntityFrameworkCore;

public interface IWikiPostService
{
    Task<BaseResponse<Wiki_Post>> addNewWikiPostAsync(addWikiPost addWikiPost);
    Task<BaseResponse<Wiki_Post>> getWikiPostById(int ID);
    Task<BaseResponse<List<Wiki_Post>>> getAllWikiPost();
    Task<BaseResponse<Wiki_Post>> editWikiPost(editWikiPost editWikiPost, int ID);
    Task<BaseResponse<Wiki_Post>> deleteWikiPost(int ID);
    Task<BaseResponse<List<Wiki_Post>>> getAllWikiPostByIDCate(int IDCate);
}
public class WikiPostService: IWikiPostService
{
    private readonly AppDbContext _appDbContext;
    public WikiPostService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<BaseResponse<Wiki_Post>> addNewWikiPostAsync(addWikiPost addWikiPost)
    {
        var success = false;
        var message = "";
        var data = new Wiki_Post();

        try
        {

            var newWikiPost = new Wiki_Post
            {
                title = addWikiPost.title,
                content = addWikiPost.content,
                status = addWikiPost.status,
                note= addWikiPost.note,
                userCrete=addWikiPost.userCrete,             
                idCateWiki =addWikiPost.idCateWiki


            };

            await _appDbContext.Wiki_Post.AddAsync(newWikiPost);
            await _appDbContext.SaveChangesAsync();

            success = true;
            message = "Add new wiki post successfully";
            data = newWikiPost;
            return new BaseResponse<Wiki_Post>(success, message, data);
        }
        catch
        {
            message = "Adding new wiki post failed! Starting date or ending date isn't in the correct format !";
            data = null;
            return new BaseResponse<Wiki_Post>(success, message, data);
        }
    }
    public async Task<BaseResponse<Wiki_Post>> getWikiPostById(int ID)
    {

        var success = false;
        var message = "";
        var data = new Wiki_Post();
        try
        {
            var checkIdWikiCate = await _appDbContext.Wiki_Post.Where(t => t.idPost == ID).FirstOrDefaultAsync();
            if (checkIdWikiCate is null)
            {
                message = "Id WikiPost does not exist";
                return new BaseResponse<Wiki_Post>(success, message, data);
            }

            success = true;
            message = "Getting WikiPost by id WikiPost sucessfully";
            data = checkIdWikiCate;
            return new BaseResponse<Wiki_Post>(success, message, data);
        }
        catch
        {
            message = "Don't connect database !";
            data = null;
            return new BaseResponse<Wiki_Post>(success, message, data);
        }
    }
    public async Task<BaseResponse<List<Wiki_Post>>> getAllWikiPost()
    {
        List<Wiki_Post>? listWikiPost = null;
        var success = false;
        var message = "";
        var data = listWikiPost;
        try
        {

            data = await _appDbContext.Wiki_Post.ToListAsync();
            success = true;
            message = "Getting All WikiPost sucessfully";

            return new BaseResponse<List<Wiki_Post>>(success, message, data);
        }
        catch
        {
            message = "Don't connect database !";
            data = null;
            return new BaseResponse<List<Wiki_Post>>(success, message, data);
        }
    }
    public async Task<BaseResponse<Wiki_Post>> editWikiPost(editWikiPost editWikiPost, int ID)
    {
        var success = false;
        var message = "";
        try
        {
            var checkIdWikiPost = await _appDbContext.Wiki_Post.Where(t => t.idPost == ID).FirstOrDefaultAsync();
            if (checkIdWikiPost is null)
            {
                message = "Id Wiki Post does not exist";
                return new BaseResponse<Wiki_Post>(success, message, checkIdWikiPost);
            }
            {


                checkIdWikiPost.title = editWikiPost.title;
                checkIdWikiPost.content = editWikiPost.content;
                checkIdWikiPost.status=editWikiPost.status;
                checkIdWikiPost.note=editWikiPost.note;
                checkIdWikiPost.userUpdate = editWikiPost.userUpdate;
                checkIdWikiPost.idCateWiki = editWikiPost.idCateWiki;
                await _appDbContext.SaveChangesAsync();





                success = true;
                message = "Edit new wiki post successfully";

                return new BaseResponse<Wiki_Post>(success, message, checkIdWikiPost);

            }

        }
        catch
        {
            message = "Don't connect database !";
            var data = new Wiki_Post();
            return new BaseResponse<Wiki_Post>(success, message, data);
        }
    }
    public async Task<BaseResponse<Wiki_Post>> deleteWikiPost(int ID)
    {
        var success = false;
        var message = "";
        var data = new Wiki_Post();

        try
        {
            var result = await _appDbContext.Wiki_Post.SingleOrDefaultAsync(h => h.idPost == ID);
            if (result is null)
            {
                message = "Don't find database !";
                data = null;
                return new BaseResponse<Wiki_Post>(success, message, data);
            }
            {
                _appDbContext.Wiki_Post.Remove(result);

                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Delete wiki post successfully";
                return new BaseResponse<Wiki_Post>(success, message, data);
            }


        }
        catch (Exception)
        {
            message = "Don't connect database !";
            data = null;
            return new BaseResponse<Wiki_Post>(success, message, data);
        }
    }
    public async Task<BaseResponse<List<Wiki_Post>>> getAllWikiPostByIDCate(int IDCate)
    {
        List<Wiki_Post>? listWikiPost = null;
        var success = false;
        var message = "";
        var data = listWikiPost;
        try
        {

            data =  await _appDbContext.Wiki_Post.Where(t => t.idCateWiki == IDCate).ToListAsync();
            if (!data.Any())

            {

                message = "Id categogy does not exist";
                return new BaseResponse<List<Wiki_Post>>(success, message, data);
            }
            else
            {
                success = true;
                message = "Getting All WikiPost by IDCate sucessfully";

                return new BaseResponse<List<Wiki_Post>>(success, message, data);
            }
          
        }
        catch
        {
            message = "Don't connect database !";
            data = null;
            return new BaseResponse<List<Wiki_Post>>(success, message, data);
        }
    }




}
