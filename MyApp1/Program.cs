using MyApp1.Configuration;
using MyApp1.Services;
using Services.Extensions;

namespace MyApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services for application layers (e.g., business logic, data access, etc.)
            builder.Services.AddServicesLayers(builder.Configuration);

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Register MyApp2Client
            builder.Services.AddSingleton<MyApp2Client>();

            // Configure settings for MyApp2
            builder.Services.Configure<MyApp2Options>(builder.Configuration.GetSection("MyApp2"));


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
