using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CatalogService.Models
{

public class ImageResponse
{
    public List<byte[]> FileBytes { get; set; }
    public CatalogItem AdditionalData { get; set; }

    public ImageResponse(List<byte[]> img, CatalogItem data)
    {
        this.FileBytes = img;
        this.AdditionalData = data;

    }
  
}
}