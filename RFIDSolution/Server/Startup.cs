using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RFIDSolution.Middlewares;
using RFIDSolution.Server.Service;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDSolution.Server
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
            services.AddGrpc();
            var strConn = Configuration.GetConnectionString("default");
            System.Console.WriteLine(strConn);
            services.AddDbContext<AppDbContext>(sp => sp.UseSqlServer(strConn));

            services.AddIdentity<UserEntity, RoleEntity>(op =>
            {
                op.Password.RequireDigit = false;
                op.Password.RequiredLength = 6;
                op.Password.RequireLowercase = false;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequiredUniqueChars = 0;
                op.Password.RequireUppercase = false;
            })
            .AddRoles<RoleEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            //Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

            services.AddControllersWithViews()
            .AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.IgnoreNullValues = false;
                option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                option.JsonSerializerOptions.WriteIndented = false;
                option.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory =  // the interjection
                    ModelStateValidator.ValidateModelState;
            });

            //string xmlDocPath = string.Format(@"{0}\RFIDSolution.Server.xml",
            //         System.AppDomain.CurrentDomain.BaseDirectory);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Anora RFID Open API Document", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                //c.IncludeXmlComments(xmlDocPath);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin",
                    builder =>
                    {
                        builder.WithExposedHeaders("grpc-status", "grpc-message");
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowAnyOrigin();
                    });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin",
                    builder =>
                    {
                        builder.WithExposedHeaders("grpc-status", "grpc-message");

                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowAnyOrigin();
                    });
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            app.UseDeveloperExceptionPage();

            app.UseExceptionHandler("/api/error/handler/500");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaiyoshaEPE Open API Document v1");
            });

            app.UseStaticFiles();

            app.UseMiddleware<CustomMiddleware>();

            app.UseRouting();

            app.UseCors("AllowAllOrigin");

            // must be added after UseRouting and before UseEndpoints 
            app.UseGrpcWeb();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ShoeModelService>().EnableGrpcWeb();
                endpoints.MapGrpcService<RFIDReadService>().EnableGrpcWeb();
                //endpoints.MapGrpcService<ProductService>().EnableGrpcWeb();

                endpoints.MapFallbackToFile("index.html");

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
