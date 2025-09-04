using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.DataAccess; //allows compiler to use types from DataAccess
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IDataAccess<Product> _dataAccess; //holds instance of dataAccess layer 

    public ProductController(ILogger<ProductController> logger, IDataAccess<Product> dataAccess) //second param added to allow service for IDataAccess to be found
    {
        _logger = logger;
        _dataAccess = dataAccess; //assign dataAccess instance to previously declared field
    }

    [HttpGet]
    public IEnumerable<object> Get(int? pageStart, int? pageSize, string currency = "GBP") //get method changed to object since two types of collections are being returned.
    {
        const decimal exchangeRate = 1.11m; //defined euro exchange rate

        var products = _dataAccess.List(pageStart, pageSize); //retrieve the initial list of products

        if (currency.ToLower() == "eur") //checks to see if the currency selected is euro and performs logic to convert
        {
            return products.Select(p => new ProductInEuros
            {
                Name = p.Name,
                PriceInEuros = p.PriceInPounds * exchangeRate // converts price from pounds to euro using exchange rate defined
            });
        }
        
        return products; //fallback to return old list if necessary
    }
}