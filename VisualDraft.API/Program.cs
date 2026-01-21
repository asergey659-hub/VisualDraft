using Microsoft.EntityFrameworkCore;
using VisualDraft.Data;

// Создаем строителя веб-приложения. 
// Он отвечает за настройку конфигурации (appsettings.json) и регистрацию сервисов.
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. СЛОЙ РЕГИСТРАЦИИ СЕРВИСОВ (Dependency Injection)
// ==========================================

// Получаем строку подключения из конфигурации.
// Это позволяет менять базу данных без перекомпиляции кода (например, на продакшене).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрируем контекст базы данных (AppDbContext).
// Используем SQLite как простую файловую базу для старта.
// В будущем UseSqlite можно легко заменить на UseNpgsql (PostgreSQL) или UseSqlServer.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Регистрируем генератор документации Swagger (OpenAPI).
// Это позволит нам видеть и тестировать наши API-методы через браузер.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка CORS (Cross-Origin Resource Sharing).
// По умолчанию браузеры запрещают запросы с одного домена (localhost:3000 - React) 
// на другой (localhost:5000 - .NET). Мы должны явно разрешить это.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Разрешаем запросы с любых сайтов
              .AllowAnyMethod()  // Разрешаем любые HTTP методы (GET, POST, PUT, DELETE)
              .AllowAnyHeader(); // Разрешаем любые заголовки
    });
});

// Строим приложение. На этом этапе DI-контейнер готов к работе.
var app = builder.Build();

// ==========================================
// 2. КОНВЕЙЕР ОБРАБОТКИ ЗАПРОСОВ (Middleware Pipeline)
// ==========================================
// Порядок middleware здесь КРИТИЧЕСКИ важен. Запрос проходит через них сверху вниз.

// Включаем Swagger UI только в режиме разработки, чтобы не светить структуру API в продакшене.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Включаем политику CORS. Должно быть до обработки эндпоинтов.
app.UseCors("AllowAll");

// Перенаправление HTTP -> HTTPS.
app.UseHttpsRedirection();

// ==========================================
// 3. ЭНДПОИНТЫ (Маршруты)
// ==========================================

// Простейший Health Check. Позволяет мониторингу понять, что приложение живо и база подключена.
app.MapGet("/ping", () =>
{
    return Results.Ok("Pong! API is running and Database is configured.");
})
.WithOpenApi(operation => new(operation)
{
    Summary = "Проверка доступности API",
    Description = "Возвращает статус 200 OK, если сервер запущен."
});

// Запуск прослушивания входящих подключений.
app.Run();