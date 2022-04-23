using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;

namespace OcelotGateway;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("ApiSecurity", x =>
            {
                var secret = Configuration.GetSection("AppSettings")["secret"];
                var key = Encoding.ASCII.GetBytes(secret);
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        
            services.AddOcelot().AddEureka().AddCacheManager(x => x.WithDictionaryHandle());
    }

    public void Configure(IApplicationBuilder builder, IWebHostEnvironment env)
    {
        var appSettings = new AppSettings();
        builder.ApplicationServices.GetService<IConfiguration>()!
            .GetSection("AppSettings")
            .Bind(appSettings);

        builder.UseCors
        (b => b
            .WithOrigins(appSettings.AllowedChatOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
        );
        builder.UseOcelot().Wait(); 
    }
}