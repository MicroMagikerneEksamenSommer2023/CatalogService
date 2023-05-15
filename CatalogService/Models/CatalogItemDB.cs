using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CatalogService.Models
{
    public class CatalogItemDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string SellerId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Valuation { get; set; }
        public double StartingBid { get; set; }
        public double BuyoutPrice { get; set; }
        public List<string>? ImagePaths { get; set; }

    public CatalogItemDB(string sellerId, string itemName, string description, string category, double valuation, double startingBid, double buyoutPrice, List<string> imagepaths)
    {
        this.SellerId = sellerId;
        this.ItemName = itemName;
        this.Description = description;
        this.Category = category;
        this.Valuation = valuation;
        this.StartingBid = startingBid;
        this.BuyoutPrice = buyoutPrice;
        this.ImagePaths = imagepaths;
    }
      public CatalogItemDB(string sellerId, string itemName, string description,string category, double valuation, double startingBid, double buyoutPrice)
    {
        this.SellerId = sellerId;
        this.ItemName = itemName;
        this.Description = description;
        this.Category = category;
        this.Valuation = valuation;
        this.StartingBid = startingBid;
        this.BuyoutPrice = buyoutPrice;
    }
    public CatalogItem Convert()
    {
        return new CatalogItem(this.Id, this.SellerId, this.ItemName,this.Description,this.Category,this.Valuation,this.StartingBid,this.BuyoutPrice);
    }
    }
}