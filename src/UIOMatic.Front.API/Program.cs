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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();
