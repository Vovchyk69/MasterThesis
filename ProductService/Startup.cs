using Microsoft.AspNetCore.Mvc;
using Shared.AmazonS3;
using Steeltoe.Discovery.Client;

namespace ProductService
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
            var appSettings = Configuration.GetSection("AppSettings");
            var bucketName = appSettings["bucketName"] ?? string.Empty;
            var region = appSettings["region"] ?? string.Empty;
            var awsAccessKeyId = appSettings["awsAccessKeyId"] ?? string.Empty;
            var awsSecretAccessKey = appSettings["awsSecretAccessKey"] ?? string.Empty;

            services
                .AddSingleton<IAwsConfiguration>(
                    new AwsConfiguration(awsAccessKeyId, awsSecretAccessKey, bucketName, region))
                .AddControllers();
            
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
            services.AddDiscoveryClient(Configuration);
            services.AddMvc()
                .AddNewtonsoftJson(JsonOptions)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
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
        
        private void JsonOptions(MvcNewtonsoftJsonOptions options)
        {
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        }
    }
}
