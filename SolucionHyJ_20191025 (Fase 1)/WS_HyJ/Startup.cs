using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using WS_HyJ.Extensions;
using WS_HyJ.Helpers;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Identity;
using WS_HyJ.Models.Repository;
using WS_HyJ.Models.Repository.DAO;
using WS_HyJ.Seguridad;
using AutoMapper;
using WS_HyJ.Helpers.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WS_HyJ.Utils;
using Microsoft.Extensions.FileProviders;
using System.IO;
using WS_HyJ.Middleware;

namespace WS_HyJ
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
            services.AddSingleton(Configuration);

            services.Configure<SwaggerSettings>(Configuration.GetSection(nameof(SwaggerSettings)));

            //Inicializar el servicio de control de origenes
            services.AddCors();

            #region Control de versiones
            string _env = Configuration["env"];

            services.AddMvc(o => { o.UseGeneralRoutePrefix(_env + "/api/v{version:apiVersion}"); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddApiVersionWithExplorer()
                .AddSwaggerOptions()
                .AddSwaggerGen();
            #endregion

            #region Obtener valores del config para el token de seguridad
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            #endregion

            #region Obtener cadena de conexión del config
            var _getCMG = Configuration.GetSection("ConnectionStrings:MongoDbDatabase").Value;
            int idx = _getCMG.LastIndexOf('/');
            services.Configure<Settings>(_ => {
                _.ConnectionString = _getCMG.Substring(0, idx); //Antes del último '/'
                _.Database = _getCMG.Substring(idx + 1); //Despies del último '/'
            });
            #endregion

            #region Configure Identity MongoDB
            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
            {
                identityOptions.Password.RequiredLength = 6;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireDigit = false;
            }, mongoIdentityOptions =>
            {
                mongoIdentityOptions.ConnectionString = Configuration.GetConnectionString("MongoDbDatabase");
            });
            #endregion

            #region Add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    //Set default Authentication Schema as Bearer
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters =
                       new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                           ValidIssuer = token.Issuer,
                           ValidAudience = token.Audience,
                           ValidateIssuer = false,
                           ValidateAudience = false,
                           ClockSkew = TimeSpan.Zero // remove delay of token when expire
                       };
                });
            #endregion

            services.AddAutoMapper(typeof(Startup));

            //Injection
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IUserManagementService, UserManagmentService>();
            
            services.AddTransient<IProviderDAO, ProviderDAO>();
            services.AddTransient<IProductDAO, ProductDAO>();
            services.AddTransient<IBrandDAO, BrandDAO>();
            services.AddTransient<IKardexDAO, KardexDAO>();
            services.AddTransient<IOrderDAO, OrderDAO>();
            services.AddTransient<IReceiptDAO, ReceiptDAO>();
            services.AddTransient<IPaymentMethodDAO, PaymentMethodDAO>();

            services.AddTransient<IImageHandler, ImageHandler>();
            services.AddTransient<IImageWriter, ImageWriter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            IdentityModelEventSource.ShowPII = true;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwaggerDocuments();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                RequestPath = "/MyImages"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                RequestPath = "/MyImages"
            });

            // ===== global cors policy =====
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // ===== Use Authentication ======
            app.UseAuthentication();

            //Add our new middleware to the pipeline
            //app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseMvc();
        }
    }
}
