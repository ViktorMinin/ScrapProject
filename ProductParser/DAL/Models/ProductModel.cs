namespace ProductParser.DAL.Models;

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string? ImageUrl { get; set; }
    public string MarketPlace { get; set; }
}