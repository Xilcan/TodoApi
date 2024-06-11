//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Data;
using TodoApi.Interfaces;
using TodoApi.Repositories;
using TodoApi.Services;

namespace TodoApi
{
    /// <summary>
    /// The entry point class for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure the database context.
            builder.Services.AddDbContext<TodoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services.
            builder.Services.AddScoped<ITodoRepository, TodoRepository>();
            builder.Services.AddScoped<ITodoService, TodoService>();

            // Add Swagger services.
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Todo API",
                    Description = "A simple example ASP.NET Core Web API",
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
                    c.RoutePrefix = "swagger"; // Set Swagger UI at the root
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
