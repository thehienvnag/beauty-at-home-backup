using ApplicationCore.Services;
using AutoMapper;
using BeautyAtHome.Authorization;
using BeautyAtHome.ExternalService;
using BeautyAtHome.Utils;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure.Contexts;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace BeautyAtHome
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
            services.AddAutoMapper(typeof(Startup));

            var pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "firebase_admin_sdk.json");
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(pathToKey)
            });

            services.AddDbContext<BeautyServiceProviderContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BeautyServiceProviderDatabase")));

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            services.AddScoped(typeof(IPagingSupport<>), typeof(PagingSupport<>));

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IAuthorizationPolicyProvider, RequiredRolePolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, RequiredRoleHandler>();
            services.AddSingleton<IUploadFileService, UploadFileService>();
            services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();
            services.AddSingleton<IPushNotificationService, PushNotificationService>();

            //services.AddTransient<IServiceRepository, ServiceRepository>();
            //services.AddTransient<IAccountRepository, AccountRepository>();

            services.AddTransient<IBeautyServicesService, BeautyServicesService>();
            services.AddTransient<IAccountService, AccountService>();

            services.AddTransient<IFeedBackService, FeedBackService>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<IBookingDetailService, BookingDetailService>();
            
            services.AddTransient<IGalleryService, GalleryService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IServiceTypeService, ServiceTypeService>();
            services.AddTransient<IAddressService, AddressService>();


            

            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["jwt:Key"])),
                    ValidAudience = Configuration["jwt:Audience"],
                    ValidIssuer = Configuration["jwt:Issuer"],
                };
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BeautyAtHome", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Enter **_ONLY JWT Bearer token in the text box below.",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //For web run
            app.UseCors(
                options => options.WithOrigins("http://0.0.0.0:3000").AllowAnyMethod().AllowAnyHeader()
            );
    
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BeautyAtHome v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
