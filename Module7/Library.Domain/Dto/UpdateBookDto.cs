namespace Library.Domain.Dto;

/// <summary>
/// Данные для обновления книги
/// </summary>
public sealed class UpdateBookDto
{
    /// <summary>Название книги</summary>
    public string Title { get; init; }

    /// <summary>Краткое описание книги</summary>
    public string Description { get; init; }

    /// <summary>Год издания</summary>
    public int Year { get; init; }
}