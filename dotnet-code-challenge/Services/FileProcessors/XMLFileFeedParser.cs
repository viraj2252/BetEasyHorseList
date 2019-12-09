using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dotnet_code_challenge.Services.FileProcessors
{
    /// <summary>
    /// XML file parser to extract horse details from race data
    /// For this test, this XML parser is set to only read one type of file
    /// </summary>
    public class XMLFileFeedParser: IFileFeedParser
    {
        private ILogger<XMLFileFeedParser> _logger;

        public XMLFileFeedParser(ILogger<XMLFileFeedParser> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get race details from xml file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public ICollection<Race> ProcessFileAndGetRaceInfo(string fullFilePath)
        {
            _logger.LogInformation($"ProcessFileAndGetRaceInfo is called with {fullFilePath}");
            if (string.IsNullOrEmpty(fullFilePath))
            {
                _logger.LogError("Full file path does not exist");
                throw new ArgumentNullException();
            }

            if (!File.Exists(fullFilePath))
            {
                _logger.LogError("File does not exist");
                throw new FileNotFoundException();
            }
            
            _logger.LogInformation("load XML file context");
            var xml = XElement.Load(fullFilePath);

            
            _logger.LogInformation("Select race details from XML");
            var result = xml.Descendants("race").Select( r =>  new Race
                {
                    Name = r.Attribute("name")?.Value,
                    StartTimeUTC = DateTime.ParseExact(xml.Element("date")?.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    NumberOfRunners = r.Descendants("horses").Count(),
                    Horses = r.Element("horses")?.Descendants("horse").Select(h => new Horse
                    {
                        Id = h.Attribute("id")?.ToString(),
                        Number = Convert.ToInt32(h.Element("number")?.Value.ToString()),
                        Name = h.Attribute("name")?.Value.ToString(),
                        Price = Convert.ToDecimal(r.Element("prices")?.Descendants("horse").First(p => p.Attribute("number")?.Value.ToString() == h.Element("number")?.Value.ToString()).Attribute("Price")?.Value.ToString())
                    }).ToList()
                }).ToList();
                

            _logger.LogInformation($"return race details. {JsonConvert.SerializeObject(result)}");
            return result;
        }
        
    }
}