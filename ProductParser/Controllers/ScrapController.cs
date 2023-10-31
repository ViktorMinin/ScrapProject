using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using ProductParser.DAL.Models;

namespace ProductParser.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ScrapController : ControllerBase
{
    private readonly HttpClient _httpClientV;
    private readonly HttpClient _httpClientS;

    public ScrapController(IHttpClientFactory httpClientFactory)
    {
        _httpClientV = httpClientFactory.CreateClient("VRequest");
        _httpClientS = httpClientFactory.CreateClient("SRequest");
    }
    
    [HttpGet("V/get")]
    public async Task<string> GRequestGet()
    {
        List<string> productCategoryList = new List<string>() {"podarochnye-korziny_1"
            ,"sobstvennoe-proizvodstvo",
            "konditerskaya-vkusnye-istorii",
            "pekarnya-vkus-khleba",
        };
        var mainUrl = "https://vkusmart.vmv.kz/catalog/";
        var web = new HtmlWeb();
        List<ProductModel> productModels = new List<ProductModel>();
        foreach (var productCategory in productCategoryList)
        {
            string url = $"{productCategory}/?PAGEN_1=1";
            var doc = web.Load(mainUrl + url);

            var nextLink = doc.DocumentNode.SelectSingleNode("//link[@rel='next']");
            while (nextLink != null)
            {

                var itemInfo = doc.DocumentNode.SelectNodes("//div[@class='inner_wrap TYPE_1']");

                if (itemInfo is not null)
                    Parallel.ForEach(itemInfo, item =>
                    {
                        var nameNode = item.SelectSingleNode(".//div[@class='item-title']//a");
                        var priceNode = item.SelectSingleNode(".//span[@class='price_value']");
                        var imageNode = item.SelectSingleNode(".//img[@class='img-responsive']");

                        var name = nameNode.InnerText ?? "Empty";
                        var price = priceNode.InnerText ?? "0";
                        var image = imageNode?.GetAttributeValue("src", "") ?? null;

                        var model = new ProductModel
                        {
                            Name = name,
                            Price = Convert.ToDouble(price),
                            ImageUrl = image,
                            MarketPlace = "VkusMart"
                        };
                        productModels.Add(model);
                    });
                string nextUrl = nextLink.GetAttributeValue("href", "");

                doc = web.Load(nextUrl);

                nextLink = doc.DocumentNode.SelectSingleNode("//link[@rel='next']");

            }

        }

        return "";
    }
}