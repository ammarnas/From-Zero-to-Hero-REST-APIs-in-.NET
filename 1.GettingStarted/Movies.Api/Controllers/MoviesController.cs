﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Api.Utilities;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }
    [Authorize(policy: AuthConstant.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request,
        CancellationToken token)
    {
        var movie = request.MapToMovie();

        await _movieService.CreateAsync(movie, token);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task <IActionResult> Get([FromRoute] string idOrSlug,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var movies = await _movieService.GetAllAsync(userId, token);
        var response = movies.MapToResponse();

        return Ok(response);
    }

    [Authorize(policy: AuthConstant.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var userId = HttpContext.GetUserId();

        var updated = await _movieService.UpdateAsync(movie, userId, token);

        if (updated is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(policy: AuthConstant.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,
        CancellationToken token)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, token);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
