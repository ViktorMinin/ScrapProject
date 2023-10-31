using ProductParser.DAL.Models;

namespace ProductParser.DAL.Repository;

public interface IProductRepository
{
    Task AddProducts(List<ProductModel> productModels);
}