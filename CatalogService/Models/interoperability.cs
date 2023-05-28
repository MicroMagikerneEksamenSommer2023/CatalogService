using Newtonsoft.Json;


namespace CatalogService.Models
{

    public class Category
    {
        [Newtonsoft.Json.JsonProperty("CategoryCode")]
        public string CategoryCode {get; set;}
        [Newtonsoft.Json.JsonProperty("CategoryName")]
        public string CategoryName {get;set;}
        [Newtonsoft.Json.JsonProperty("ItemDescription")]
        public string ItemDescription {get;set;}
        [Newtonsoft.Json.JsonProperty("AuctionDate")]
        public DateTime AuctionDate {get;set;}
        
        [JsonConstructor]
        public Category(string categoryCode, string categoryName, string itemDescription, DateTime auctionDate)
        {
         this.CategoryCode = categoryCode;
         this.CategoryName = categoryName;
         this.ItemDescription = itemDescription;
         this.AuctionDate = auctionDate;   
        }
    }
}