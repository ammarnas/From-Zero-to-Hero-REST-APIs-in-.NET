using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid  Id { get; init; }
    public required string Title { get; init; }
    public string Slug => GenerateSlug();
    public required int YearOfRelease { get; init; }
    public required List<string> Genres { get; init; } = new();
    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower()
            .Replace(" ", "-");

        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^a-zA-Z0-9 _-]", RegexOptions.NonBacktracking, 10)]
    private static partial Regex SlugRegex();
}
