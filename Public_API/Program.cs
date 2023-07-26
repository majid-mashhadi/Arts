using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Public_API.Models.Account;
using Public_API.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

//builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.RegisterServices();


var app = builder.Build();
// Get the configuration
var configuration = app.Configuration;

// Get the connection string from the configuration
var connectionString = configuration.GetConnectionString("DefaultConnection");


// Create the database context options with MySQL provider
//using (var connection = new MySqlConnection(connectionString))
//{
//    // Open the connection to ensure the database is created and migrations are applied
//    connection.Open();
    
//}


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}


app.UseSwagger();
app.UseSwaggerUI();

//app.UseIdentityServer();

app.UseCors("Application");

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
    db?.Database.Migrate();
}

app.Run();

