using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace VisualDraft.Domain.Entities
{
    /// <summary>
    /// Представляет собой проект (дизайн-макет), загруженный пользователем.
    /// Является корневой сущностью для комментариев.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Уникальный идентификатор проекта.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название проекта, отображаемое в списке.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Относительный путь или URL к файлу изображения макета.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Ширина оригинального изображения в пикселях.
        /// Необходима для предотвращения сдвига верстки на клиенте до загрузки картинки.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота оригинального изображения в пикселях.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Дата и время создания проекта (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Список комментариев (пинов), привязанных к данному макету.
        /// </summary>
        public List<Pin> Pins { get; set; } = new List<Pin>();
    }
}