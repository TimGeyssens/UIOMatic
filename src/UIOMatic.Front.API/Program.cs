using HotChocolate.Language;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SqlClient;
using UIOMatic.Front.API;
using UIOMatic.Front.API.Data;
using UIOMatic.Front.API.Extensions;
using UIOMatic.Front.API.Services;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LiteDB;
using System.IO;
using UIOMatic.Front.API.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add LiteDB
var dbPath = System.IO.Path.Combine(builder.Environment.ContentRootPath, "UIOMatic.db");
builder.Services.AddSingleton(new LiteDatabase(dbPath));
builder.Services.AddSingleton<LiteDBMigrationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddSingleton<UIOMaticObjectService>();
builder.Services.AddTransient<IUIOMaticObjectService, ObjectService>();
builder.Services.AddSingleton<IUIOMaticHelper, UIOMaticHelper>();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddTransient<QueryFactory>((e) =>
{
    var connection = new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
    var compiler = new SqlServerCompiler();

    return new QueryFactory(connection, compiler);
});

builder.Services.AddSingleton<UIOMaticTypeModule>();

builder.Services.AddGraphQLServer()
                 .AddTypeModule<UIOMaticTypeModule>();

builder.Services.AddMemoryCache();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite's default port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Run database migrations
var migrationService = app.Services.GetRequiredService<LiteDBMigrationService>();
await migrationService.InitializeDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS middleware
app.UseCors("SpaPolicy");

// Add static file serving
app.UseStaticFiles();

// Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();
