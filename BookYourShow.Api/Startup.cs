using System.Text;
using BookYourShow.Api.DIContainer;
using BookYourShow.Data.EFDatabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookYourShow.BusinessLogic.StartupMethods;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BookYourShow.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Initialize EfCore With sql server
            services.AddCors(options =>  
            {  
                options.AddPolicy(MyAllowSpecificOrigins,
                builder => {
                    builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });  
            });  

            services.AddDbContext<BYSDBContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("TestDatabase"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 5;
            }).AddEntityFrameworkStores<BYSDBContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(auth => {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["AuthSettings:Audience"],
                    ValidIssuer = Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });
            
            Startuphelper.InjectDependency(services);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //Serilog.Extensions.Logging.File
            //Add file is extension method from Serilog Package....
            string filePath = Configuration["LoggerSettings:LogFilePath"] + "log-{Date}.txt";
            loggerFactory.AddFile (filePath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles ();

            app.UseStaticFiles (new StaticFileOptions () {
                FileProvider = new PhysicalFileProvider (
                        Path.Combine (Configuration["FilePaths:Thumbnail"])),
                    RequestPath = new PathString ("/api")
            });

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
