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
public class CatalogController : ControllerBase
{


    private readonly ILogger<CatalogController> _logger;

    private readonly ICatalogDBService dBService;

    private readonly PictureService picService;

    public CatalogController(ILogger<CatalogController> logger, IConfiguration configuration, ICatalogDBService dbservice, PictureService picservice)
    {
        _logger = logger;
        dBService = dbservice;
        picService = picservice;
    }

     [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        
        try
        {
            var response = await dBService.GetAllItems();
            return Ok(response);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
         catch (Exception ex)
        {
            
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message });
        }
       
    }

    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById([FromRoute]string id)
    {
        try
        {
            var item = await dBService.GetById(id);
            return Ok(item);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }

    [HttpGet("getbycategory/{category}")]
    public async Task<IActionResult> GetByCategory([FromRoute]string category)
    {
        try
        {
            var items = await dBService.GetByCategory(category);
            return Ok(items);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }

    [HttpDelete("deletebyid/{id}")]
    public async Task<IActionResult> DeleteById([FromRoute]string id)
    {
        try
        {
            var item = await dBService.DeleteById(id);
            return Ok(item);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }

   [HttpPut("updateitem")]
    public async Task<IActionResult> UdpdateItem([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data,List<IFormFile> images)
    {
        try{
        var item = await dBService.UpdateCatalogItem(images,data);
        return Ok(item);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }

    }
    [HttpPost("createitem")]
    public async Task<IActionResult> CreateItem([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data,List<IFormFile> images)
    {
        //burde return noget, men kan ikke fetche id
         try
    {
        bool insertedStatus = await dBService.CreateCatalogItem(images, data);
        return Ok(new { message = "Catalog item created successfully.", insertedStatus });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Failed to create catalog item." });
    }
      
    }
    [HttpPut("updatetime")]
    public async Task<IActionResult> UdpdateTime([FromBody]TimeDTO data) {
        try{
        var item = await dBService.SetTime(data);
        return Ok(item);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }
    [HttpGet("getitemandprice/{id}")]
    public async Task<IActionResult> GetTimeAndPrice([FromRoute]string id)
    {
        try
        {
            var items = await dBService.GetTimeAndPrice(id);
            return Ok(items);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }

    [HttpGet("getfrompushers/{categoryid}")]
    public async Task<IActionResult> GetFromPushers([FromRoute]string categoryid)
    {
        try
        {
            var items = await dBService.GetFromPushers(categoryid);
            return Ok(items);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }

    [HttpGet("privatepusherapi/{category}")]
    public async Task<IActionResult> SendToPushers([FromRoute]string category)
    {
        try
        {
            var items = await dBService.GetByCategory(category);
            List<Category> templist = new List<Category>();
            foreach(var item in items)
            {
                templist.Add(new Category(item.AdditionalData.Category,item.AdditionalData.Category,item.AdditionalData.Description,item.AdditionalData.StartTime));
            }
            return Ok(templist);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle other exceptions or unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred."+ ex.Message });
        }
    }

}
