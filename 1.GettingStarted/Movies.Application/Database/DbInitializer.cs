using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateDbConnectionAsync();

        await connection.ExecuteAsync(
        """
            CREATE TABLE IF NOT EXISTS movies (
                id UUID PRIMARY KEY,
                slug Text not null,
                title Text not null,
                yearofrelease integer not null
            );
            """);

        await connection.ExecuteAsync("""
                create unique index concurrently if not exists movies_slug_idx
                on movies
                using btree (slug);
            """);

            
        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS genres (
                movieId UUID references movies (id),
                name Text not null
            );
            """);
    }
}
