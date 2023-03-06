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
                    messgae = "Nhận tất cả người dùng bằng IdProject thành công";
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
                (u.IdGroup == 4))).ToListAsync();
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
                    messgae = "Nhận tất cả người dùng bên ngoài dự án thành công";
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
                    return BadRequest("Mã người dùng đang tồn tại");
                }

                var isNumber = request.identitycard.All(char.IsDigit);
                if (!isNumber)
                    return BadRequest("Chứng minh nhân dân không phải là số");
                if (request.identitycard.Length != 9 && request.identitycard.Length != 12)
                    return BadRequest("Độ dài của số CMND phải là 9 hoặc 12.");

                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == request.identitycard);
                if (identityExist.Any())
                    return BadRequest("Số nhận dạng tồn tại");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == request.phoneNumber);
                if (request.phoneNumber != null)
                {
                    if (phoneExist.Any() && request.phoneNumber.Length != 0)
                        return BadRequest("Số điện thoại tồn tại");
                }

                var emailExist = _context.Users.Where(u => u.email == request.email);
                if (emailExist.Any())
                    return BadRequest("Email tồn tại");
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
        public async Task<ActionResult<Users>> UpdateUser(int id, EditUserDto user_dto)
        {
            var mess = "";
            try
            {
                var user = await _context.Users.FindAsync(id);
                var editor = await _context.Users.Where(u => u.id == user_dto.userModified).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("Số nhận dạng tồn tại");
                }
                //check is exist
                var identityExist = _context.Users.Where(u => u.identitycard == user_dto.identitycard && u.id != id);
                if (identityExist.Any())
                    return BadRequest("CMND đã tồn tại");
                var phoneExist = _context.Users.Where(u => u.phoneNumber == user_dto.phoneNumber && u.id != id && u.phoneNumber != null);
                if (phoneExist.Any())
                    return BadRequest("Số điện thoại đã tồn tại");
                var emailExist = _context.Users.Where(u => u.email == user_dto.email && u.id != id);
                if (emailExist.Any())
                    return BadRequest("Email tồn tại");

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
                        Message = mess == "" ? "Cập nhật thành công!" : mess, // if mess is empty => return Update success
                    });
                }
                return BadRequest("Không lấy được thông tin của editor !");
            }
            catch
            {
                return BadRequest("Lỗi không rõ !");
            }
        }
        [HttpPut("changePassword/{userCode}")]
        public async Task<IActionResult> ChangePassword(string userCode, [FromBody] ChangePassDto resource)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FirstOrDefaultAsync(d => d.userCode.Equals(userCode));
            if (users == null)
            {
                return NotFound("Người dùng không tồn tại !");
            }
            if (users.workStatus != 1)
            {
                return BadRequest("Tài khoản của bạn đã bị khóa !");
            }
            if (resource.newPassword.Equals(resource.oldPassword))
            {
                return BadRequest("Mật khẩu mới không thể giống mật khẩu cũ!");
            }
            if (BCrypt.Net.BCrypt.Verify(resource.oldPassword, users.userPassword))
            {
                users.userPassword = BCrypt.Net.BCrypt.HashPassword(resource.newPassword);
                await _context.SaveChangesAsync();
                return Ok("Đổi mật khẩu thành công!");
            }
            return BadRequest("Mật khẩu hiện tại không chính xác!");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.userCode.ToLower() == loginDto.UserName.ToLower());
            if (user == null || user.isDeleted == 1)
            {
                return NotFound("Tài khoản không tồn tại !");
            }
            if (user.workStatus != 1)
            {
                return BadRequest("Người dùng đã bị khóa!");
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
                        Message = "Xác thực thành công...",
                        Username = loginDto.UserName,
                        Token = accessToken,
                    });
                }
                return BadRequest("Sai mật khẩu !");
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
                return NotFound("Tài khoản không tồn tại !");
            }
            if (user.workStatus != 1)
            {
                return BadRequest("Người dùng đã bị khóa!");
            }
            Guid id = Guid.NewGuid();
            string newPass = id.ToString().Substring(0, 8);
            user.userPassword = BCrypt.Net.BCrypt.HashPassword(newPass);
            var maildto = new MailDto();
            maildto.To = user.email;
            maildto.Subject = "Đặt lại mật khẩu";
            maildto.Body = "Đây là mật khẩu mới: " + newPass;
            if (await _mailService.SendMail(maildto))
            {
                _context.SaveChanges();
                return Ok(new ApiResponseDto
                {
                    Success = true,
                    Message = "Vui lòng kiểm tra email của bạn !"
                });
            }
            else
            {
                return Ok(new ApiResponseDto
                {
                    Success = false,
                    Message = "Không thể đặt lại mật khẩu!"
                });
            }
        }
        [HttpGet("Pagination")]
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
                return BadRequest("Người dùng không tồn tại...");
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

