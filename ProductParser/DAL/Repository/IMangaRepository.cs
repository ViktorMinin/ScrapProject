using ProductParser.DAL.Models;
using ProductParser.DTO;

namespace ProductParser.DAL.Repository;

public interface IMangaRepository
{
    public Task<bool> AddMange(List<MangaInfo> manga);
}