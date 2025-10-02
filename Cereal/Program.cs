using Cereal.Context;
using Cereal.Interfaces;
using Cereal.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cereal API", Version = "v1" });
    // makes the auth button appear in swagger ui. The button is used to enter username;password and send it with authorization header
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Enter your username and password"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basic" } // reference the defined scheme above
            },
            new string[] {}
        }
    });
});

// Pomelo MySQL provider
builder.Services.AddDbContext<CerealContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddScoped<ParserManager>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null); // Register the custom auth handler
builder.Services.AddAuthorization(); // flow: swagger ui auth button -> auth handler -> auth manager -> db

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // makes wwwroot folder publicly available
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();
