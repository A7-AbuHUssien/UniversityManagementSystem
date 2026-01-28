using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UniversityManagementSystem.Api.Middlewares;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;
using UniversityManagementSystem.Application.Services;
using UniversityManagementSystem.Application.Validators;
using UniversityManagementSystem.Infrastructure.DataAccess;
using UniversityManagementSystem.Infrastructure.Repositories;
using UniversityManagementSystem.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IStudentService), typeof(StudentService));    
builder.Services.AddScoped(typeof(IUnitOfWork),typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IStudentBusinessValidation), typeof(StudentBusinessValidation));
builder.Services.AddScoped<IDepartmentBusinessValidation, DepartmentBusinessValidation>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IInstructorBusinessValidation, InstructorBusinessValidation>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<ICourseBusinessValidation, CourseBusinessValidation>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentBusinessValidation, EnrollmentBusinessValidation>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IStudentScheduleService, StudentScheduleService>();
builder.Services.AddScoped<IStudentProgressService, StudentProgressService>();
builder.Services.AddScoped<IGradingService, GradingService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IDisplayEnrollmentService, DisplayEnrollmentService>();
// 1. Enable Automatic Validation
builder.Services.AddFluentValidationAutoValidation();

// 2. Register all validators from the assembly where StudentValidator is located
builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CourseValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<InstructorValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SemesterValidator>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => 
    {
        options
            .WithTitle("University Management System API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();