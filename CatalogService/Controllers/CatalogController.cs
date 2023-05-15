using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CatalogService.Services;
using MongoDB.Bson;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
//using System.Web.Http;

namespace CatalogService.Controllers;


[ApiController]
[Route("catalogservice/v1")]
public class CustomerController : ControllerBase
{


    private readonly ILogger<CustomerController> _logger;

    private readonly CatalogDBService dBService;

    private readonly PictureService picService;

    public CustomerController(ILogger<CustomerController> logger, CatalogDBService dbservice, PictureService picservice)
    {
        _logger = logger;
        dBService = dbservice;
        picService = picservice;
    }

     [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        List<ImageResponse> response = await dBService.GetAllItems();
        return Ok(response);
    }

    [HttpDelete("deletebyid/{id}")]
    public async Task<IActionResult> DeleteById([FromRoute]string id)
    {
        ImageResponse response = await dBService.DeleteById(id);
        return Ok(response);
    }

    [HttpPost("createitem")]
    public async Task<IActionResult> CreateCustomer([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data,List<IFormFile> images)
    {
        _logger.LogInformation("Call revieved, " + data.ToString());
        dBService.CreateCatalogItem(images,data);
        return Ok();
      
    }
}
;
