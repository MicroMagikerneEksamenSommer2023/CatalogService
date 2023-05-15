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

        private readonly PictureService picService;

       public CatalogDBService(ILogger<CatalogDBService> logger, IConfiguration config, PictureService picservice)
        {
            _logger = logger;
            _config = config;
            picService = picservice;

            var mongoClient = new MongoClient(_config["connectionsstring"]);
            var database = mongoClient.GetDatabase(_config["database"]);
            _catalogitems = database.GetCollection<CatalogItemDB>(_config["collection"]);
        }
        public async Task<List<ImageResponse>> GetAllItems()
        {
            List<ImageResponse> result = new List<ImageResponse>();
            
            var filter = Builders<CatalogItemDB>.Filter.Empty;
            List<CatalogItemDB> dbData = (await _catalogitems.FindAsync(filter)).ToList();
            foreach(var item in dbData)
            {
                List<byte[]> img = picService.ReadPicture(item.ImagePaths);
                CatalogItem catalogdata = item.Convert();
                ImageResponse combined = new ImageResponse(img,catalogdata);
                result.Add(combined);
            }
            return result;


        }
        public async Task<ImageResponse> GetById(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            CatalogItemDB dbData = (await _catalogitems.FindAsync(filter)).FirstOrDefault();
            List<byte[]> img = picService.ReadPicture(dbData.ImagePaths);
            CatalogItem catalogdata = dbData.Convert();
            ImageResponse combined = new ImageResponse(img,catalogdata);
            return combined;
        }
        public async Task<List<ImageResponse>> GetByCategory(string category)
        {
            List<ImageResponse> result = new List<ImageResponse>();
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Category, category);
            List<CatalogItemDB> dbData = (await _catalogitems.FindAsync(filter)).ToList();
            foreach(var item in dbData)
            {
                List<byte[]> img = picService.ReadPicture(item.ImagePaths);
                CatalogItem catalogdata = item.Convert();
                ImageResponse combined = new ImageResponse(img,catalogdata);
                result.Add(combined);
            }
            return result;
        }
          public async Task<ImageResponse> DeleteById(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            CatalogItemDB dbData = await _catalogitems.FindOneAndDeleteAsync(filter);
            List<byte[]> img = picService.ReadAndDeletePictures(dbData.ImagePaths);
            CatalogItem catalogdata = dbData.Convert();
            ImageResponse combined = new ImageResponse(img,catalogdata);
            return combined;
        }
        public async Task<ImageResponse> UpdateCatalogItem(List<IFormFile> pictures ,CatalogItem data)
        {
            //Burde kunne skrives bedre, men det kræver lige lidt hjerne...
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, data.Id);
            CatalogItemDB itemToUpdate = _catalogitems.Find(filter).FirstOrDefault();
            var deletedPics = picService.ReadAndDeletePictures(itemToUpdate.ImagePaths);
            List<string> newPaths = await picService.SavePicture(pictures);
            var update = Builders<CatalogItemDB>.Update.Set(c=>c.SellerId,data.SellerId).Set(c=>c.ItemName, data.ItemName).Set(c=>c.Description, data.Description).Set(c=>c.Category, data.Category).Set(c=>c.Valuation, data.Valuation).Set(c=>c.StartingBid, data.StartingBid).Set(c=>c.BuyoutPrice, data.BuyoutPrice).Set(c=>c.ImagePaths, newPaths);
            CatalogItemDB dbData = await _catalogitems.FindOneAndUpdateAsync(filter,update, new FindOneAndUpdateOptions<CatalogItemDB>{ReturnDocument = ReturnDocument.Before});
            return new ImageResponse(picService.ReadPicture(newPaths),dbData.Convert());
        }
        public async void CreateCatalogItem(List<IFormFile> pictures ,CatalogItem data)
        {
            List<string> paths = await picService.SavePicture(pictures);
            CatalogItemDB item = data.Convert(paths);
            await _catalogitems.InsertOneAsync(item);
            //burde kunne return noget, men det har mongodb driver ikke
        }
    }
}