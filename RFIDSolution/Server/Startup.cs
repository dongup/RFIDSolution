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
using RFIDSolution.Server.SignalRHubs;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.Models.Shared;
using RFIDSolution.Shared.Utils;
using RFIDSolution.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiyoshaEPE.WebApi.Hubs;

namespace RFIDSolution.Server
{
    public class Startup
    {
        /// <summary>
        /// Giầy gần nhất đã đi qua reader
        /// </summary>
        public ProductEntity lastProduct = new ProductEntity();
        private static int port = 1;

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
            services.AddSignalR();
            var strConn = Configuration.GetConnectionString("default");
            //var strConn = Configuration.GetConnectionString("iot");
            //System.Console.WriteLine(strConn);
            AppDbContext.ConnStr = strConn;
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Annora RFID Open API Document", Version = "v1" });
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

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            port = Configuration.GetSection("RFReaderConfig:RedGPOPort").Get<int>();
            ReaderHepler readerHepler = new ReaderHepler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            _ = Task.Run(() =>
            {
                //Kết nối reader ngay khi mở api
                System.Console.WriteLine("Connecting reader...");
                AppDbContext context = new AppDbContext();
                //System.Console.WriteLine("connstr: " + AppDbContext.ConnStr);
                Program.Reader = new ReaderHepler(context);
                Program.Reader.Connect();
                int redPort = Configuration.GetSection("RFReaderConfig").GetValue<int>("RedGPOPort");
                var checkPointAntenas = context.ANTENNAS.Where(x => x.LOCATION == Shared.Enums.AppEnums.AntennaLocation.CheckPoint).Select(x => x.ANTENNA_ID);

                //Hàm handle sự kiện khi có tag không hợp lệ đi qua cổng
                Program.Reader.OnTagRead += (tag) =>
                {
                    RFTagResponse rFTag = (RFTagResponse)tag;
                    //Lấy ra những anten được config cho check point
                    if (checkPointAntenas.Contains(rFTag.AntennaID))
                    {
                        //System.Console.WriteLine($"[{DateTime.Now.ToVNString()}] EPC: {rFTag.EPCID} pass though antenna {rFTag.AntennaID}");
                        var antenna = context.ANTENNAS.FirstOrDefault(x => x.ANTENNA_ID == rFTag.AntennaID);

                        //Check tag invalid và thêm vào database
                        var shoe = context.PRODUCT.FirstOrDefault(x => x.EPC == rFTag.EPCID);
                        if (shoe != null)
                        {
                            if (shoe.PRODUCT_STATUS != Shared.Enums.AppEnums.ProductStatus.Transfered)
                            {
                                //Lấy ra 1 dòng alert mới nhất có EPC scan thấy
                                var savedItem = context.PRODUCT_ALERT
                                .OrderByDescending(x => x.CREATED_DATE)
                                .Where(x => x.EPC == shoe.EPC)
                                .FirstOrDefault();

                                if (savedItem == null)
                                {
                                    Program.Reader.OpenGPOPort(port);

                                    System.Console.WriteLine($"------Invalid item: {shoe.EPC} pass though antenna name {antenna.ANTENNA_NAME}-----");
                                    savedItem = new ProductAlertEntity();
                                    savedItem.EPC = shoe.EPC;
                                    savedItem.ALERT_TIME = DateTime.Now;
                                    savedItem.PRODUCT_ID = shoe.PRODUCT_ID;
                                    savedItem.CREATED_DATE = DateTime.Now;

                                    context.PRODUCT_ALERT.Add(savedItem);
                                    try
                                    {
                                        context.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.InnerException?.Message);
                                        Console.WriteLine(ex.InnerException?.StackTrace);
                                    }
                                }
                                else
                                {
                                    //Không cảnh báo nếu vừa cảnh báo item này 10p trước
                                    if ((DateTime.Now - savedItem.CREATED_DATE).TotalMinutes > 10)
                                    {
                                        Program.Reader.OpenGPOPort(port);

                                        System.Console.WriteLine($"------Invalid 2 item: {shoe.EPC} pass though antenna name {antenna.ANTENNA_NAME}-----");
                                        var newItem = new ProductAlertEntity();
                                        newItem.EPC = shoe.EPC;
                                        newItem.ALERT_TIME = DateTime.Now;
                                        newItem.PRODUCT_ID = shoe.PRODUCT_ID;
                                        newItem.CREATED_DATE = DateTime.Now;

                                        context.PRODUCT_ALERT.Add(newItem);
                                        try
                                        {
                                            context.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.InnerException?.Message);
                                            Console.WriteLine(ex.InnerException?.StackTrace);
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
                if (Program.Reader.ReaderStatus.IsConnected)
                {
                    Program.Reader.StartInventory();
                }
            });

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Annora RFID Open API Document v1");
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
                endpoints.MapFallbackToFile("index.html");

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ReaderHub>("/readerhub");
                endpoints.MapHub<ReaderStatusHub>("/ReaderStatusHub");
            });
        }
    }
}
