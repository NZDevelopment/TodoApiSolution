using Serilog;
using TodoApi.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Services;
using TodoApi.Repositories;
using TodoApi.Middleware;

namespace TodoApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Host.UseSerilog((context, services, configuration) =>
                configuration.WriteTo.Console().WriteTo.File("Logs/todoapi.log", rollingInterval: RollingInterval.Day));

            builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Todo repository
            builder.Services.AddScoped<ITodoRepository, TodoRepository>();
            builder.Services.AddScoped<TodoService>();
            builder.Services.AddScoped<WeatherService>();

            // Register the TodoService and its HttpClient
            builder.Services.AddHttpClient<TodoService>();



            // Add WeatherService HttpClient
            builder.Services.AddHttpClient<WeatherService>();

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<TodoDbContext>();
                    SeedData.Initialize(services); // Pass the IServiceProvider for initialization

                    // Fetch and store the To-Do list
                    var todoService = services.GetRequiredService<TodoService>();
                    await todoService.FetchAndStoreTodosAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ErrorHandlingMiddleware>();


            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
