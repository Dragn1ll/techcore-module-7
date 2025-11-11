using Library.SharedKernel.Enums;

namespace Library.Domain.Dto;

/// <summary>
/// Данные для создания книги
/// </summary>
public sealed class CreateBookDto
{
    /// <summary>Название книги</summary>
    public string Title { get; init; }

    /// <summary>Авторы</summary>
    public IReadOnlyList<string> Authors { get; init; }

    /// <summary>Краткое описание книги</summary>
    public string Description { get; init; }

    /// <summary>Год издания</summary>
    public int Year { get; init; }

    /// <summary>Категория</summary>
    public BookCategory Category { get; init; }
}