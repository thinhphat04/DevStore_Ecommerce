using Azure;
using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.Middleware;
using BackEnd.RequestHelpers;
using BackEnd.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt auth header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<StoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyMethod() // Allow any HTTP method
                .AllowAnyHeader()
                .AllowCredentials();
        });
});


builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
        })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<StoreContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration["JWTSettings:TokenKey"]))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

var app = builder.Build();
var publishableKey = builder.Configuration["StripeSettings:PublishableKey"];
var secretKey = builder.Configuration["StripeSettings:SecretKey"];
var whSecret = builder.Configuration["StripeSettings:WhSecret"];

//data seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await context.Database.MigrateAsync();
        await DbInitializer.Initialize(context, userManager);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Problem migrating data");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//middleware to serve static file of front end
app.UseDefaultFiles();
app.UseStaticFiles();

// Handle CORS preflight requests

app.UseCors("MyCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToController("Index", "Fallback");

app.Run();
