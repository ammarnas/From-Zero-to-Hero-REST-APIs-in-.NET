using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Api.Utilities;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();

        await _movieRepository.CreateAsync(movie);
        return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
    }

    [HttpGet(ApiEndpoints.Movies.GetById)]
    public async Task <IActionResult> GetById([FromRoute] Guid id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie is null)
        {
            return NotFound();

        }

        var response = movie.MapToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieRepository.GetAllAsync();
        var response = movies.MapToResponse();

        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);

        var updated = await _movieRepository.UpdateAsync(movie);

        if (!updated)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }
}
