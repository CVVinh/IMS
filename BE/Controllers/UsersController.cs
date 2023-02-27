using BE.Data.Contexts;
using BE.Data.Dtos.UsersDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using BE.Services.TokenServices;
using BE.Data.Dtos.UserDtos;
using BE.Services.MailServices;
using BE.Services.MemberProjectServices;
using BE.Data.Dtos.MailDtos;
using BE.Response;
using BE.Services.PaginationServices;
using ClosedXML.Excel;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly MailService _mailService;

        private readonly IMemberProjectServices _memberProjectServices;
        private readonly IPaginationServices<Users> _paginationServices;

        //public static Users user = new Users();


        public UsersController(AppDbContext context, TokenService tokenService, IMemberProjectServices memberProjectServices, MailService mailService, IPaginationServices<Users> paginationServices)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _memberProjectServices = memberProjectServices ?? throw new ArgumentNullException(nameof(memberProjectServices));
            _paginationServices = paginationServices;
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendMail(MailDto mailDto)
        {
            try
            {
                await _mailService.SendMail(mailDto);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /*[HttpGet("getAllWithPaging")]
        //[Authorize(Roles = "Director,HR, Admin")]
        [Authorize(Roles = "permission_group: True module: remotes")]
        [Authorize(Roles = "controller: project action: click add: 1 update: 1 delete: 0 export: 1")]
        public async Task<ActionResult<Users>> GetAllWithPaging(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var users = await _context.Users.OrderByDescending(u => u.dateCreated).ToListAsync();
                if (users == null)
                    return NotFound();
                var result = users.ToPagedList(pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }*/

        [HttpGet("getAll")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<Users>> GetAll()
        {
            try
            {

                var users = await (from u in _context.Users
                                   from r in _context.Groups.Where(a => a.Id == u.IdGroup).DefaultIfEmpty()
                                   select new
                                   {
                                       id = u.id,
                                       userCode = u.userCode,
                                       userCreated = u.userCreated, // => userCode
                                       dateCreated = u.dateCreated,
                                       userModified = u.userModified, // => userCode
                                       dateModified = u.dateModified,
                                       firstName = u.firstName,
                                       lastName = u.lastName,
                                       phoneNumber = u.phoneNumber,
                                       dOB = u.dOB,
                                       gender = u.gender,
                                       address = u.address,
                                       university = u.university,
                                       yearGraduated = u.yearGraduated,
                                       email = u.email,
                                       skype = u.skype,
                                       workStatus = u.workStatus,
                                       dateStartWork = u.dateStartWork,
                                       dateLeave = u.dateLeave,
                                       maritalStatus = u.maritalStatus,
                                       reasonResignation = u.reasonResignation,
                                       identityCard = u.identitycard,
                                       idGroup = r.NameGroup,
                                       isDeleted = u.isDeleted,
                                   }).OrderByDescending(a => a.id).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getAllUsersByIdProject/{idProject}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<IActionResult> getAllUsersByIdProject(int idProject)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                var member = await _memberProjectServices.GetMembersByIdProjectAsync(idProject);
                if (member._success)
                {
                    foreach (var result in member._Data)
                    {
                        var user = await _context.Users.FindAsync(result.member);
                        if (user == null)
                        {
                            return NotFound();
                        }
                        data.Add(user);
                    }

                    success = true;
                    messgae = "Get All Users By IdProject Succsessfully";
                    return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                }
                messgae = member._Message;
                data = null;
                return BadRequest(new BaseResponse<List<Users>>(success, messgae, data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                data = null;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getUsersOutsideProject/{idProject}")]
        public async Task<IActionResult> getAllUsersOutsideProject(int idProject)
        {
            var success = false;
            var messgae = "";
            var data = new List<Users>();
            try
            {
                var listAllUsers = await _context.Users.Where(u => (u.workStatus == 1 &&
                (u.IdGroup == 4 ))).ToListAsync();
                List<Users> listUserProject = new List<Users>();
                var member = await _memberProjectServices.GetMembersByIdProjectAsync(idProject);
                if (member._success)
                {
                    foreach (var result in member._Data)
                    {
                        var user = await _context.Users.FindAsync(result.member);
                        listUserProject.Add(user);

                    }

                    IEnumerable<Users> users = listAllUsers.Except<Users>(listUserProject);
                    data = users.ToList();
                    success = true;
                    messgae = "Get All Users outside project Succsessfully";
                    return Ok(new BaseResponse<List<Users>>(success, messgae, data));
                }
                messgae = member._Message;
                data = null;
                return BadRequest(new BaseResponse<List<Users>>(success, messgae, data));
            }
            catch (Exception ex)
            {
                messgae = ex.Message;
                data = null;
                return StatusCode(500, new BaseResponse<List<Users>>(success, messgae, data));
            }
        }

        [HttpGet("getUserById/{id}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<Users>> GetUsersById(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet("getUserByUserCode/{usercode}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<Users>> GetUsersByUserCode(string usercode)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.userCode.ToLower() == usercode.ToLower()).SingleOrDefaultAsync();
            if (users == null)
                return NotFound();
            var data = new GetAllDto(users);
            return Ok(data);
        }

        [HttpGet("getByGender/{gender}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<Users>> GetUsersByGender(int gender)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.gender == gender).ToListAsync();
            var data = new List<GetAllDto>();
            foreach (var user in users)
            {
                data.Add(new GetAllDto(user));
            }
            return Ok(data);
        }

        [HttpGet("getByWorkStatus/{workStatus}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<Users>> GetUsersByWorkStatus(int workStatus)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.workStatus == workStatus).ToListAsync();
            var data = new List<GetAllDto>();
            foreach (var user in users)
            {
                data.Add(new GetAllDto(user));
            }
            return Ok(data);
        }


        [HttpGet("getBydOB/{dOB}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<Users>> GetUsersByDOB(DateTime dOB)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(u => u.dOB == dOB).ToListAsync();
            var data = new List<GetAllDto>();
            foreach (var user in users)
            {
                data.Add(new GetAllDto(user));
            }
            return Ok(data);
        }



        [HttpGet("userDropdown")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<IEnumerable<DropdownUserDto>>> GetUsersDropdown()
        {
            var users = await _context.Users.ToListAsync();
            var userDropdown = new List<DropdownUserDto>();
            foreach (var user in users)
            {
                userDropdown.Add(new DropdownUserDto(user));
            }
            return Ok(userDropdown);
        }

        [HttpGet("getInfo")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<ActionResult<UserInfoDto>> GetInfo()
        {
            var users = await _context.Users.ToListAsync();
            var usersInfo = new List<UserInfoDto>();
            if (users == null)
            {
                return NotFound();
            }
            foreach (var user in users)
            {
                usersInfo.Add(new UserInfoDto(user));
            }
            return Ok(usersInfo);
        }

        [HttpPut("deleteUser/{id}")]
        [Authorize(Roles = "permission_group: True module: users")]
        [Authorize(Roles = "module: users delete: 1")]
        public async Task<IActionResult> DeleteUser(int id, DeleteUserDto dto)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            dto.isDeleted = 1;

            users.isDeleted = dto.isDeleted;
            users.userModified = dto.userModified;
            users.dateModified = dto.dateModified;

            await _context.SaveChangesAsync();

            return Ok("Delete success...");
        }

        [HttpGet("getGroupByUser/{id}")]
        [Authorize(Roles = "permission_group: True module: users")]
        public async Task<IActionResult> GetRoleUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User not exist...");
            }
            return Ok(user.IdGroup);
        }

        [HttpPost("addUser")]
        //[Authorize(Roles = "permission_group: True module: users")]
        //[Authorize(Roles = "module: users add: 1")]
        public async Task<ActionResult<AddUserDto>> CreateUser(AddUserDto request)
        {
            try
            {
                var userexist = _context.Users.Where(u => u.userCode == request.userCode);
                if (userexist.Any())
                {
                    return BadRequest("Usercode is exist");
                }

                var isNumber = request.identitycard.All(char.IsDigit);
                if (!isNumber)
                    return BadRequest("Identity card is not a number");
                if (request.identitycard.Length != 9 && request.identitycard.Length != 12)
                    return BadRequest("Identity number's length must be 9 or 12.");

                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == request.identitycard);
                if (identityExist.Any())
                    return BadRequest("Identity number is exist");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == request.phoneNumber);
                if (request.phoneNumber != null)
                {
                    if (phoneExist.Any() && request.phoneNumber.Length != 0)
                        return BadRequest("Phone number is exist");
                }

                var emailExist = _context.Users.Where(u => u.email == request.email);
                if (emailExist.Any())
                    return BadRequest("Email is exist");
                var user = new Users
                {

                    userCode = request.userCode,
                    userPassword = request.userPassword == null ?
                        BCrypt.Net.BCrypt.HashPassword(request.userCode) :
                        BCrypt.Net.BCrypt.HashPassword(request.userPassword), // if password is null -> password is usercode
                    userCreated = request.userCreated,
                    dateCreated = request.dateCreated,
                    firstName = request.firstName,
                    lastName = request.lastName,
                    phoneNumber = request.phoneNumber,
                    dOB = request.dOB,
                    gender = request.gender,
                    address = request.address,
                    university = request.university,
                    yearGraduated = request.yearGraduated,
                    email = request.email,
                    emailPassword = request.emailPassword == null ? null : BCrypt.Net.BCrypt.HashPassword(request.emailPassword),
                    skype = request.skype,
                    skypePassword = request.skypePassword == null ? null : BCrypt.Net.BCrypt.HashPassword(request.skypePassword),
                    workStatus = 1,
                    dateStartWork = request.dateStartWork,
                    maritalStatus = request.maritalStatus,
                    reasonResignation = request.reasonResignation,
                    identitycard = request.identitycard,
                    IdGroup = request.IdGroup,
                    isDeleted = 0,
                    refreshToken = null,
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("updateUser/{id}")]
        [Authorize(Roles = "permission_group: True module: users")]
        [Authorize(Roles = "module: users update: 1")]
        public async Task<ActionResult<Users>> UpdateUser(int id, EditUserDto user_dto)
        {
            var mess = "";
            try
            {
                var user = await _context.Users.FindAsync(id);
                var editor = await _context.Users.Where(u => u.id == user_dto.userModified).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("User does not exist !");
                }
                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == user_dto.identitycard && u.id != id);
                if (identityExist.Any())
                    return BadRequest("Identity number is exist");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == user_dto.phoneNumber && u.id != id && u.phoneNumber != null);
                if (phoneExist.Any())
                    return BadRequest("Phone number is exist");
                var emailExist = _context.Users.Where(u => u.email == user_dto.email && u.id != id);
                if (emailExist.Any())
                    return BadRequest("Email is exist");

                if (user_dto.userModified != null)
                {
                    user.dateModified = user_dto.dateModified == null ?  // if value is null, will get value in database
                        DateTime.Now : user_dto.dateModified;
                    user.firstName = user_dto.firstName == null ?
                        user.firstName : user_dto.firstName;
                    user.lastName = user_dto.lastName == null ?
                        user.lastName : user_dto.lastName;
                    user.phoneNumber = user_dto.phoneNumber;
                    user.dOB = user_dto.dOB;
                    user.gender = user_dto.gender == 0 ?
                        user.gender : user_dto.gender;
                    user.address = user_dto.address;
                    user.university = user_dto.university;
                    user.yearGraduated = user_dto.yearGraduated;
                    user.email = user_dto.email;
                    user.skype = user_dto.skype;
                    user.maritalStatus = user_dto.maritalStatus == null ?
                        user.maritalStatus : user_dto.maritalStatus;
                    user.reasonResignation = user_dto.reasonResignation;
                    user.userModified = user_dto.userModified;
                    user.identitycard = user_dto.identitycard;
                    user.workStatus = user_dto.workStatus == 0 ?
                                user.workStatus : user_dto.workStatus;
                    user.dateLeave = user_dto.dateLeave;
                    user.dateStartWork = user_dto.dateStartWork;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok(new ApiResponseDto
                    {
                        Success = true,
                        Message = mess == "" ? "Update success !" : mess, // if mess is empty => return Update success
                    });
                }
                return BadRequest("Cannot get information of editor !");
            }
            catch
            {
                return BadRequest("Unknown error !");
            }
        }

        [HttpPut("changePassword/{userCode}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userCode, [FromBody] ChangePassDto resource)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FirstOrDefaultAsync(d => d.userCode.Equals(userCode));
            if (users == null)
            {
                return NotFound("User does not exist !");
            }
            if (users.workStatus != 1)
            {
                return BadRequest("Your account has been locked !");
            }
            if (resource.newPassword.Equals(resource.oldPassword))
            {
                return BadRequest("New password can't same old password!");
            }
            if (BCrypt.Net.BCrypt.Verify(resource.oldPassword, users.userPassword))
            {
                users.userPassword = BCrypt.Net.BCrypt.HashPassword(resource.newPassword);
                await _context.SaveChangesAsync();
                return Ok("Change password successfully!");
            }
            return BadRequest("The current password is not correct!");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.userCode.ToLower() == loginDto.UserName.ToLower());
            if (user == null || user.isDeleted == 1)
            {
                return NotFound("Account does not exist !");
            }
            if (user.workStatus != 1)
            {
                return BadRequest("User has been locked !");
            }
            else
            {
                bool isPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.userPassword);
                if (isPassword)
                {
                    var accessToken = _tokenService.GenerateToken(user);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    user.refreshToken = refreshToken;
                    user.refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _context.SaveChangesAsync();
                    return Ok(new ApiResponseDto
                    {
                        Success = true,
                        Message = "Authenticate success...",
                        Username = loginDto.UserName,
                        Token = accessToken,
                    });
                }
                return BadRequest("Wrong password !");
            }
        }

        [HttpPost]
        [Route("ResetPass")]
        public async Task<IActionResult> GetNewPass([FromBody] string request)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Where(u => u.userCode == request).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound("Account does not exist !");
            }
            if (user.workStatus != 1)
            {
                return BadRequest("User has been locked !");
            }
            Guid id = Guid.NewGuid();
            string newPass = id.ToString().Substring(0, 8);
            user.userPassword = BCrypt.Net.BCrypt.HashPassword(newPass);
            var maildto = new MailDto();
            maildto.To = user.email;
            maildto.Subject = "Reset Password";
            maildto.Body = "This is new password: " + newPass;
            if (await _mailService.SendMail(maildto))
            {
                _context.SaveChanges();
                return Ok(new ApiResponseDto
                {
                    Success = true,
                    Message = "Please check your email !"
                });
            }
            else
            {
                return Ok(new ApiResponseDto
                {
                    Success = false,
                    Message = "Cannot reset password !"
                });
            }
        }

        [HttpGet("Pagination")]
        [Authorize]
        public async Task<ActionResult> GetUsers(int page)
        {
            if (_context.Users == null)
                return NotFound();

            var pageResults = 3f;
            var pageCount = Math.Ceiling(_context.Users.Count() / pageResults);

            var Users = await _context.Users
                .Skip((page - 1) * (int)pageResults)
                .Take((int)pageResults)
                .ToListAsync();

            var response = new UserResponse
            {
                Users = Users,
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return Ok(response);
        }

        [HttpPost("Logout")]

        public IActionResult Logout([FromBody] string Username)
        {
            var user = _context.Users.SingleOrDefault(u => u.userCode.ToLower() == Username.ToLower());
            if (user == null)
            {
                return BadRequest("User does not exist...");
            }
            else
            {
                user.refreshToken = null;
                user.refreshTokenExpiryTime = DateTime.MinValue;
                _context.SaveChanges();
                return Ok(new ApiResponseDto
                {
                    Success = true,
                });
            }
        }

        [HttpGet]
        [Route("exportExcel")]
        /*[Authorize(Roles = "permission_group: True module: groups")]
        [Authorize(Roles = "module: groups export: 1")]*/
        public async Task<string> DownloadFile()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            // get data from DB
            var _data = await _context.Users.ToListAsync();
            var columns_name = typeof(Users).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();
            // table header
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var cell = ws.Cell(1, idx + 1);
                cell.Value = columns_name[idx];
            }
            // table data
            ws.Cells("A2").Value = _data;
            // Apply style to excel
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var col = ws.Column(idx + 1);
                col.AdjustToContents();
            }
            // border for table
            IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(_data.Count() + 1, columns_name.Length).Address);
            data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            // save file to excel folder
            wb.SaveAs("..\\FE\\Excel\\Users_Table.xlsx");
            return "Excel\\Users_Table.xlsx";
        }
    }
}

