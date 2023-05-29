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
    // Attributter
    private readonly ILogger<CatalogController> _logger;
    private readonly ICatalogDBService dBService;
    private readonly PictureService picService;

    // Constructor
    public CatalogController(ILogger<CatalogController> logger, IConfiguration configuration, ICatalogDBService dbservice, PictureService picservice)
    {
        _logger = logger;
        dBService = dbservice;
        picService = picservice;
    }

    // Henter alle elementer fra databasen
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

    // Henter et element fra databasen baseret på id
    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
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
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message }); // Handle other exceptions or unexpected errors
        }
    }

    // Henter elementer fra databasen baseret på kategori
    [HttpGet("getbycategory/{category}")]
    public async Task<IActionResult> GetByCategory([FromRoute] string category)
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
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message }); // Handle other exceptions or unexpected errors
        }
    }

    // Sletter et element fra databasen baseret på id
    [HttpDelete("deletebyid/{id}")]
    public async Task<IActionResult> DeleteById([FromRoute] string id)
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
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message }); // Handle other exceptions or unexpected errors
        }
    }

    // Opdaterer et elemet i databasen
    [HttpPut("updateitem")]
    public async Task<IActionResult> UpdateItem([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data, List<IFormFile> images)
    {
        try
        {
            var item = await dBService.UpdateCatalogItem(images, data);
            return Ok(item);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message }); // Handle other exceptions or unexpected errors
        }
    }

    // Opretter et element i databasen
    [HttpPost("createitem")]
    public async Task<IActionResult> CreateItem([ModelBinder(BinderType = typeof(JsonModelBinder))] CatalogItem data, List<IFormFile> images)
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

    // Opdaterer et sluttidspunkt i databasen
    [HttpPut("updatetime")]
    public async Task<IActionResult> UpdateTime([FromBody] TimeDTO data)
    {
        try
        {
            var item = await dBService.SetTime(data);
            return Ok(item);
        }
        catch (ItemsNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message }); // Handle other exceptions or unexpected errors
        }
    }

    // Henter starttid, sluttid, startbud og købspris for et element baseret på id
    [HttpGet("getitemandprice/{id}")]
    public async Task<IActionResult> GetTimeAndPrice([FromRoute] string id)
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
            return StatusCode(500, new { error = "An unexpected error occurred." + ex.Message }); // Handle other exceptions or unexpected errors
        }
    }
}
