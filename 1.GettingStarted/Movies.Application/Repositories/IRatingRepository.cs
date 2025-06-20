namespace Movies.Application.Repositories;

internal interface IRatingRepository
{
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default);
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
}
