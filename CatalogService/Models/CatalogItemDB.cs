using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;
using StackExchange.Redis;

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
        public DateTime StartTime {get;set;}
        public DateTime EndTime {get;set;}

    public CatalogItemDB(string sellerId, string itemName, string description, string category, double valuation, double startingBid, double buyoutPrice, List<string> imagepaths, DateTime startTime, DateTime endTime)
    {
        this.SellerId = sellerId;
        this.ItemName = itemName;
        this.Description = description;
        this.Category = category;
        this.Valuation = valuation;
        this.StartingBid = startingBid;
        this.BuyoutPrice = buyoutPrice;
        this.ImagePaths = imagepaths;
        this.StartTime = startTime;
        this.EndTime = endTime;
    }
      public CatalogItemDB(string sellerId, string itemName, string description,string category, double valuation, double startingBid, double buyoutPrice,DateTime startTime, DateTime endTime)
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
    public async Task<ItemWithBid> Convert(string cache)
    {
        double currentBid = await FetchBid(this.Id, cache);
        return new ItemWithBid(this.Id, this.SellerId, this.ItemName,this.Description,this.Category,this.Valuation,this.StartingBid,this.BuyoutPrice,this.StartTime,this.EndTime, currentBid);
    }
    public async Task<double> FetchBid(string id, string connectionstring)
    {
        
        
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionstring);
        IDatabase cache = redis.GetDatabase();
        
            var price = await cache.StringGetAsync("price" + id);
            if (price.IsNull)
            {
                return 0.0;
            }
            else
            {return double.Parse(price);}
            
        }

    
    }
}