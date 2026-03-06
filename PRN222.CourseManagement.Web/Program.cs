using Microsoft.EntityFrameworkCore;
using PRN222.CourseManagement.Repository.IRepository.ICourseRepository;
using PRN222.CourseManagement.Repository.IRepository.IDepartmentRepository;
using PRN222.CourseManagement.Repository.IRepository.IEnrollmentRepository;
using PRN222.CourseManagement.Repository.IRepository.StudentRepository;
using PRN222.CourseManagement.Repository.IUnitOfWork;
using PRN222.CourseManagement.Repository.Models;
using PRN222.CourseManagement.Repository.Repository.CourseRepositroy;
using PRN222.CourseManagement.Repository.Repository.DepartmentRepository;
using PRN222.CourseManagement.Repository.Repository.EnrollmentRepository;
using PRN222.CourseManagement.Repository.Repository.StudentRepositroy;
using PRN222.CourseManagement.Repository.UnitOfWork;
using PRN222.CourseManagement.Service.IService;
using PRN222.CourseManagement.Service.Service;

var builder = WebApplication.CreateBuilder(args);


// DI DB
builder.Services.AddDbContext<CourseManagementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//DI Repo
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollementRepository, EnrollmentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

//DI Service

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}");

app.Run();
