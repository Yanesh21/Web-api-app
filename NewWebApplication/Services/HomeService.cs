﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewWebApplication.DBContext;
using NewWebApplication.Models;

namespace NewWebApplication.Services
{
    public class HomeService
    {

        private CustomerModel customerDetails;
        private readonly ILogger<HomeService> _logger;
        private readonly DefaultDbContext _dbContext;

        public HomeService(ILogger<HomeService> logger, DefaultDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task uploadXmlFile(IFormFile uploadFile)
        {
            _logger.LogInformation("HomeService: Uploading and processing data from xml file");
            if (uploadFile.Length > 0)
            {
                List<CustomerModel> customerList = new List<CustomerModel>();
                string path = Environment.CurrentDirectory + "\\upload\\";
                string filePath = Path.Combine(path, uploadFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadFile.CopyTo(stream);
                }

                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            List<string> skipFields = new List<string> { "RequestDoc", "Doc_Date", "Doc_Data", "Doc_Ref", "DataItem_Customer", "Mailing_Address", "Contact_Details", "Shares_Details" };
                            string label = reader.Name.ToString();
                            if (label == "DataItem_Customer")
                            {
                                if (customerDetails != null)
                                {
                                    customerList.Add(customerDetails);
                                }
                                customerDetails = new CustomerModel();
                            }

                            if (!skipFields.Contains(label))
                            {
                                DateTime dateOfBirth = new DateTime();
                                foreach (PropertyInfo prop in customerDetails.GetType().GetProperties())
                                {
                                    if (prop.Name == label)
                                    {
                                        try
                                        {
                                            if (label == "Date_Of_Birth")
                                            {
                                                string date = reader.ReadString();
                                                dateOfBirth = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                                int age = DateTime.Today.Year - dateOfBirth.Year;
                                                if (age < 18)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    prop.SetValue(customerDetails, dateOfBirth);
                                                }
                                            }
                                            else if (label == "Num_Shares")
                                            {
                                                try
                                                {
                                                    int numShare = int.Parse(reader.ReadString());
                                                    if (numShare == 0)
                                                    {
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        prop.SetValue(customerDetails, numShare);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    //log exception
                                                    _logger.LogError(label + "cannot be cast as integer", ex);
                                                    continue;
                                                }
                                            }
                                            else if (label == "Share_Price")
                                            {
                                                decimal decimalValue = 0;
                                                string sharePrice = reader.ReadString();
                                                int convertedSharePrice = 0;
                                                try
                                                {
                                                    convertedSharePrice = int.Parse(sharePrice);
                                                    if (convertedSharePrice == 0)
                                                    {
                                                        continue;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    if (Decimal.TryParse(sharePrice, out decimalValue))
                                                    {
                                                        prop.SetValue(customerDetails, decimalValue);
                                                        continue;
                                                    }
                                                }
                                            }
                                            else if (label == "Date_Incorp")
                                            {
                                                string dateIncorp = reader.ReadString();
                                                if (!String.IsNullOrEmpty(dateIncorp))
                                                {
                                                    prop.SetValue(customerDetails, DateTime.ParseExact(dateIncorp, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                }
                                                else
                                                {
                                                    customerDetails.Date_Incorp = null;
                                                }
                                            }
                                            else
                                            {
                                                Type type = prop.PropertyType;
                                                prop.SetValue(customerDetails, reader.ReadElementContentAs(type, null));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //log error
                                            _logger.LogError("Failed to read xml file", ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (CustomerModel customerDetail in customerList)
                {
                    if (customerDetail.Date_Of_Birth != null && customerDetail.Num_Shares > 0 && customerDetail.Share_Price > 0)
                    {
                        _dbContext.Customer.Add(customerDetail);
                        _dbContext.SaveChanges();
                    }
                }
            }
        }

        public async Task<IEnumerable<CustomerModel>> getCustomerDetails()
        {
            try
            {
                _logger.LogInformation("HomeService: getting customers details");

                IEnumerable<CustomerModel> customerDetailList = await _dbContext.Customer.ToListAsync();
                return customerDetailList;
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeService: failed to get customers details", ex);
                throw;
            }
        }
    }
}