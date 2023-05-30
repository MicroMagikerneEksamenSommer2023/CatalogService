using CatalogService.Controllers;
using CatalogService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CatalogService.Services
{
    public interface ICatalogDBService
    {
        Task<bool> CreateCatalogItem(List<IFormFile> pictures, CatalogItem data);
        Task<ImageResponse> DeleteById(string id);
        Task<List<ImageResponse>> GetAllItems();
        Task<List<ImageResponse>> GetByCategory(string category);
        Task<ImageResponse> GetById(string id);
        Task<Wrapper> GetTimeAndPrice(string id);
        Task<bool> SetTime(TimeDTO data);
        Task<ImageResponse> UpdateCatalogItem(List<IFormFile> pictures, CatalogItem data);
        Task<List<Category>> GetFromPushers(string category);
    }

    public class CatalogDBService : ICatalogDBService
    {
        // Attributter
        private readonly ILogger<CatalogDBService> _logger;
        private readonly IMongoCollection<CatalogItemDB> _catalogitems;
        private readonly IConfiguration _config;
        private readonly PictureService picService;
        private readonly string RedisConnection;
        private readonly string GitPusherBase;
        private readonly string GitPusherEndpoint;

        // Constructor
        public CatalogDBService(ILogger<CatalogDBService> logger, IConfiguration config, PictureService picservice)
        {
            _logger = logger;
            _config = config;
            picService = picservice;

            var mongoClient = new MongoClient(_config["connectionsstring"]);
            var database = mongoClient.GetDatabase(_config["database"]);
            _catalogitems = database.GetCollection<CatalogItemDB>(_config["collection"]);
            RedisConnection = _config["redisconnection"];
            GitPusherBase = _config["gitpusherbase"] ?? string.Empty;
            GitPusherEndpoint = _config["gitpusherendpoint"];
        }

        // Henter alle elementer fra databasen og returnerer en liste af ImageResponse-objekter
        public async Task<List<ImageResponse>> GetAllItems()
        {
            List<ImageResponse> result = new List<ImageResponse>();
            var filter = Builders<CatalogItemDB>.Filter.Empty;
            var dbData = (await _catalogitems.FindAsync(filter)).ToList();

            if (dbData.Count == 0)
            {
                throw new ItemsNotFoundException("No items were found in the database.");
            }
            foreach (var item in dbData)
            {
                _logger.LogInformation("found item date:" + item.StartTime);
                List<byte[]> img = picService.ReadPicture(item.ImagePaths);
                var catalogdata = await item.Convert(RedisConnection);
                ImageResponse combined = new ImageResponse(img, catalogdata);
                result.Add(combined);
            }
            return result;
        }

        // Henter et element fra databasen baseret på id og returnerer en ImageResponse
        public async Task<ImageResponse> GetById(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            var dbData = (await _catalogitems.FindAsync(filter)).FirstOrDefault();
            if (dbData == null)
            {
                throw new ItemsNotFoundException($"No item with ID {id} was found in the database.");
            }
            var img = picService.ReadPicture(dbData.ImagePaths);
            var catalogdata = await dbData.Convert(RedisConnection);
            var combined = new ImageResponse(img, catalogdata);
            return combined;
        }

        // Henter elementer fra databasen baseret på kategori og returnerer en liste af ImageResponse-objekter
        public async Task<List<ImageResponse>> GetByCategory(string category)
        {
            List<ImageResponse> result = new List<ImageResponse>();
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Category, category);
            var dbData = (await _catalogitems.FindAsync(filter)).ToList();
            if (dbData.Count == 0)
            {
                throw new ItemsNotFoundException($"No items found in the database for category '{category}'.");
            }
            foreach (var item in dbData)
            {
                List<byte[]> img = picService.ReadPicture(item.ImagePaths);
                var catalogdata = await item.Convert(RedisConnection);
                ImageResponse combined = new ImageResponse(img, catalogdata);
                result.Add(combined);
            }
            return result;
        }

        // Interoperabilitet med Git Pushers

        public async Task<List<Category>> GetFromPushers(string category)
        {
            List<Category> result = new List<Category>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(GitPusherBase);
                HttpResponseMessage response = await client.GetAsync(GitPusherEndpoint + "/" + category);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<Category>>(responseData);
                    return result;
                }
                else
                { throw new ItemsNotFoundException("Kunne ikke finde noget i databasen hos GitPushers"); }
            }
        }

        // Sletter et element fra databasen baseret på id og returnerer en ImageResponse for det slettede element
        public async Task<ImageResponse> DeleteById(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            var dbData = await _catalogitems.FindOneAndDeleteAsync(filter);
            if (dbData == null)
            {
                throw new ItemsNotFoundException($"No item with ID {id} was found in the database for deletion.");
            }
            List<byte[]> img = picService.ReadAndDeletePictures(dbData.ImagePaths);
            var catalogdata = await dbData.Convert(RedisConnection);
            ImageResponse combined = new ImageResponse(img, catalogdata);
            return combined;
        }

        // Opdaterer et katalogelement i databasen og returnerer en ImageResponse for det opdaterede element
        public async Task<ImageResponse> UpdateCatalogItem(List<IFormFile> pictures, CatalogItem data)
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
            var update = Builders<CatalogItemDB>.Update.Set(c => c.SellerId, data.SellerId.ToLower()).Set(c => c.ItemName, data.ItemName).Set(c => c.Description, data.Description).Set(c => c.Category, data.Category).Set(c => c.Valuation, data.Valuation).Set(c => c.StartingBid, data.StartingBid).Set(c => c.BuyoutPrice, data.BuyoutPrice).Set(c => c.ImagePaths, newPaths).Set(c => c.StartTime, data.StartTime).Set(c => c.EndTime, data.EndTime);
            CatalogItemDB dbData = await _catalogitems.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<CatalogItemDB> { ReturnDocument = ReturnDocument.After });
            return new ImageResponse(picService.ReadPicture(newPaths), await dbData.Convert(RedisConnection));
        }

        // Opretter et nyt katalogelement i databasen og returnerer en bool-værdi, der angiver om oprettelsen var vellykket
        public async Task<bool> CreateCatalogItem(List<IFormFile> pictures, CatalogItem data)
        {
            _logger.LogInformation("create starttime: " + data.StartTime);
            List<string> paths = await picService.SavePicture(pictures);
            CatalogItemDB item = data.Convert(paths);
            item.SellerId = item.SellerId.ToLower();
            try
            {
                await _catalogitems.InsertOneAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to insert document: " + ex.Message);
            }
            //burde kunne return noget, men det har mongodb driver ikke
        }

        // Henter starttid, sluttid, startbud og købspris for et element baseret på id og returnerer en Wrapper
        public async Task<Wrapper> GetTimeAndPrice(string id)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, id);
            var dbData = (await _catalogitems.FindAsync(filter)).FirstOrDefault();
            if (dbData == null)
            {
                throw new ItemsNotFoundException($"No item with ID {id} was found in the database.");
            }
            Wrapper newWrapper = new Wrapper(dbData.StartTime, dbData.EndTime, dbData.StartingBid, dbData.BuyoutPrice);
            return newWrapper;
        }

        // Opdaterer sluttidspunktet for et katalogelement baseret på dets ID og returnerer en bool-værdi, der angiver om opdateringen var vellykket
        public async Task<bool> SetTime(TimeDTO data)
        {
            var filter = Builders<CatalogItemDB>.Filter.Eq(c => c.Id, data.CatalogId);
            var itemToUpdate = _catalogitems.Find(filter).FirstOrDefault();
            if (itemToUpdate == null)
            {
                throw new ItemsNotFoundException($"No item with ID {data.CatalogId} was found in the database for the update.");
            }
            var update = Builders<CatalogItemDB>.Update.Set(c => c.EndTime, data.EndTime);
            CatalogItemDB dbData = await _catalogitems.FindOneAndUpdateAsync(filter, update);
            return true;
        }
    }
}