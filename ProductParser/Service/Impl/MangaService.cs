using HtmlAgilityPack;
using ProductParser.DAL.Models;
using ProductParser.DAL.Repository;
using ProductParser.DTO;

namespace ProductParser.Service.Impl;

public class MangaService : IMangaService
{
    private readonly IMangaRepository _mangaRepository;

    public MangaService(IMangaRepository mangaRepository)
    {
        _mangaRepository = mangaRepository;
    }

    public async Task<bool> GetManga()
    {
        try
        {
            HtmlWeb web = new HtmlWeb();
            const string url = "https://x.desu.me/";
            var doc = web.Load(url);
            Object lockMe = new Object(); 
            if (doc is not null)
            {
                string nextPageUrl = "manga";
                while (true)
                {
                    doc = web.Load(url+nextPageUrl);

                    List<MangaInfo> mangas = new List<MangaInfo>();
                    
                    var titles = doc.DocumentNode.SelectNodes("//a[@class='animeTitle oTitle']");

                    if (titles is not null)

                        Parallel.ForEach(titles, title =>
                        {
                            var titleHref = title.GetAttributeValue("href", "");
                            var titleDoc = new HtmlWeb().Load(url + titleHref);
                            if (titleDoc is not null)
                            {
                                
                                var manga = new MangaModel();
                                
                                List<AuthorModel> authorModels = new List<AuthorModel>();
                                List<GenreModel> genreModels = new List<GenreModel>();

                                
                                var titleOriginalNameNode = titleDoc.DocumentNode.SelectSingleNode("//span[@class='name']");
                                var titleSecondNameNode = titleDoc.DocumentNode.SelectSingleNode("//span[@class='rus-name']");

                                manga.OriginalName = titleOriginalNameNode?.InnerText ?? "Empty";
                                manga.RusName = titleSecondNameNode?.InnerText ?? "Empty";
                                manga.TitleUrl = url + titleHref;
                                
                                var keyTypeDiv = titleDoc.DocumentNode.SelectSingleNode("//div[@class='key' and text()='Тип:']");
                                manga.Type = "None";
                                if (keyTypeDiv != null)
                                {
                                    HtmlNode valueDiv = keyTypeDiv.ParentNode.SelectSingleNode(".//div[@class='value']");
                                    manga.Type = valueDiv?.InnerText ?? "None";
                                    
                                }
                                
                                var keyStatusDiv = titleDoc.DocumentNode.SelectSingleNode("//div[@class='key' and text()='Статус:']");
                                
                                if (keyStatusDiv != null)
                                {
                                    HtmlNode valueDiv = keyStatusDiv.ParentNode.SelectSingleNode(".//div[@class='value']");
                                    manga.TitleStatus = valueDiv.SelectSingleNode(".//span")?.InnerText.Replace("\n","") ?? "None";
                                    
                                }
                                
                                var keyImageDiv = titleDoc.DocumentNode.SelectSingleNode("//img[@itemprop='image']");
                                
                                if (keyImageDiv != null)
                                {
                                    manga.ImageUrl = url + keyImageDiv.GetAttributeValue("src", "");

                                }
                                
                                var keyUnlateDiv = titleDoc.DocumentNode.SelectSingleNode("//div[@class='key' and text()='Перевод:']");
                                
                                if (keyUnlateDiv != null)
                                {
                                    HtmlNode valueDiv = keyUnlateDiv.ParentNode.SelectSingleNode(".//div[@class='value']");;
                                    manga.UnlateStatus = valueDiv?.InnerText.Replace("\n", "") ?? "None";
                                }

                                var keyViewCountDiv = titleDoc.DocumentNode.SelectSingleNode("//div[@class='key' and text()='Просмотров:']");
                                
                                if (keyViewCountDiv != null)
                                {
                                    HtmlNode valueDiv = keyViewCountDiv.ParentNode.SelectSingleNode(".//div[@class='value']").SelectSingleNode(".//div[@class='value']");
                                    manga.ViewCount = Convert.ToInt32(valueDiv.FirstChild?.InnerText ?? "0");
                                }

                                
                                var genres = titleDoc.DocumentNode.SelectNodes(".//a[@class='tag PreviewTooltip']");
                                
                                if (genres is not null)
                                {
                                    foreach (var genre in genres)
                                    {
                                        var genreHref = genre.GetAttributeValue("href", "");
                                        var genreName = genre.LastChild.InnerText;
                                        genreModels.Add(new GenreModel()
                                        {
                                            GenreHref = url+genreHref,
                                            GenreName = genreName
                                        });
                                    }
                                }

                                var authors =
                                    titleDoc.DocumentNode.SelectNodes("//a[@class='b-tag Tooltip bubbled-processed']");
                                if(authors is not null)
                                    foreach (var author in authors)
                                    {
                                        if (author.GetAttributeValue("href", "").Contains("manga?people"))
                                            authorModels.Add(
                                                new AuthorModel
                                                {
                                                    AuthorHref = url+author.GetAttributeValue("href", ""),
                                                    AuthorName = author?.InnerText ?? "None"
                                                }
                                            );
                                    }

                                var mangaCount = titleDoc.DocumentNode.SelectNodes("//a[@class='tips Tooltip']");

                                manga.ChapterCount = mangaCount?.Count ?? 0;

                                var score = titleDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'score-value')]");

                                manga.Rating = Convert.ToDouble(score?.InnerText.Replace(".",",") ?? "0");
                                
                                lock (lockMe)
                                {
                                    mangas.Add(new MangaInfo()
                                    {
                                        Manga = manga,
                                        Authors = authorModels,
                                        Genres = genreModels
                                    });
                                }
                            }
                        });
                    await _mangaRepository.AddManga(mangas);
                    var nextNodes = doc.DocumentNode.SelectNodes("//a[@class='text']");
                    HtmlNode? nextNode = null;
                    foreach (var next in nextNodes)
                    {
                        if (next.InnerText.Contains("Вперёд"))
                            nextNode = next;
                    }
                    if(nextNode is null)
                        break;
                    nextPageUrl = nextNode.GetAttributeValue("href", "");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Service error {ex.Message}");
            return false;
        }

        return true;
    }
}