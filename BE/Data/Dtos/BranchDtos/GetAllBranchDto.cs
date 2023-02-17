using BE.Data.Models;
namespace BE.Data.Dtos.BranchDtos
{
    public class GetAllBranchDto : Branchs
    {
        public GetAllBranchDto(Branchs brands)
        {
            idBranch = brands.idBranch;
            branchName = brands.branchName;
        }
    }
}
