using Microsoft.AspNetCore.Mvc;
using ProductParser.DAL;

namespace ProductParser.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ScrapController : ControllerBase
{
    private readonly IntegrationDbContext _context;
    public ScrapController(IntegrationDbContext context)
    {
        _context = context;
    }
    
}