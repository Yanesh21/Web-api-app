﻿using System;
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
using Microsoft.AspNetCore.Authorization;

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

        /// <summary>
        /// Gets all customers details
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets customer details by id
        /// </summary>
        /// <param name="customer_id">customer id</param>
        /// <returns></returns>
        [HttpPost("index")]
        public async Task<IActionResult> GetCustomerDetailsById(string customer_id)
        {
            try
            {
                _logger.LogInformation("HomeController: gets customers details");
                CustomerModel customerDetail = await _homeService.getCustomerDetailsById(customer_id);
                customerDetail.balance = customerDetail.Num_Shares * customerDetail.Share_Price;
                ViewBag.data = customerDetail;

                return View(customerDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController: Failed to get customers details", ex);
                throw;
            }
        }

        /// <summary>
        /// uploads and processes data from xml file
        /// </summary>
        /// <param name="uploadFile">formfile object</param>
        /// <returns></returns>
        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult UploadFile(IFormFile uploadFile)
        {
            try
            {
                _logger.LogInformation("HomeController: Uploading and processing data from xml file");
                _homeService.uploadXmlFile(uploadFile);
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController: Failed to upload and process data from xml file", ex);
            }
            return RedirectToAction("index");
        }

        /// <summary>
        /// Updates customer details by id
        /// </summary>
        /// <param name="customerDetail">customer detail object</param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task<IActionResult> GetCustomerDetailsById([FromForm] CustomerModel customerDetail)
        {
            try
            {
                _logger.LogInformation("HomeController: Updates customer detail");
                await _homeService.updateCustomerDetail(customerDetail);

                return RedirectToAction("getcustomerdetailsbyid");
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController: Failed to update customer detail", ex);
                throw;
            }
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
