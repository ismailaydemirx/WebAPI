using ETicaretProject.API.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretProject.API
{
    public class Startup
    {
        // Dependency Injection : Bağımlılık enjeksiyonu
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // servis olarak databasecontext'i eklediğimiz için artık dbxontex'tin new'lenmesinden biz sorumlu değiliz. Bana ctor'dan bir tane Databasecontext ver diyeceğiz ve ta damm.
            services.AddDbContext<DatabaseContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(Configuration.GetConnectionString("ETicaretAppDBConnectionString"));
                //optionsAction.UseLazyLoadingProxies();
            });

            // Bu ayar modelstate otomatik valid kontrolünü devre dışı bırakır.
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            
            // JWT Token ile Token Oluşturuyoruz.
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer= true,
                        ValidateAudience= true,
                        ValidateLifetime= true,
                        ValidateIssuerSigningKey= true,
                        ValidIssuer="site.com",
                        ValidAudience="site.com",
                        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtOptions:Key"])) // anahtar oluşturuyoruz.
                    };
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaretProject.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETicaretProject.API v1"));
            }

            app.UseRouting();

            // Authorization'dan önce Authenticaton mekanizmasını devreye aldık bu sayede kontrol gerçekleştirdikten sonra doğrulama yapacağız.
            app.UseAuthentication(); 

            app.UseAuthorization(); // Bu kısımda, eğer bir rol belirtilmişse ve bu role özgü yetkilendirme kuralları varsa, istemcilerin bu rolleri ve kuralları kontrol ederek erişim haklarını denetlemesini sağlarız.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
