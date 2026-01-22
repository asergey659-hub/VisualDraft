using System.Text.Json.Serialization;

namespace VisualDraft.Domain.Entities
{
    /// <summary>
    /// Представляет комментарий (ответ) к пину на макете.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Уникальный идентификатор комментария.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор родительского пина, к которому относится данный комментарий.
        /// </summary>
        public Guid PinId { get; set; }

        /// <summary>
        /// Текст комментария.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания комментария (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}