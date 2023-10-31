using Microsoft.EntityFrameworkCore;
using ProductParser.DAL.Models;

namespace ProductParser.DAL.Repository;

public class ProductRepository : IProductRepository
{
    private readonly IntegrationDbContext _context;

    public ProductRepository(IntegrationDbContext context)
    {
        _context = context;
    }

    public async Task AddProducts(List<ProductModel> productModels)
    {
        foreach (var productModel in productModels)
        {
            var exist = await _context.Product
                .Where(x => x.Name == productModel.Name && x.MarketPlace == productModel.MarketPlace)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.Price, productModel.Price)
                        .SetProperty(p=> p.ImageUrl, productModel.ImageUrl));
            if(exist>0) continue;
            _context.Add(productModel);
        }
        await _context.SaveChangesAsync();
    }
}