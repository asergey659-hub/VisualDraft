using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VisualDraft.Api.Hubs;
using VisualDraft.Data;
using VisualDraft.Domain.Contracts;
using VisualDraft.Domain.Entities;

namespace VisualDraft.Api.Endpoints
{
    /// <summary>
    /// Класс расширения для регистрации маршрутов API, связанных с проектами и пинами.
    /// </summary>
    public static class ProjectEndpoints
    {
        /// <summary>
        /// Регистрирует эндпоинты для работы с проектами, пинами и комментариями.
        /// </summary>
        /// <param name="app">Построитель маршрутов приложения.</param>
        public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/projects").WithTags("Projects");
            var pinsGroup = app.MapGroup("/api/pins").WithTags("Pins");

            // Проекты
            group.MapPost("/", CreateProject)
                 .WithName("CreateProject")
                 .Produces<Project>(201)
                 .WithSummary("Создать новый проект")
                 .WithDescription("Создает запись проекта в базе данных.");

            group.MapGet("/", GetProjects)
                 .WithName("GetProjects")
                 .Produces<List<Project>>(200)
                 .WithSummary("Получить список проектов");

            group.MapGet("/{id:guid}", GetProjectById)
                 .WithName("GetProjectById")
                 .Produces<Project>(200)
                 .Produces(404)
                 .WithSummary("Получить детали проекта")
                 .WithDescription("Возвращает проект со всеми пинами и комментариями к ним.");

            group.MapPost("/{projectId:guid}/pins", CreatePin)
                 .WithName("CreatePin")
                 .Produces<Pin>(201)
                 .Produces(404)
                 .WithSummary("Создать пин")
                 .WithDescription("Добавляет новый пин к проекту и уведомляет клиентов через SignalR.");

            // Пины (удаление и ответы)
            pinsGroup.MapDelete("/{pinId:guid}", DeletePin)
                     .WithName("DeletePin")
                     .Produces(200)
                     .Produces(404)
                     .WithSummary("Удалить пин")
                     .WithDescription("Удаляет пин и все связанные комментарии.");

            pinsGroup.MapPost("/{pinId:guid}/comments", AddCommentToPin)
                     .WithName("AddComment")
                     .Produces<Comment>(200)
                     .Produces(404)
                     .WithSummary("Добавить комментарий")
                     .WithDescription("Добавляет ответ к существующему пину.");
        }

        // --- Обработчики (Handlers) ---

        /// <summary>
        /// Обработчик создания нового проекта.
        /// </summary>
        private static async Task<IResult> CreateProject(CreateProjectRequest request, AppDbContext context)
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

        /// <summary>
        /// Получает список всех проектов, отсортированных по дате создания (сначала новые).
        /// </summary>
        private static async Task<IResult> GetProjects(AppDbContext context)
        {
            var projects = await context.Projects
                                        .AsNoTracking()
                                        .OrderByDescending(p => p.CreatedAt)
                                        .ToListAsync();
            return Results.Ok(projects);
        }

        /// <summary>
        /// Получает полную информацию о проекте по ID, включая пины и вложенные комментарии.
        /// </summary>
        private static async Task<IResult> GetProjectById(Guid id, AppDbContext context)
        {
            var project = await context.Projects
                .AsNoTracking()
                .Include(p => p.Pins)
                .ThenInclude(pin => pin.Comments) // Eager Loading комментариев
                .FirstOrDefaultAsync(p => p.Id == id);

            return project is not null ? Results.Ok(project) : Results.NotFound();
        }

        /// <summary>
        /// Создает новый пин и рассылает уведомление через SignalR.
        /// </summary>
        private static async Task<IResult> CreatePin(
            Guid projectId,
            CreatePinRequest request,
            AppDbContext context,
            IHubContext<DesignHub> hub)
        {
            var projectExists = await context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists) return Results.NotFound(new { message = "Project not found" });

            var pin = new Pin
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Content = request.Content,
                X = request.X,
                Y = request.Y,
                CreatedAt = DateTime.UtcNow
            };

            context.Pins.Add(pin);
            await context.SaveChangesAsync();

            // Уведомляем подписчиков группы проекта
            await hub.Clients.Group(projectId.ToString()).SendAsync("PinCreated", pin);

            return Results.Created($"/api/pins/{pin.Id}", pin);
        }

        /// <summary>
        /// Удаляет пин по ID и уведомляет клиентов об удалении.
        /// </summary>
        private static async Task<IResult> DeletePin(
            Guid pinId,
            AppDbContext db,
            IHubContext<DesignHub> hub)
        {
            var pin = await db.Pins.FindAsync(pinId);
            if (pin == null) return Results.NotFound();

            db.Pins.Remove(pin);
            await db.SaveChangesAsync();

            // Отправляем ID удаленного пина, чтобы фронтенд убрал его с холста
            await hub.Clients.Group(pin.ProjectId.ToString()).SendAsync("PinDeleted", pinId);

            return Results.Ok();
        }

        /// <summary>
        /// Добавляет текстовый комментарий к пину.
        /// </summary>
        private static async Task<IResult> AddCommentToPin(
            Guid pinId,
            string text,
            AppDbContext db,
            IHubContext<DesignHub> hub)
        {
            var pin = await db.Pins.FirstOrDefaultAsync(p => p.Id == pinId);
            if (pin == null) return Results.NotFound();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                PinId = pinId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            // Отправляем объект с ID пина и самим комментарием для обновления UI
            await hub.Clients.Group(pin.ProjectId.ToString())
                     .SendAsync("CommentAdded", new { PinId = pinId, Comment = comment });

            return Results.Ok(comment);
        }
    }
}