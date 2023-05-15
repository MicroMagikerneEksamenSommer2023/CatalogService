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
            var dbData = (await _catalogitems.FindAsync(filter)).ToList();

            if (dbData.Count == 0)
            {
                throw new ItemsNotFoundException("No items were found in the database.");
            }
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
            var dbData = (await _catalogitems.FindAsync(filter)).FirstOrDefault();
              if (dbData == null)
            {
                throw new ItemsNotFoundException($"No item with ID {id} was found in the database.");
            }
            var img = picService.ReadPicture(dbData.ImagePaths);
            var catalogdata = dbData.Convert();
            var combined = new ImageResponse(img,catalogdata);
            return combined;
        }
        public async Task<List<ImageResponse>> GetByCategory(string category)
        {
            List<ImageResponse> result = new List<ImageResponse>();
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Category, category);
            var dbData = (await _catalogitems.FindAsync(filter)).ToList();
            if (dbData.Count == 0)
            {
                throw new ItemsNotFoundException($"No items found in the database for category '{category}'.");
            }
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
            var dbData = await _catalogitems.FindOneAndDeleteAsync(filter);
             if (dbData == null)
            {
                throw new ItemsNotFoundException($"No item with ID {id} was found in the database for deletion.");
            }
            List<byte[]> img = picService.ReadAndDeletePictures(dbData.ImagePaths);
            CatalogItem catalogdata = dbData.Convert();
            ImageResponse combined = new ImageResponse(img,catalogdata);
            return combined;
        }
        public async Task<ImageResponse> UpdateCatalogItem(List<IFormFile> pictures ,CatalogItem data)
        {
            //Burde kunne skrives bedre, men det kræver lige lidt hjerne...
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, data.Id);
            var itemToUpdate = _catalogitems.Find(filter).FirstOrDefault();
             if (itemToUpdate == null)
            {
                throw new ItemsNotFoundException($"No item with ID {data.Id} was found in the database for the update.");
            }
            var deletedPics = picService.ReadAndDeletePictures(itemToUpdate.ImagePaths);
            List<string> newPaths = await picService.SavePicture(pictures);
            var update = Builders<CatalogItemDB>.Update.Set(c=>c.SellerId,data.SellerId).Set(c=>c.ItemName, data.ItemName).Set(c=>c.Description, data.Description).Set(c=>c.Category, data.Category).Set(c=>c.Valuation, data.Valuation).Set(c=>c.StartingBid, data.StartingBid).Set(c=>c.BuyoutPrice, data.BuyoutPrice).Set(c=>c.ImagePaths, newPaths);
            CatalogItemDB dbData = await _catalogitems.FindOneAndUpdateAsync(filter,update, new FindOneAndUpdateOptions<CatalogItemDB>{ReturnDocument = ReturnDocument.Before});
            return new ImageResponse(picService.ReadPicture(newPaths),dbData.Convert());
        }
        public async Task<bool> CreateCatalogItem(List<IFormFile> pictures ,CatalogItem data)
        {
            List<string> paths = await picService.SavePicture(pictures);
            CatalogItemDB item = data.Convert(paths);
            try
            {
                await _catalogitems.InsertOneAsync(item);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to insert document: " + ex.Message);
            }
            //burde kunne return noget, men det har mongodb driver ikke
        }
    }
}