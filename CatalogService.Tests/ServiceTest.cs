using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using CatalogService.Services;
using Microsoft.AspNetCore.Mvc;
using CatalogService.Models;
using CatalogService.Controllers;
using Microsoft.AspNetCore.Http;

namespace CatalogService.Tests;

public class ServiceTest
{
    // Attributter til ILogger og IConfuguration
    private ILogger<CatalogController> _logger = null;
    private IConfiguration _configuration = null!;
    
    // Opsætter testmiljøet ved at initialisere _logger og _configuration
    [SetUp]
    public void Setup()
    {
         _logger = new Mock<ILogger<CatalogController>>().Object;

        var myConfiguration = new Dictionary<string, string?>
        {
            {"CatalogServiceBrokerHost", "http://testhost.local"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    // Tester oprettelse af catalog item med succes
    [Test]
    public async Task CreateCatalogItemTest_Succes()
    {
        //Arrange
        var catalogItem = CreateCatalogItem("Stol");
        List<IFormFile> images = new List<IFormFile>();
        bool succes = true;

        var stubServiceCatalog = new Mock<ICatalogDBService>();
        var stubServicePicture = new Mock<PictureService>();

        stubServiceCatalog.Setup(svc => svc.CreateCatalogItem(images, catalogItem))
            .Returns(Task.FromResult<bool>(succes));

        var controller = new CatalogController(_logger,_configuration, stubServiceCatalog.Object, stubServicePicture.Object);

        //Act
        var result = await controller.CreateItem(catalogItem, images);

        //Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>()); 
    }

    // Tester oprettelse af catalog item med fejl
    [Test]
    public async Task CreateCatalogItemTest_Failure()
    {
        //Arrange
        var catalogItem = CreateCatalogItem("Stol");
        List<IFormFile> images = new List<IFormFile>();

        var stubServiceCatalog = new Mock<ICatalogDBService>();
        var stubServicePicture = new Mock<PictureService>();

        stubServiceCatalog.Setup(svc => svc.CreateCatalogItem(images, catalogItem))
            .ThrowsAsync(new Exception());

        var controller = new CatalogController(_logger,_configuration, stubServiceCatalog.Object, stubServicePicture.Object);

        //Act
        var result = await controller.CreateItem(catalogItem, images);

        //Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    // Tester opdatering af endTime med succes
    [Test]
    public async Task UpdateTimeTest_Succes()
    {
        //Arrange
        var timeDTO = CreateTimeDTO("35", new DateTime(2023, 07, 02, 20, 00, 00));
        bool succes = true;
      
        var stubServiceCatalog = new Mock<ICatalogDBService>();
        var stubServicePicture = new Mock<PictureService>();

        stubServiceCatalog.Setup(svc => svc.SetTime(timeDTO))
            .Returns(Task.FromResult<bool>(succes));

        var controller = new CatalogController(_logger,_configuration, stubServiceCatalog.Object, stubServicePicture.Object);

        //Act
        var result = await controller.UpdateTime(timeDTO);

        //Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>()); 
    }

    // Tester opdatering af endTime med fejl - NotFound
    [Test]
    public async Task UpdateTimeTest_Failure_NotFound()
    {
        //Arrange
        var timeDTO = CreateTimeDTO("35", new DateTime(2023, 07, 02, 20, 00, 00));

        var stubServiceCatalog = new Mock<ICatalogDBService>();
        var stubServicePicture = new Mock<PictureService>();

        stubServiceCatalog.Setup(svc => svc.SetTime(timeDTO))
            .ThrowsAsync(new ItemsNotFoundException());

        var controller = new CatalogController(_logger,_configuration, stubServiceCatalog.Object, stubServicePicture.Object);

        //Act
        var result = await controller.UpdateTime(timeDTO);

        //Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>()); 
    }

    // Tester opdatering af endTime med fejl - Exception
    [Test]
    public async Task UpdateTimeTest_Failure_Exception()
    {
        //Arrange
        var timeDTO = CreateTimeDTO("35", new DateTime(2023, 07, 02, 20, 00, 00));

        var stubServiceCatalog = new Mock<ICatalogDBService>();
        var stubServicePicture = new Mock<PictureService>();

        stubServiceCatalog.Setup(svc => svc.SetTime(timeDTO))
            .ThrowsAsync(new Exception());

        var controller = new CatalogController(_logger,_configuration, stubServiceCatalog.Object, stubServicePicture.Object);

        //Act
        var result = await controller.UpdateTime(timeDTO);

        //Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    /// <summary>
    /// Helper method for creating CatalogItem instance.
    /// </summary>
    /// <returns></returns>
    private CatalogItem CreateCatalogItem(string itemName)
    {
        var catalogItem = new CatalogItem("Test SellerId", itemName, "Test Description", "Test Category", 
                                            200, 10, 800, new DateTime(2023, 06, 20), new DateTime(2023, 06, 30));

        return catalogItem;
    }

    /// <summary>
    /// Helper method for creating TimeDTO instance.
    /// </summary>
    /// <returns></returns>
    private TimeDTO CreateTimeDTO(string catalogId, DateTime endTime)
    {
        var timeDTO = new TimeDTO(catalogId, endTime);

        return timeDTO;
    }

}