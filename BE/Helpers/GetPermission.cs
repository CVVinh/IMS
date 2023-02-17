using API_LVTN.DAL;
using BE.Data.Contexts;
using BE.Data.Dtos;
using static BE.Helpers.TokenHelper;

namespace BE.Helpers
{
    public class GetPermission
    {
        public static AppDbContext? _context;

        public static PermissionResponse GetPermissionForUser(Token token, int? idModule)
        {

            var response = new PermissionResponse
            {
                Get = false,
                Add = false,
                Delete = false,
                Update = false,
                Export = false,
            };

            if (idModule == null || token.User == 0 || token.Group == 0 || _context == null)
            {
                return response;
            }

            var permissionModule = _context.Permission_Use_Menus.FirstOrDefault(filter => filter.IdUser == token.User && filter.idModule == idModule);

            if (permissionModule == null)
            {
                var permissionGroup = _context.Permission_Groups.FirstOrDefault(filter => filter.Id == token.Group);
                if(permissionGroup == null)
                {
                    return response;
                }
                else
                {
                    var permissionModuleAccess = _context.Permission_Groups.FirstOrDefault(filter => filter.IdGroup == permissionGroup.IdGroup && filter.IdModule == idModule);
                    if (permissionModuleAccess != null && permissionModuleAccess.Access)
                    {
                        response.Get = true;
                        response.Add = true;
                        response.Delete = true;
                        response.Update = true;
                        response.Export = true;

                        return response;
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            else
            {
                if(permissionModule.Add == 1)
                {
                    response.Get = true;
                    response.Add = true;
                }
                if(permissionModule.Delete == 1)
                {
                    response.Get = true;
                    response.Delete = true;
                }
                if(permissionModule.Update == 1)
                {
                    response.Get = true;
                    response.Update = true;
                }
                if(permissionModule.Export == 1)
                {
                    response.Get = true;
                    response.Export = true;
                }

                return response;
            }

        }
    }
}
