using Microsoft.EntityFrameworkCore;
using ProductParser.DAL.Models;
using ProductParser.DTO;

namespace ProductParser.DAL.Repository;

public class MangaRepository : IMangaRepository
{
    private readonly IntegrationDbContext _context;

    public MangaRepository(IntegrationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddMange(List<MangaInfo> manga)
    {
        foreach (var title in manga)
        {
            foreach (var author in title.Authors)
            {
                var authorUpdate = await _context.Author.Where(x => x.AuthorName == author.AuthorName)
                    .ExecuteUpdateAsync(x => x.SetProperty
                        (p => p.AuthorHref, author.AuthorHref));
                if (authorUpdate < 1)
                { 
                    await _context.Author.AddAsync(author);
                }

            }
            foreach (var genre in title.Genres)
            {
                var authorUpdate = await _context.Genre.Where(x => x.GenreName == genre.GenreName)
                    .ExecuteUpdateAsync(x => x.SetProperty
                        (p => p.GenreHref, genre.GenreHref));
                if (authorUpdate < 1)
                    await _context.Genre.AddAsync(genre);
            }

            var titleUpdate = await _context.Manga.Where(x => x.OriginalName == title.Manga.OriginalName)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.RusName, title.Manga.RusName)
                        .SetProperty(p => p.Rating, title.Manga.Rating)
                        .SetProperty(p => p.ChapterCount, title.Manga.ChapterCount)
                        .SetProperty(p => p.TitleStatus, title.Manga.TitleStatus)
                        .SetProperty(p => p.UnlateStatus, title.Manga.TitleStatus)
                        .SetProperty(p => p.ViewCount, title.Manga.ViewCount));
            if (titleUpdate < 1)
                await _context.Manga.AddAsync(title.Manga);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var titleId = _context.Manga.First(x => x.OriginalName == title.Manga.OriginalName).Id;
                var authorsId = new List<long>();
                foreach (var author in title.Authors)
                {
                    authorsId.Add(_context.Author.First(x=>x.AuthorName==author.AuthorName).Id);
                }
                var genresId = new List<long>();
                foreach (var author in title.Genres)
                {
                    genresId.Add(_context.Genre.First(x=>x.GenreName==author.GenreName).Id);
                }

                foreach (var authorId in authorsId)
                {
                    var exist = await _context.MangaAuthors.FirstOrDefaultAsync(x => x.AuthorId == authorId && x.MangaId == titleId);
                    if (exist is null)
                        await _context.MangaAuthors.AddAsync(new MangaAuthor()
                        {
                            AuthorId = authorId,
                            MangaId = titleId
                        });
                }
                foreach (var genreId in genresId)
                {
                    var exist = await _context.MangaGenres.FirstOrDefaultAsync(x => x.GenreId == genreId && x.MangaId == titleId);
                    if (exist is null)
                        await _context.MangaGenres.AddAsync(new MangaGenre()
                        {
                            GenreId = genreId,
                            MangaId = titleId
                        });
                }
                
                await _context.SaveChangesAsync();
            }
        }

        return true;
    }
}