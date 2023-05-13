using CatalogService.Controllers;
using CatalogService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CatalogService.Services
{
    public class CatalogDBService
    {
        private readonly ILogger<CatalogDBService> _logger;

        private readonly IMongoCollection<CatalogItemDB> _catalogitems;

        private readonly IConfiguration _config;

       public CatalogDBService(ILogger<CatalogDBService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            var mongoClient = new MongoClient(_config["connectionsstring"]);
            var database = mongoClient.GetDatabase(_config["database"]);
            _catalogitems = database.GetCollection<CatalogItemDB>(_config["collection"]);
        }
        public List<CatalogItemDB> GetAllItems()
        {
            var filter = Builders<CatalogItemDB>.Filter.Empty;
            return _catalogitems.Find(filter).ToList();
        }
        public CatalogItemDB GetById(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            return _catalogitems.Find(filter).FirstOrDefault();
        }
        public List<CatalogItemDB> GetByCategory(string category)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Category, category);
            return _catalogitems.Find(filter).ToList();
        }
          public CatalogItemDB DeleteById(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            return _catalogitems.FindOneAndDelete(filter);
        }
        public CatalogItemDB UpdateCatalogItem(CatalogItemDB data)
        {
            //kald image service og indset metadata i objektet
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, data.Id);
            var update = Builders<CatalogItemDB>.Update.Set(c=>c.SellerId,data.SellerId).Set(c=>c.ItemName, data.ItemName).Set(c=>c.Description, data.Description).Set(c=>c.Category, data.Category).Set(c=>c.Valuation, data.Valuation).Set(c=>c.StartingBid, data.StartingBid).Set(c=>c.BuyoutPrice, data.BuyoutPrice).Set(c=>c.ImagePaths, data.ImagePaths);
            return _catalogitems.FindOneAndUpdate(filter,update, new FindOneAndUpdateOptions<CatalogItemDB>{ReturnDocument = ReturnDocument.After});
        }
        public async Task<CatalogItemDB> CreateCatalogItem(CatalogItemDB data)
        {
            //chatgpt virker nok ikke lol
            //skal også kalde imageservice
            var item = data;
            await _catalogitems.InsertOneAsync(data);
            var InsertedID = data.Id;
            return GetById(InsertedID.ToString());
        }
    }
}