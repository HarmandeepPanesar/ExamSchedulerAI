using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ExamsSchedule.Areas.Identity;
using ExamsSchedule.Data;
using ExamsSchedule.Services;
using Blazored.Toast;
using Radzen;
using ExamsSchedule.Models.DB;
using BlazorStrap;
using System.Net.Http;

namespace ExamsSchedule
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<DeepsensorClientContext>();

            //services.AddDbContext<MyContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            services.AddDbContext<DeepsensorClientContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password = new PasswordOptions
                {
                    RequiredLength = 6,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false,
                    RequireDigit = false
                };
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); // Ensure you add this for tokens like password reset

           
            // Add Bootstrap CSS  
            //services.AddBootstrapCss();
            //services.AddD<IdentityUser>(options =>
            //{
            //    options.Password = new PasswordOptions
            //    {
            //        RequiredLength = 6,
            //        RequireLowercase = false,
            //        RequireUppercase = false,
            //        RequireNonAlphanumeric = false,
            //        RequireDigit = false
            //    };
            //    options.SignIn.RequireConfirmedAccount = false;
            //})
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            //services.AddBootstrapCss();
            services.AddBlazoredToast();
            services.AddProtectedBrowserStorage();
            services.AddScoped<SpinnerService>();
            services.AddScoped<DialogService>();
            services.AddScoped<ExamService>();
            services.AddScoped<StudentService>();
            services.AddScoped<ExamStudentService>();
            services.AddScoped<LiveViewService>();
            services.AddScoped<ExamStatusService>();
            services.AddScoped<BlackListService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<VerifyExamService>();
            services.AddScoped<TeacherService>();
            services.AddScoped<SearchStudentService>();
            services.AddScoped<StudentFacultyService>();
            services.AddScoped<LiveProctorService>();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddScoped<WeatherForecastService>();
            services.AddHttpContextAccessor();
            if (services.All(x => x.ServiceType != typeof(HttpClient)))
            {
                services.AddScoped(
                    s =>
                    {
                        var navigationManager = s.GetRequiredService<NavigationManager>();
                        return new HttpClient
                        {
                            BaseAddress = new Uri(navigationManager.BaseUri)
                        };
                    });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseDefaultFiles();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
