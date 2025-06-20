using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, yearofrelease)
            values (@Id, @Slug, @Title, @YearOfRelease)
         """, movie, cancellationToken: token));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }
        }

        transaction.Commit();
        return result > 0;
    }
    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieId = @id
        """, new { id }, cancellationToken: token));

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id
        """, new { id }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }
    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);

        var result = await connection.QueryAsync(new CommandDefinition("""
            select m.*, string_agg(distinct g.name, ',') as genres,
                round(avg(r.rating), 1) as rating, myr.rating as userRating
            from movies as m 
            left join genres as g 
                on m.id = g.movieId
            left join ratings as r
                on m.id = r.movieId
            left join ratings myr
                on myr.userId = @userId
            group by m.id, userRating
        """, new { userId }, cancellationToken: token));

        return result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = m.yearofrelease,
            Rating = (float?)m.rating,
            UserRating = (int?)m.userRating,
            Genres = Enumerable.ToList(m.genres.Split(','))
        });
    }
    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userRating
            from movies as m
            left join ratings as r
                on m.id = r.movieId
            left join ratings myr
                on myr.userId = @userId
            where id = @id
            group by id, userRating
        """, new { id, userId }, cancellationToken: token));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieId = @id
        """, new { id }, cancellationToken: token));

        // method 1
        //     movie.Genres.AddRange(genres);

        // method 2
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }
    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);
        
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userRating
            from movies as m
            left join ratings as r
                on m.id = r.movieId
            left join ratings myr
                on myr.userId = @userId
            where slug = @slug
            group by id, userRating
        """, new { slug, userId }, cancellationToken: token));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieId = @id
        """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }
    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres
            where movieId = @id
        """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movieId, name)
                values (@MovieId, @Name)
            """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies
            set 
                slug = @Slug,
                title = @Title,
                yearofrelease = @YearOfRelease
            where id = @id
        """, movie, cancellationToken: token));

        transaction.Commit();
        return result > 0;
    }
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync(token);

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from movies where id = @id
        """, new { id }, cancellationToken: token));
    }
}
