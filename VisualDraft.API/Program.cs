using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using VisualDraft.Api.Endpoints;
using VisualDraft.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Services ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VisualDraft API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

// Одна строка вместо кучи кода!
app.MapProjectEndpoints();

app.MapGet("/ping", () => Results.Ok("Pong!"))
   .WithName("HealthCheck");

app.Run();