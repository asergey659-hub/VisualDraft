using Microsoft.EntityFrameworkCore;
using VisualDraft.Domain.Entities;

namespace VisualDraft.Data
{
    /// <summary>
    /// Основной контекст базы данных приложения.
    /// Выступает в роли Unit of Work и Repository.
    /// Отвечает за подключение к БД, маппинг сущностей и сохранение изменений.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста.
        /// Принимает настройки (например, строку подключения или провайдер БД) через Dependency Injection.
        /// </summary>
        /// <param name="options">Настройки контекста, передаваемые из Program.cs</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Таблицы (DbSets) ---

        /// <summary>
        /// Таблица проектов (макетов).
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Таблица пинов (комментариев на макетах).
        /// </summary>
        public DbSet<Pin> Pins { get; set; }

        /// <summary>
        /// Метод настройки схемы базы данных.
        /// Используется Fluent API для точной конфигурации таблиц, ключей и связей.
        /// </summary>
        /// <remarks>
        /// Конфигурация через Fluent API имеет приоритет над Data Annotations (атрибутами).
        /// Это позволяет оставить классы Домена чистыми от логики базы данных.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Конфигурация сущности Project ===
            modelBuilder.Entity<Project>(entity =>
            {
                // Установка первичного ключа
                entity.HasKey(e => e.Id);

                // Поле Title обязательно (NOT NULL) и ограничено 200 символами
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                // Настройка связи "Один-ко-Многим"
                // Удаление Проекта -> Автоматическое удаление всех его Пинов (Cascade)
                entity.HasMany(e => e.Pins)
                      .WithOne(p => p.Project)
                      .HasForeignKey(p => p.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Конфигурация сущности Pin ===
            modelBuilder.Entity<Pin>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Координаты критически важны для отображения, поэтому они обязательны
                entity.Property(e => e.X).IsRequired();
                entity.Property(e => e.Y).IsRequired();

                // Текст комментария обязателен
                entity.Property(e => e.Content).IsRequired();
            });
        }
    }
}