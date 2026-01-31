using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using UniversityManagementSystem.Api.Filters;
using UniversityManagementSystem.Api.Middlewares;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;
using UniversityManagementSystem.Application.LogicConstraints;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;
using UniversityManagementSystem.Application.Services;
using UniversityManagementSystem.Application.Services.Identity;
using UniversityManagementSystem.Application.Validators;
using UniversityManagementSystem.Infrastructure.DataAccess;
using UniversityManagementSystem.Infrastructure.Repositories;
using UniversityManagementSystem.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Database Configuration ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. Identity Configuration ---
builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// --- 4. Service Discovery & DI ---
builder.Services.AddControllers(options => { options.Filters.Add<ApiResponseFilter>(); });
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Business Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentBusinessValidation, StudentBusinessValidation>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentBusinessValidation, EnrollmentBusinessValidation>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IStudentScheduleService, StudentScheduleService>();
builder.Services.AddScoped<IStudentProgressService, StudentProgressService>();
builder.Services.AddScoped<IGradingService, GradingService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IDisplayEnrollmentService, DisplayEnrollmentService>();
builder.Services.AddScoped<IRegistrationControlService, RegistrationControlService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(UniversityManagementSystem.Application.Interfaces.Services.IEmailService),
    typeof(UniversityManagementSystem.Application.Services.EmailService));
builder.Services.AddScoped<IUserService, UserService>();

// --- 5. Validation ---
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>();

// --- 6. API Documentation ---
builder.Services.AddOpenApi();

var app = builder.Build();

// --- 7. Data Seeding ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser<Guid>>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await DbInitializer.SeedAsync(context, userManager, roleManager);
}

// --- 8. Middleware Pipeline ---
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("University Management System API")
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 👈 Who are you? (Checks JWT)
app.UseAuthorization();  // 👈 Are you allowed? (Checks Roles)

app.MapControllers();

app.Run();