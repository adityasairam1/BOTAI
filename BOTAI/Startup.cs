using BOTAI.Generated;
using Microsoft.EntityFrameworkCore;

namespace BOTAI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = InjectConfiguration(configuration);
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // ✅ Register health checks service
            services.AddHealthChecks();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // ✅ Swagger setup
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<BOTAIContext>(builder =>
                builder.UseSqlServer(
                    Configuration.GetConnectionString("BOTAIDB"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)
                )
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BOTAIContext dbContext)
        {
            if (dbContext.Database.IsRelational())
            {
                dbContext.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                // ✅ Swagger UI available only in Development by default
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            // ✅ Proper endpoint mapping instead of UseHealthChecks
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        private IConfiguration InjectConfiguration(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration);
            return builder.Build();
        }
    }
}
