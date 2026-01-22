using System.ComponentModel.DataAnnotations;

namespace VisualDraft.Domain.Contracts
{
    /// <summary>
    /// Контракт запроса на создание комментария (пина).
    /// </summary>
    public record CreatePinRequest(
        /// <summary>
        /// Текст комментария.
        /// </summary>
        /// <example>Здесь нужно поправить отступ.</example>
        [Required(ErrorMessage = "Текст комментария обязателен.")]
        string Content,

        /// <summary>
        /// Координата X (в % или пикселях относительно макета).
        /// </summary>
        /// <example>150</example>
        [Required]
        [Range(0, 100000)]
        double X,

        /// <summary>
        /// Координата Y.
        /// </summary>
        /// <example>300</example>
        [Required]
        [Range(0, 100000)]
        double Y
    );
}