using BE.Data.Contexts;
using FluentValidation;

namespace BE.Data.Dtos.WikiPost.Validator
{
    public class addWikiPostValidator : AbstractValidator<addWikiPost>
    {
        private readonly AppDbContext _context;
        public addWikiPostValidator(AppDbContext context)
        {
            _context = context;
            RuleFor(x => x.title).NotEmpty().WithMessage("Title is not empty");
            RuleFor(x => x.title).MaximumLength(80).WithMessage("Title can not 80 characters");
            RuleFor(x => x.idCateWiki).NotEmpty().WithMessage("idCateWiki is not empty");
            RuleFor(x => x.idCateWiki).Must(cateWikiExist).WithMessage("idCateWiki doesn't exist !");
            RuleFor(x => x.userCrete).NotEmpty().WithMessage("user cerate is not empty");
            RuleFor(x => x.userCrete).Must(userCreateExist).WithMessage("user create doesn't exist !");
            RuleFor(x => x.title).Must(titleExist).WithMessage("title doesn't exist !");
            
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
        private bool userCreateExist(int idUsser)
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

        #endregion
    }
}
