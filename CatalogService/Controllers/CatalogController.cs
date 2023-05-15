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

    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById([FromRoute]string id)
    {
        ImageResponse response = await dBService.GetById(id);
        return Ok(response);
    }

    [HttpGet("getbyid/{category}")]
    public async Task<IActionResult> GetByCategory([FromRoute]string category)
    {
        List<ImageResponse> response = await dBService.GetByCategory(category);
        return Ok(response);
    }

    [HttpDelete("deletebyid/{id}")]
    public async Task<IActionResult> DeleteById([FromRoute]string id)
    {
        ImageResponse response = await dBService.DeleteById(id);
        return Ok(response);
    }

   [HttpPut("updatecustomer")]
    public async Task<IActionResult> UdpdateCustomer([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data,List<IFormFile> images)
    {
        //tester
        ImageResponse response = await dBService.UpdateCatalogItem(images,data);
        return Ok(response);

    }
    [HttpPost("createitem")]
    public async Task<IActionResult> CreateCustomer([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data,List<IFormFile> images)
    {
        //burde return noget, men kan ikke fetche id
        dBService.CreateCatalogItem(images,data);
        return Ok();
      
    }
}
;
