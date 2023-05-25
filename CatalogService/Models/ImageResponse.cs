using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CatalogService.Models
{

public class ImageResponse
{
    public List<byte[]> FileBytes { get; set; }
    public ItemWithBid AdditionalData { get; set; }

    public ImageResponse(List<byte[]> img, ItemWithBid data)
    {
        this.FileBytes = img;
        this.AdditionalData = data;

    }
  
}
}