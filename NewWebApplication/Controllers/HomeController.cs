using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewWebApplication.Models;
using NewWebApplication.Services;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.IO;
using System.Xml;
using System.Globalization;

namespace NewWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _homeService;
        public HomeController(ILogger<HomeController> logger, HomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                
                _logger.LogInformation("HomeController: gets customers details");
                IEnumerable<CustomerModel> customerDetail = await _homeService.getCustomerDetails();
                ViewBag.data = customerDetail;
                return View(customerDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController: Failed to get customers details", ex);
                throw;
            }
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(IFormFile uploadFile)
        {
            try
            {
                _logger.LogInformation("HomeController: Uploading and processing data from xml file");
                await _homeService.uploadXmlFile(uploadFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController: Failed to upload and process data from xml file", ex);
            }
            return RedirectToAction("index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
