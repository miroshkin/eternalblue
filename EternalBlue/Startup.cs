using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EternalBlue.Ifs;
using EternalBlue.Mapping;
using EternalBlue.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitmentService;

namespace EternalBlue
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
            services.AddDbContext<IFSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(nameof(IFSContext))));

            services.AddSingleton<IRecruitmentService, RecruitmentServiceClient>();

            services.Configure<IFSApiConfiguration>(Configuration.GetSection(nameof(IFSApiConfiguration)))
                .AddHttpClient<IIfsDataProvider, IfsDataProvider>((serviceProvider, clientCfg) =>
                {
                    var cfg = serviceProvider.GetRequiredService<IOptions<IFSApiConfiguration>>().Value;
                    clientCfg.BaseAddress = new Uri(cfg.ApiAddress);
                });

            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfile(new IFSMappingProfile());
            }).CreateMapper());

            services.AddSingleton<IEncryptor, Encryptor>();
            
            services.AddControllersWithViews();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
