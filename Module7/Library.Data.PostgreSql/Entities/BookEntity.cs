using Library.SharedKernel.Enums;

namespace Library.Data.PostgreSql.Entities;

/// <summary>
/// Сущность книги
/// </summary>
public class BookEntity
{
    /// <summary>Идентификатор книги</summary>
    public Guid Id { get; set; }
    
    /// <summary>Название книги</summary>
    public string Title { get; set; }

    /// <summary>Авторы</summary>
    public ICollection<AuthorEntity> Authors { get; set; }

    /// <summary>Краткое описание книги</summary>
    public string Description { get; set; }

    /// <summary>Год издания</summary>
    public int Year { get; set; }

    /// <summary>Категория</summary>
    public BookCategory Category { get; set; }
}