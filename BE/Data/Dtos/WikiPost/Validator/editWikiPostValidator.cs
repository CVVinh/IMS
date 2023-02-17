using BE.Data.Contexts;
using FluentValidation;

namespace BE.Data.Dtos.WikiPost.Validator
{
    public class editWikiPostValidator : AbstractValidator<editWikiPost>
    {
        private readonly AppDbContext _context;
        public editWikiPostValidator(AppDbContext context)
        {
            _context = context;
            RuleFor(x => x.title).NotEmpty().WithMessage("Title is not empty");
            RuleFor(x => x.title).MaximumLength(80).WithMessage("Title can not 80 characters");
            // RuleFor(x => x.title).Must(titleExist).WithMessage("title doesn't exist !");
            RuleFor(x => x.idCateWiki).NotEmpty().WithMessage("idCateWiki is not empty");
            RuleFor(x => x.idCateWiki).Must(cateWikiExist).WithMessage("idCateWiki doesn't exist !");
            RuleFor(x => x.userUpdate).NotEmpty().WithMessage("user update is not empty");
            RuleFor(x => x.userUpdate).Must(userUpdateExist).WithMessage("user update doesn't exist !");





        }

        #region testing functions



        private bool cateWikiExist(int idCateWiki)
        {
            var cateWiki = _context.Wiki_Categogy.Where(p => p.idCateWiki == idCateWiki).FirstOrDefault();
            if (cateWiki is null)
            {
                return false;
            }
            return true;
        }
        private bool userUpdateExist(int idUsser)
        {
            var user = _context.Users.Where(p => p.id == idUsser).FirstOrDefault();
            if (user is null)
            {
                return false;
            }
            return true;
        }

        private bool titleExist(string title)
        {
            var result = _context.Wiki_Post.Where(p => p.title == title).FirstOrDefault();
            if (result is null)
            {
                return true;
            }
            return false;
        }

        private bool checkDate(DateTime date, int ID)
        {
            var result = _context.Wiki_Post.Where(p => p.idPost == ID).FirstOrDefault();
            if (result == null)
            {
                return false;
            }
            if (result.dateCreate <= date)
            {
                return true;
            }
            return false;
        }


        #endregion
    }
}
