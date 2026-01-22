using System.Text.Json.Serialization;

namespace VisualDraft.Domain.Entities
{
    /// <summary>
    /// Сущность, представляющая точку обсуждения (пин) на макете проекта.
    /// Содержит координаты, начальное сообщение и список ответов.
    /// </summary>
    public class Pin
    {
        /// <summary>
        /// Уникальный идентификатор пина.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор проекта, к которому привязан данный пин.
        /// Внешний ключ для связи с таблицей Projects.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Навигационное свойство для доступа к сущности проекта.
        /// Помечено [JsonIgnore], чтобы избежать циклической ссылки при сериализации (Project -> Pins -> Project).
        /// </summary>
        [JsonIgnore]
        public Project? Project { get; set; }

        /// <summary>
        /// Основное содержимое пина (заголовок или текст первого сообщения).
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Координата X точки на макете (в пикселях относительно левого верхнего угла изображения).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата Y точки на макете (в пикселях относительно левого верхнего угла изображения).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Дата и время создания пина (в формате UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Коллекция ответов (комментариев) к данному пину.
        /// Инициализируется пустым списком для предотвращения NullReferenceException.
        /// </summary>
        public List<Comment> Comments { get; set; } = new();
    }
}