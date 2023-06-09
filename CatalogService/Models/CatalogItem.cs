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
        public double Valuation { get; set; }

        [Newtonsoft.Json.JsonProperty("startingBid")]
        public double StartingBid { get; set; }

        [Newtonsoft.Json.JsonProperty("buyoutPrice")]
        public double BuyoutPrice { get; set; }
        [Newtonsoft.Json.JsonProperty("startTime")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartTime {get;set;}
        [Newtonsoft.Json.JsonProperty("endTime")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime EndTime {get;set;}

    [JsonConstructor]
    public CatalogItem(string sellerId, string itemName, string description, string category, double valuation, double startingBid, double buyoutPrice,DateTime startTime, DateTime endTime)
    {
        this.SellerId = sellerId;
        this.ItemName = itemName;
        this.Description = description;
        this.Category = category;
        this.Valuation = valuation;
        this.StartingBid = startingBid;
        this.BuyoutPrice = buyoutPrice;
        this.StartTime = startTime;
        this.EndTime = endTime;
    }
       public CatalogItem(string id,string sellerId, string itemName, string description, string category, double valuation, double startingBid, double buyoutPrice,DateTime startTime, DateTime endTime)
    {
        this.Id = id;
        this.SellerId = sellerId;
        this.ItemName = itemName;
        this.Description = description;
        this.Category = category;
        this.Valuation = valuation;
        this.StartingBid = startingBid;
        this.BuyoutPrice = buyoutPrice;
        this.StartTime = startTime;
        this.EndTime = endTime;
    }


    public CatalogItemDB Convert(List<string> paths)
    {
        return new CatalogItemDB(this.SellerId, this.ItemName,this.Description,this.Category,this.Valuation,this.StartingBid,this.BuyoutPrice,paths,this.StartTime,this.EndTime);
    }

    }

        

}