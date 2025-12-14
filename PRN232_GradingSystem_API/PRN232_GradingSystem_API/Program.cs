using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Services.Services.Implementations;
using PRN232_GradingSystem_Services.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using PRN232_GradingSystem_Services.SignalR;

using PRN232_GradingSystem_Repositories.Repositories.Implementations;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using OfficeOpenXml;
var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialPersonal("GROUP4");

// Cấu hình để chạy trên Docker/Render(Note: khi nao chay docker thi mo len)
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(8080); // Render yêu cầu chạy ở port 8080
//    serverOptions.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024; // 2 GB
//});

//config chay local
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // serverOptions.ListenAnyIP(8080); // Comment dòng này lại khi chạy Local
    serverOptions.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024; // 2 GB
});


// Add services to the container.

builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Count().Expand().SetMaxTop(100);
    });
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PRN232_GradingSystem_API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Allow large multipart uploads (Swagger shows "Failed to fetch" on 413)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024L * 1024 * 1024; // 1 GB
});
// Add Memory Cache
builder.Services.AddMemoryCache();
// Add DbContext
builder.Services.AddDbContext<PRN232_GradingSystem_APIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add SignalR
builder.Services.AddSignalR();
// Unit of Work / Repositories
builder.Services.AddScoped<PRN232_GradingSystem_Repositories.UnitOfWork.IUnitOfWork, PRN232_GradingSystem_Repositories.UnitOfWork.UnitOfWork>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://gradingsystem-two.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile(new PRN232_GradingSystem_Services.Helpers.ServiceMappingProfile());
    cfg.AddProfile(new PRN232_GradingSystem_API.Mapping.MapperProfile());
});

// Services

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IGradedetailService, GradedetailService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupStudentService, GroupStudentService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISemesterSubjectService, SemesterSubjectService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IFileStorageService, BackblazeStorageService>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExamExportService, ExamExportService>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();

builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ICriteriaService, CriteriaService>();

// JWT & Auth
builder.Services.AddSingleton<PRN232.Lab2.CoffeeStore.Services.Helpers.JwtHelper>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles(); // Enable static files for test HTML
app.UseCors("AllowFrontend");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Bật OData route (vẫn dùng song song với route cũ)
    endpoints.MapControllers();
    // Map SignalR Hub
    endpoints.MapHub<NotificationHub>("/notificationHub");
});
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
