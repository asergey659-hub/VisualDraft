using System.ComponentModel.DataAnnotations;

namespace VisualDraft.Domain.Contracts
{
    /// <summary>
    /// Контракт (DTO) запроса на создание нового проекта.
    /// Содержит только данные, необходимые для инициализации макета.
    /// </summary>
    public record CreateProjectRequest(
        /// <summary>
        /// Название проекта.
        /// </summary>
        /// <example>Дизайн лендинга v1</example>
        [Required(ErrorMessage = "Название проекта обязательно.")]
        [MaxLength(200, ErrorMessage = "Название не может превышать 200 символов.")]
        string Title,

        /// <summary>
        /// Ссылка на файл изображения (относительный путь или URL).
        /// </summary>
        /// <example>/uploads/layout-2024.jpg</example>
        [Required(ErrorMessage = "URL изображения обязателен.")]
        string ImageUrl,

        /// <summary>
        /// Ширина изображения в пикселях.
        /// </summary>
        /// <example>1920</example>
        [Range(1, 20000, ErrorMessage = "Ширина должна быть в диапазоне от 1 до 20000 пикселей.")]
        int Width,

        /// <summary>
        /// Высота изображения в пикселях.
        /// </summary>
        /// <example>1080</example>
        [Range(1, 20000, ErrorMessage = "Высота должна быть в диапазоне от 1 до 20000 пикселей.")]
        int Height
    );
}