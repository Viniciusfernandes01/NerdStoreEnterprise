using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class CatalogController : MainController
    {
        private readonly ICatalogServiceRefit _catalogService;

        public CatalogController(ICatalogServiceRefit catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> index()
        {
            var products = await _catalogService.GetAll();
            return View(products);
        }

        [HttpGet]
        [Route("product-detail/{id}")]
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var product = await _catalogService.GetProductId(id);
            return View(product);
        }


    }
}
