using Microsoft.OpenApi.Models;


namespace AdminGateway.Services
{
    public class SwaggerOptions
    {
        public bool Enabled { get; set; }
        public string JsonRoutePrefix { get; set; }
        public string ApiBasePath { get; set; }
        public string ApiBaseScheme { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public static SwaggerOptions Default = new SwaggerOptions
        {
            Enabled = false,
            JsonRoutePrefix = "",
            ApiBasePath = "/",
            ApiBaseScheme = "http",
            Title = "",
            Description = ""
        };
    }
    public static class Swagger
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
            IConfiguration configuration)
        {
            SwaggerOptions swaggerOptions =
                configuration.GetSection("Swagger").Get<SwaggerOptions>() ?? SwaggerOptions.Default;
            if (!swaggerOptions.Enabled)
            {
                return services;
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Admin Gateway", });
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "Enter 'Bearer' [space] and then your token in the text input below.Example: 'Bearer 12345abcdef'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"},
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            SwaggerOptions swaggerOptions =
                configuration.GetSection("Swagger").Get<SwaggerOptions>() ?? SwaggerOptions.Default;

            if (!swaggerOptions.Enabled)
            {
                return app;
            }

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new()
                        {
                            Url =
                                $"{swaggerOptions.ApiBaseScheme}://{httpReq.Host.Value}{swaggerOptions.ApiBasePath}"
                        }
                    };

                    var paths = swaggerDoc.Paths.ToDictionary(item =>
                        FirstCharacterToLower(item.Key), item => item.Value);

                    swaggerDoc.Paths.Clear();
                    foreach ((string key, OpenApiPathItem value) in paths)
                    {
                        swaggerDoc.Paths.Add(key, value);
                    }
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{swaggerOptions.JsonRoutePrefix}/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = "swagger";
            });

            return app;
        }

        private static string FirstCharacterToLower(string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
                return str;

            string endpoint = string.Empty;
            var parts = str.Split('/').ToList();

            foreach (string part in parts.Where(part => !string.IsNullOrEmpty(part)))
            {
                endpoint += "/";
                endpoint += char.ToLowerInvariant(part[0]) + part[1..];
            }

            return endpoint;
        }
    }
}
