using System;

namespace VisualDraft.Domain.Entities
{
    /// <summary>
    /// Точка обсуждения (пин) на макете.
    /// Содержит координаты и текст комментария.
    /// </summary>  
    public class Pin
    {
        /// <summary>
        /// Уникальный идентификатор комментария.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Координата X по горизонтали.
        /// </summary>
        /// <remarks>
        /// Хранится в относительном значении (от 0.0 до 1.0), где 1.0 = 100% ширины.
        /// </remarks>
        public double X { get; set; }

        /// <summary>
        /// Координата Y по вертикали (от 0.0 до 1.0).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Текст комментария, оставленного пользователем.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Имя автора комментария.
        /// </summary>
        // TODO: В будущем заменить на User ID после внедрения системы аутентификации.
        public string AuthorName { get; set; } = string.Empty;

        /// <summary>
        /// Флаг состояния: true, если проблема решена или обсуждение закрыто.
        /// </summary>
        public bool IsResolved { get; set; } = false;

        /// <summary>
        /// Дата создания комментария (в формате UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Навигационные свойства ---

        /// <summary>
        /// Внешний ключ для связи с проектом.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Ссылка на проект-родитель.
        /// Может быть null, если данные проекта не были явно загружены (Lazy Loading).
        /// </summary>
        public Project? Project { get; set; }
    }
}