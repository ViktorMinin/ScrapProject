namespace ProductParser.DAL.Models;

public class MangaModel
{
    public long Id { get; set; }
    public string? OriginalName { get; set; }
    public string? RusName { get; set; }
    public string Type { get; set; }
    public long? ViewCount { get; set; }
    public double? Rating { get; set; }
    public string? TitleStatus { get; set; }
    public string? UnlateStatus { get; set; }
    public string? ImageUrl { get; set; }
    public string? TitleUrl { get; set; }
    public int? ChapterCount { get; set; }
}

public class MangaAuthor
{
    public long Id { get; set; }
    public long MangaId { get; set; }
    public long AuthorId { get; set; }
}
public class MangaGenre
{
    public long Id { get; set; }
    public long MangaId { get; set; }
    public long GenreId { get; set; }
}