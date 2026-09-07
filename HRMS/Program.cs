using HRMS.Data;
using HRMS.Repositories.Implementations;
using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using AutoMapper;

namespace HRMS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            

            builder.Services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HRMSContext>();
            var connectionString =
              builder.Configuration.GetConnectionString("HRMSConnection")
              ?? throw new InvalidOperationException(
                  "Connection string 'HRMSContext' not found.");

            builder.Services.AddDbContext<HRMSContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
            builder.Services.AddScoped<IPerformanceReviewRepository,PerformanceReviewRepository>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            

            var app = builder.Build();

            // Seed Roles and Admin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await DbSeed.SeedRolesAndAdminAsync(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();

            app.Run();
        }
    }
}