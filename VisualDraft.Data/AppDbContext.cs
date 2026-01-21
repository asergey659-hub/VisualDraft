using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using VisualDraft.Domain.Entities;

namespace VisualDraft.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор: принимает настройки (например, строку подключения) и отдает их базовому классу
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Регистрация таблиц
        public DbSet<Project> Projects { get; set; }
        public DbSet<Pin> Pins { get; set; }

        // Настройка схемы базы данных
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Настройка Project ---
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id); // Первичный ключ
                entity.Property(e => e.Title).IsRequired(); // Обязательное поле

                // Настройка связи "Один-ко-многим"
                entity.HasMany(e => e.Pins)
                      .WithOne(p => p.Project)
                      .HasForeignKey(p => p.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade); // Удалил проект -> удалились все пины. Важно!
            });

            // --- Настройка Pin ---
            modelBuilder.Entity<Pin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.X).IsRequired();
                entity.Property(e => e.Y).IsRequired();
            });
        }
    }
}