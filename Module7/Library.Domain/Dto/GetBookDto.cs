using Library.SharedKernel.Enums;

namespace Library.Domain.Dto;

/// <summary>
/// Данные книги для получения
/// </summary>
public sealed class GetBookDto
{
    public Guid Id { get; init; }
    
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