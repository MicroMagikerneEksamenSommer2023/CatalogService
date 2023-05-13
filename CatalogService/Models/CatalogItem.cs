using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CatalogService.Models
{
    public class CatalogItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Newtonsoft.Json.JsonProperty("sellerId")]
        public string SellerId { get; set; }

        [Newtonsoft.Json.JsonProperty("itemName")]
        public string ItemName { get; set; }

        [Newtonsoft.Json.JsonProperty("description")]
        public string Description { get; set; }

        [Newtonsoft.Json.JsonProperty("category")]
        public string Category { get; set; }

        [Newtonsoft.Json.JsonProperty("valuation")]
        public string Valuation { get; set; }

        [Newtonsoft.Json.JsonProperty("startingBid")]
        public double StartingBid { get; set; }

        [Newtonsoft.Json.JsonProperty("buyoutPrice")]
        public double BuyoutPrice { get; set; }

      public CatalogItem(string sellerId, string itemName, string description, string category, string valuation, double startingBid, double buyoutPrice)
    {
        this.SellerId = sellerId;
        this.ItemName = itemName;
        this.Description = description;
        this.Category = category;
        this.Valuation = valuation;
        this.StartingBid = startingBid;
        this.BuyoutPrice = buyoutPrice;
    }

    }

        

}