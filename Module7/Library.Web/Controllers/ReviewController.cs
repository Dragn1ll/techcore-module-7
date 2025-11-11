using Library.Domain.Abstractions.Services;
using Library.Domain.Abstractions.Storage;
using Library.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

[ApiController]
[Route("api")]
public sealed class ReviewController : Controller
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }
    
    [HttpPost("reviews")]
    public async Task<ActionResult> CreateReview([FromBody] BookReview bookReview)
    {
        await _reviewRepository.AddReviewAsync(bookReview);
        
        return Ok();
    }

    [HttpGet("books/{id:guid}/reviews")]
    public async Task<ActionResult> GetReviews([FromRoute] Guid id)
    {
        var result = await _reviewRepository.GetReviewsForBookAsync(id);
        
        return Ok(result);
    }
}