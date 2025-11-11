using System.Text.Json;
using Library.Domain.Abstractions.Services;
using Library.Domain.Abstractions.Storage;
using Library.Domain.Dto;
using Library.Domain.Models;
using Library.SharedKernel.Enums;
using Library.SharedKernel.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace Library.Domain.Services;

/// <inheritdoc cref="IBookService"/>
public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IDistributedCache _cache;

    public BookService(IBookRepository bookRepository, IDistributedCache cache)
    {
        _bookRepository = bookRepository;
        _cache = cache;
    }
    
    /// <inheritdoc cref="IBookService.CreateAsync"/>
    public async Task<Result<Guid>> CreateAsync(CreateBookDto createBook)
    {
        try
        {
            var bookId = await _bookRepository.AddAsync(new Book
            {
                Title = createBook.Title,
                Description = createBook.Description,
                Authors = createBook.Authors,
                Year = createBook.Year,
                Category = createBook.Category
            });
            
            return Result<Guid>.Success(bookId);
        }
        catch (Exception exception)
        {
            return Result<Guid>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    /// <inheritdoc cref="IBookService.GetByIdAsync"/>
    public async Task<Result<GetBookDto>> GetByIdAsync(Guid bookId)
    {
        try
        {
            var key = $"book:{bookId}";
            var book = JsonSerializer.Deserialize<Book>(await _cache.GetStringAsync(key) ?? string.Empty);

            if (book == null)
            {
                book = await _bookRepository.GetByIdAsync(bookId);

                if (book == null)
                {
                    throw new ArgumentException("Книга не найденаю.");
                }
                
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(book));
            }
            
            return Result<GetBookDto>.Success(new GetBookDto
            {
                Id = bookId,
                Title = book.Title,
                Category = book.Category,
                Authors = book.Authors,
                Description = book.Description,
                Year = book.Year
            });
        }
        catch (Exception exception)
        {
            return Result<GetBookDto>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    /// <inheritdoc cref="IBookService.GetAllAsync"/>
    public async Task<Result<ICollection<GetBookDto>>> GetAllAsync()
    {
        try
        {
            var books = await _bookRepository.GetAllAsync();

            return Result<ICollection<GetBookDto>>.Success(books
                .Select(e => new GetBookDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Category = e.Category,
                    Authors = e.Authors,
                    Description = e.Description,
                    Year = e.Year
                })
                .ToList());
        }
        catch (Exception exception)
        {
            return Result<ICollection<GetBookDto>>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    /// <inheritdoc cref="IBookService.UpdateAsync"/>
    public async Task<Result> UpdateAsync(Guid bookId, UpdateBookDto updateBook)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            
            if (book == null)
            {
                return Result.Failure(new Error(ErrorType.NotFound, "Книга не найдена"));
            }

            book.Title = updateBook.Title;
            book.Description = updateBook.Description;
            book.Year = updateBook.Year;
            
            await _bookRepository.UpdateAsync(book);
            
            await _cache.RemoveAsync($"book:{bookId}");

            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    /// <inheritdoc cref="IBookService.DeleteBook"/>
    public async Task<Result> DeleteBook(Guid bookId)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                return Result.Failure(new Error(ErrorType.NotFound, "Книга не найдена"));
            }

            await _bookRepository.DeleteAsync(bookId);
            
            await _cache.RemoveAsync($"book:{bookId}");
            
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }
}