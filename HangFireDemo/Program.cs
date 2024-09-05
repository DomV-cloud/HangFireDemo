
using Hangfire;

namespace HangFireDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddHangfire(
                (sp, config) =>
                {
                    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("sqlConnection");
                    config.UseSqlServerStorage(connectionString);
                });

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.MapControllers();

            app.Run();
        }
    }
}
