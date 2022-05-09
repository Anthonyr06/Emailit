using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Data;
using Emailit.Hubs;
using Emailit.Models;
using Emailit.Services;
using Emailit.Services.Data;
using Emailit.Services.Policies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Emailit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            double CookieExpireTimeSpanHours = Configuration.GetValue<double>("CookieOptions:ExpireTimeSpan:Hours");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookieOptions =>
                {
                    cookieOptions.Cookie.Name = "UserCookie";
                    cookieOptions.LoginPath = "/Account/Login";
                    cookieOptions.ExpireTimeSpan = TimeSpan.FromHours(CookieExpireTimeSpanHours);
                    cookieOptions.SlidingExpiration = true; //Extends the cookie expiration time if it detects a lot of user activity
                    cookieOptions.EventsType = typeof(CustomCookieAuthenticationEvents);
                });

            services.AddScoped<CustomCookieAuthenticationEvents>();

            //services.AddMvc().AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.AddPageRoute("/Index", "");
            //});

            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

            services.AddScoped<IEmailSender, EmailSender>();

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddSingleton<IAuthorizationHandler, PermissionsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();

            services.AddRazorPages();

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddSignalR();

            services.AddDbContext<ApplicationDbContext>(c =>
            {
                c.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
              

                #warning DEACTIVATE IN PRODUCTION
                c.EnableSensitiveDataLogging(true);


            });

            services.AddHttpContextAccessor();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IClaimFactory, ClaimsPrincipalFactory>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IUserRepository, UserRepositoryEF>();
            services.AddScoped<IUserSessionRepository, UserSessionRepositoryEF>();
            services.AddScoped<IDepartmentRepository, DepartmentRepositoryEF>();
            services.AddScoped<IJobRepository, JobRepositoryEF>();
            services.AddScoped<IUserModificationRepository, UserModificationRepositoryEF>();
            services.AddScoped<IRoleRepository, RoleRepositoryEF>();
            services.AddScoped<IUserRoleRepository, UserRoleRepositoryEF>();
            services.AddScoped<IBranchOfficeRepository, BranchOfficeRepositoryEF>();
            services.AddScoped<IMessageRepository, MessageRepositoryEF>();
            services.AddScoped<IReceivedMessageRepository, ReceivedMessageRepositoryEF>();
            services.AddScoped<IReceivedMessageStateRepository, ReceivedMessageStateRepositoryEF>();
            services.AddScoped<IFileManagement, FileManagement>();
            services.AddScoped<IAttachedFileRepository, AttachedFileRepositoryEF>();
            services.AddScoped<IFileDataRepository, FileDataRepositoryEF>();
            services.AddSingleton<IHubConnectionManager, HubConnectionManager>();
            services.AddSingleton(typeof(IHubNotificationHelper<>), typeof(HubNotificationHelper<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            });

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //to identify the source IP address of a client connecting to a web server through an HTTP proxy or load balancer
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<NotificationHub>("/NotificationHub");
            });
        }
    }
}
