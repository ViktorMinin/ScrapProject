using ProductParser.DAL.Models;

namespace ProductParser.DTO;

public class MangaInfo
{
    public MangaModel Manga { get; set; }
    public List<GenreModel> Genres { get; set; }
    public List<AuthorModel> Authors { get; set; }
}