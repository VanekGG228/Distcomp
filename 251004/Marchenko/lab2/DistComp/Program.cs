using System.Text.Json.Serialization;
using DistComp.Extensions;
using DistComp.Infrastructure.Mapper;
using DistComp.Infrastructure.Validators;
using DistComp.Middleware;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Infrastructure
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Регистрируем все валидаторы из сборки (текущей), где находится StoryRequestDTOValidator
// Scoped lifetime
builder.Services.AddValidatorsFromAssemblyContaining<CreatorRequestDTOValidator>();

builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDbContext(builder.Configuration);

var app = builder.Build();

// Middleware для глобальных ошибок
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.DeepSpace);
    });
}

app.UseAuthorization();

app.MapControllers();

//app.ApplyMigrations(app.Services);

app.Run();