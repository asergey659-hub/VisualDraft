using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using VisualDraft.Api.Endpoints;
using VisualDraft.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 1. Services ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString, b => b.MigrationsAssembly("VisualDraft.Data")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VisualDraft API", Version = "v1" });
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Точный адрес фронтенда (без слеша в конце!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Разрешаем куки/авторизацию для SignalR
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

// --- 2. Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// --- 3. Endpoints ---

app.MapProjectEndpoints();

app.MapGet("/ping", () => Results.Ok("Pong!"))
   .WithName("HealthCheck");

app.MapHub<VisualDraft.Api.Hubs.DesignHub>("/hubs/design");

app.Run();