using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PostalCode.API.Common;
using PostalCode.API.Service;
using PostalCode.API.Service.Common;
using PostalCode.API.Service.Interfaces;
using PostalCode.API.Service.Mapper;

namespace PostalCode.API
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PostCodesLookup", Version = "v1" });
            });
            #region Allow-Orgin
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });
            #endregion
            
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddLogging();
            
            services.AddSingleton<ILogger>(provider =>
            provider.GetService<ILoggerFactory>().CreateLogger("BaseController"));
            services.AddScoped<SearchResultMapper>();
            services.AddScoped<PostCodeAPIConfig>();
            services.AddScoped<IHttpClientRepository, HttpClientRepository>();
            services.AddScoped<IPostCodesLookupService, PostCodesLookupService>();
            services.AddScoped<CustomException>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseMiddleware<BaseController>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string path = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{path}/swagger/v1/swagger.json", "PostCodesLookup V1"); 
                //c.RoutePrefix = string.Empty;
            });
            
        }
    }
}
