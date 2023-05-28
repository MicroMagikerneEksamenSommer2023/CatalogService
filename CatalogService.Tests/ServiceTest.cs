using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private ILogger<CatalogController> _logger = null;
    private IConfiguration _configuration = null!;
    
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
}