using Microsoft.EntityFrameworkCore;
using VisualDraft.Domain.Entities;

namespace VisualDraft.Data
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Таблица проектов.
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Таблица пинов.
        /// </summary>
        public DbSet<Pin> Pins { get; set; }

        /// <summary>
        /// Таблица комментариев.
        /// </summary>
        public DbSet<Comment> Comments { get; set; } // <--- ВОТ ЭТОГО НЕ ХВАТАЛО

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связей (Fluent API)
            modelBuilder.Entity<Pin>()
                .HasOne(p => p.Project)
                .WithMany(project => project.Pins)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Удаляем проект -> удаляются пины

            modelBuilder.Entity<Comment>()
                .HasOne<Pin>()
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PinId)
                .OnDelete(DeleteBehavior.Cascade); // Удаляем пин -> удаляются комменты
        }
    }
}