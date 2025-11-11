using Library.Domain.Abstractions.Storage;
using Library.Domain.Models;
using Library.Domain.Services;
using Library.SharedKernel.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Library.Tests;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _repoMock = new();
    private readonly Mock<IDistributedCache> _cacheMock = new();
    private readonly BookService _service;

    public BookServiceTests()
    {
        _service = new BookService(_repoMock.Object, _cacheMock.Object);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Authors = ["Author1"],
            Description = "Description",
            Year = 2020,
            Category = BookCategory.FictionBook
        };

        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        _repoMock.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        // Act
        var result = await _service.GetByIdAsync(bookId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(bookId, result.Value.Id);
        Assert.Equal("Test Book", result.Value.Title);

        _repoMock.Verify(r => r.GetByIdAsync(bookId), Times.Once);
        _cacheMock.Verify(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), 
            CancellationToken.None), Times.Once);
    }
}