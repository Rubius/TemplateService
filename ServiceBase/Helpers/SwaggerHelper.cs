using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ServiceBase
{
    internal class SwaggerHelper
    {
        // cм. Properties -> Build -> Output -> XML documentation file
        private const string DOCUMENTATION_FILE = "documentation.xml";

        private readonly string _serviceName;

        public SwaggerHelper(string serviceName)
        {
            _serviceName = serviceName;
        }

        public void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{_serviceName} API",
                    Version = "v1",
                    Description = $"{_serviceName} service"
                });

                var xmlPath = Path.Combine(AppContext.BaseDirectory, DOCUMENTATION_FILE);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void UseSwagger(IApplicationBuilder app)
        {
            app
               .UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{_serviceName} API");
               });
        }
    }
}
