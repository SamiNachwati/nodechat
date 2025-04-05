using Microsoft.EntityFrameworkCore;
using Assignment4ChatApplication.Models;
using System;
namespace Assignment4ChatApplication

// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.
{
    public class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChatConnection")));
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);          
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);      
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapHub<ChatArea>("/chatarea");

            app.Run();
        }
    }
}