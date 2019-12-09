using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotnet_code_challenge.Services.FileProcessors
{
    /// <summary>
    /// Json file parser to extract horse details from race data
    /// For this test, this JSON parser is set to only read one type of file
    /// </summary>
    public class JsonFileFeedParser: IFileFeedParser
    {
        private ILogger<JsonFileFeedParser> _logger;

        public JsonFileFeedParser(ILogger<JsonFileFeedParser> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get Race details from json file
        /// </summary>
        /// <param name="fullFilePath">Full file path to the file</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public ICollection<Race> ProcessFileAndGetRaceInfo(string fullFilePath)
        {
            _logger.LogInformation($"ProcessFileAndGetRaceInfo is called with {fullFilePath}");
            
            if (string.IsNullOrEmpty(fullFilePath))
            {
                _logger.LogError("Full file path is empty");
                throw new ArgumentNullException();
            }

            if (!File.Exists(fullFilePath))
            {
                _logger.LogError("File does not exist");
                throw new FileNotFoundException();
            }


            
            var fileContent = File.ReadAllText(fullFilePath);
            
            _logger.LogInformation("parse JSON file context");
            dynamic jToken = JToken.Parse(fileContent);

            _logger.LogInformation("Select race details");
            var races = ((JArray)jToken.RawData.Markets).Select((dynamic x) => new Race
            {
                Name =  jToken.RawData.FixtureName,
                StartTimeUTC = Convert.ToDateTime(jToken.RawData.StartTime.ToString()),
                NumberOfRunners = jToken.RawData.Participants.Count,
                Horses = ((JArray)x.Selections).Select((dynamic h) => new Horse
                {
                    Id = h.Id,
                    Number = h.Tags.participant,
                    Name = h.Tags.name,
                    Price = h.Price
                }).ToList()
            }).ToList<Race>();

            _logger.LogInformation($"return race details. {JsonConvert.SerializeObject(races)}");
            return races;
        }
    }
}