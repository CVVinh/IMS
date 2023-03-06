
using BE.Data.Contexts;
using BE.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BE.Helpers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using BE.Services.TokenServices;
using FluentValidation.AspNetCore;
using FluentValidation;
using BE.Data.Dtos.UsersDTO;
using BE.Data.Dtos.UsersDTO.Validator;
using BE.Services.TaskServices;
using BE.Services.PaginationServices;
using BE.Data.Dtos.TaskDto;
using BE.Data.Dtos.TaskDtos.Validator;
using BE.Data.Dtos.TaskDtos;
using BE.Services.MemberProjectServices;
using BE.Data.Dtos.MemberProjectDtos.Validator;
using BE.Data.Dtos.MemberProjectDtos;
using BE.Services.MailServices;
using BE.Services.LeaveOffServices;
using BE.Data.Models;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Dtos.LeaveOffDtos.Validator;
using BE.Services.Wiki;
using BE.Services.WikiPost;
using BE.Data.Dtos.WikiDtos;
using BE.Data.Dtos.WikiCateDtos;
using BE.Data.Dtos.WikiPost;
using BE.Data.Dtos.WikiCateDtos.Validator;
using BE.Data.Dtos.WikiPost.Validator;
using API_LVTN.DAL;
using BE.Mapper;
using BE.Services.PaidServices;
using BE.shared.Interface;
using BE.Services.RulesServices;
using BE.Services.InfoDeviceServices;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.PaidDtos.Validator;
using BE.Services.Customers;
using BE.Hubs;
using BE.Services.UserServices;

var builder = WebApplication.CreateBuilder(args);


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//Add services to the container.

builder.Services.AddControllers()
    .AddFluentValidation();
builder.Services.AddSignalR();
#region Registering AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion


#region Registering Services
builder.Services.AddScoped<ILeaveOffServices, LeaveOffServices>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPaidReasonsService, PaidReasonServices>();
builder.Services.AddScoped<ITasksServices, TasksServices>();
builder.Services.AddScoped<IWikiCategogyService, WikiCategogyServices>();
builder.Services.AddScoped<IWikiPostService, WikiPostService>();
builder.Services.AddScoped<IUserGroupServices, UserGroupServices>();

builder.Services.AddScoped<IMemberProjectServices, MemberProjectServices>();
builder.Services.AddScoped<IPaginationServices<GetAllTaskDto>, PaginationServices<GetAllTaskDto>>();
builder.Services.AddScoped<IPaginationServices<LeaveOff>, PaginationServices<LeaveOff>>();
builder.Services.AddScoped<IPaginationServices<PaidReasons>, PaginationServices<PaidReasons>>();
builder.Services.AddScoped<IPaginationServices<Customer>, PaginationServices<Customer>>();
builder.Services.AddScoped<CommonHelperDtos>();
builder.Services.AddScoped<IPaginationServices<Users>, PaginationServices<Users>>();
builder.Services.AddScoped<IPaginationServices<Projects>, PaginationServices<Projects>>();
builder.Services.AddScoped<IPaginationServices<Devices>, PaginationServices<Devices>>();
builder.Services.AddScoped<IPaidServices, PaidService>();
builder.Services.AddScoped<IPaginationServices<Paid>, PaginationServices<Paid>>();
builder.Services.AddScoped<IPaginationServices<PaidImage>, PaginationServices<PaidImage>>();
builder.Services.AddScoped<IRulesService, RulesService>();
builder.Services.AddScoped<IPaginationServices<Rules>, PaginationServices<Rules>>();
builder.Services.AddScoped<IPaginationServices<User_Group>, PaginationServices<User_Group>>();
builder.Services.AddScoped<IInfoDeviceService,InfoDeviceService>();
#endregion

#region Registering Validator
builder.Services.AddTransient<IValidator<AddUserDto>, AddUserDtoValidator>();
builder.Services.AddTransient<IValidator<AddNewTaskDto>, AddNewTaskDtoValidator>();


builder.Services.AddTransient<IValidator<AddMemberDto>, AddMemberDtoValidator>();
builder.Services.AddTransient<IValidator<addWiki_Categogy>, addWikiCateValidator>();
builder.Services.AddTransient<IValidator<editWikiCate>, editWikiCateValidator>();
builder.Services.AddTransient<IValidator<addWikiPost>, addWikiPostValidator>();
builder.Services.AddTransient<IValidator<editWikiPost>, editWikiPostValidator>();
builder.Services.AddTransient<IValidator<EditTaskByIdDto>, EditTaskByIdDtoValidator>();
builder.Services.AddTransient<IValidator<AddMemberDto>, AddMemberDtoValidator>();
builder.Services.AddTransient<IValidator<AddNewLeaveOffDto>, AddNewLeaveOffDtoValidator>();
builder.Services.AddTransient<IValidator<AccepterLeaveOffDto>, AccepterLeaveOffDtoValidator>();
builder.Services.AddTransient<IValidator<EditRegisterLeaveOffDtos>, EditRegisterLeaveOffDtoValidator>();
builder.Services.AddTransient<IValidator<AcceptPaymentPaidDtos>, AcceptPaymentPaidValidator>();
#endregion

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration["DbContextSetting:ConnectionString"], b => b.MigrationsAssembly("BE"));
});

// Configure Cors
builder.Services.AddCors(otps =>
{
    otps.AddPolicy("AppCorsPolicy", policy => 
    {
        //policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        policy.WithOrigins("http://localhost:3000/").SetIsOriginAllowed((host) => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAutoMapperCollection();
// Register instance
/*Configure*/
builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSetting"));
/*Add instance */
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<EncryptionHelper>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<TokenHelper>();


// add repository
builder.Services.AddScoped(typeof(GenericRepository<>));

// Configure Authentication
var secretKey = builder.Configuration["JwtSetting:Secret"];
var secretKeyByte = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Get Jwt Setting
    var jwtSetting = builder.Configuration.GetSection("JwtSetting").Get<JwtSetting>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // default True
        ValidateAudience = false, // default True

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret)),

        ClockSkew = TimeSpan.Zero
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseCors("AppCorsPolicy");

app.UseStaticFiles();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationHub>("/NotificationHub");

app.MapControllers();


app.Run();

