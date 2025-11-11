using Library.Domain.Abstractions.Services;
using Library.Domain.Abstractions.Storage;
using Library.Domain.Models;
using Library.SharedKernel.Enums;
using Library.SharedKernel.Utils;

namespace Library.Domain.Services;

/// <inheritdoc cref="IReviewService"/>
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    
    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }
    
    /// <inheritdoc cref="IReviewService.GetReviewsForBookAsync"/>
    public async Task<Result<List<BookReview>>> GetReviewsForBookAsync(Guid bookId)
    {
        try
        {
            var reviews = await _reviewRepository.GetReviewsForBookAsync(bookId);
            
            return Result<List<BookReview>>.Success(reviews);
        }
        catch (Exception exception)
        {
            return Result<List<BookReview>>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result<List<BookReview>>> GetAllReviewsAsync()
    {
        try
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();
            
            return Result<List<BookReview>>.Success(reviews);
        }
        catch (Exception exception)
        {
            return Result<List<BookReview>>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }
}