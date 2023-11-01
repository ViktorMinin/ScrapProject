using Microsoft.AspNetCore.Mvc;
using ProductParser.Service;

namespace ProductParser.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ScrapController : ControllerBase
{
    private readonly IMangaService _mangaService;
    public ScrapController(IMangaService mangaService)
    {
        _mangaService = mangaService;
    }

    [HttpGet("manga/get")]
    public async Task<bool> GetManga()
    { 
        return await _mangaService.GetManga();
    }
}