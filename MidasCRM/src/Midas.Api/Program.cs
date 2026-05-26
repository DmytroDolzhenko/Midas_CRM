using Api.Modules;
using Application;
using Application.Common.Securities;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Midas.Core.Users;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.SetupServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);


// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://midas-crm.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ===== Identity =====
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ===== JWT =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            )
        };
    });


// ===== Authorization =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotDeletedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new NotDeletedRequirement());
    });

    options.DefaultPolicy = options.GetPolicy("NotDeletedUser")!;
});

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Paste JWT token only (without Bearer prefix).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
    });
});

// ===== Controllers =====
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultBufferSize = 16 * 1024;
    });
var app = builder.Build();

// ===== Middleware =====
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Midas API v1");
    options.RoutePrefix = "swagger";
});

await app.InitialiseDatabaseAsync();

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
