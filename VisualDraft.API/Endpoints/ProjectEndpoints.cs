using Microsoft.EntityFrameworkCore;
using VisualDraft.Data;
using VisualDraft.Domain.Contracts;
using VisualDraft.Domain.Entities;

namespace VisualDraft.Api.Endpoints
{
    /// <summary>
    /// Класс расширения для регистрации эндпоинтов, связанных с проектами.
    /// Позволяет вынести логику маршрутизации из Program.cs.
    /// </summary>
    public static class ProjectEndpoints
    {
        /// <summary>
        /// Регистрирует маршруты API для управления проектами.
        /// </summary>
        /// <param name="app">Построитель маршрутов (обычно WebApplication).</param>
        public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/projects")
                           .WithTags("Projects");

            // POST: Создание проекта
            group.MapPost("/", CreateProject)
                 .WithName("CreateProject")
                 .Produces<Project>(201)
                 .Produces(400)
                 .WithSummary("Создать новый проект")
                 .WithDescription("Создает запись о проекте в БД и возвращает созданный объект.");

            // GET: Список проектов
            group.MapGet("/", GetProjects)
                 .WithName("GetProjects")
                 .Produces<List<Project>>(200)
                 .WithSummary("Получить список проектов")
                 .WithDescription("Возвращает список всех макетов, отсортированных по дате создания.");

            // GET: Проект по ID
            group.MapGet("/{id:guid}", GetProjectById)
                 .WithName("GetProjectById")
                 .Produces<Project>(200)
                 .Produces(404)
                 .WithSummary("Получить проект по ID")
                 .WithDescription("Возвращает полную информацию о проекте, включая список всех пинов.");
        }

        // --- Обработчики (Handlers) ---

        private static async Task<IResult> CreateProject(
            CreateProjectRequest request,
            AppDbContext context)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                ImageUrl = request.ImageUrl,
                Width = request.Width,
                Height = request.Height,
                CreatedAt = DateTime.UtcNow
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            return Results.Created($"/api/projects/{project.Id}", project);
        }

        private static async Task<IResult> GetProjects(AppDbContext context)
        {
            var projects = await context.Projects
                                        .AsNoTracking()
                                        .OrderByDescending(p => p.CreatedAt)
                                        .ToListAsync();
            return Results.Ok(projects);
        }

        private static async Task<IResult> GetProjectById(Guid id, AppDbContext context)
        {
            var project = await context.Projects
                .AsNoTracking()
                .Include(p => p.Pins)
                .FirstOrDefaultAsync(p => p.Id == id);

            return project is not null
                ? Results.Ok(project)
                : Results.NotFound();
        }
    }
}