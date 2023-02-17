

using BE.Data.Contexts;
using FluentValidation;

namespace BE.Data.Dtos.WikiCateDtos.Validator
{
    public class editWikiCateValidator: AbstractValidator<editWikiCate>
    {
        

            private readonly AppDbContext _context;
            public editWikiCateValidator(AppDbContext context)
            {
                _context = context;
                RuleFor(x => x.wikiName).NotEmpty().WithMessage("Wiki name is not empty")
                    .MaximumLength(50).WithMessage("Wiki name can not 50 characters");
            }




        
    }
}
