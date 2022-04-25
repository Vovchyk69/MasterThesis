using Microsoft.AspNetCore.Mvc;
using SchedulerService.DataParsing;
using SchedulerService.GeneticAlgorithm;
using Shared.AmazonS3;

namespace SchedulerService;

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
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("http://localhost:7015")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddScoped(typeof(IChromosome<>), typeof(GeneticAlgorithm<>));
        services.AddScoped<IFile, JsonFile>();
        services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        
        var appSettings = Configuration.GetSection("AppSettings");
        var bucketName = appSettings["bucketName"] ?? string.Empty;
        var region = appSettings["region"] ?? string.Empty;
        var awsAccessKeyId = appSettings["awsAccessKeyId"] ?? string.Empty;
        var awsSecretAccessKey = appSettings["awsSecretAccessKey"] ?? string.Empty;

        services
            .AddSingleton<IAwsConfiguration>(
                new AwsConfiguration(awsAccessKeyId, awsSecretAccessKey, bucketName, region))
            .AddControllers();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
            
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }
            
        app.UseCors(builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
            
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}