using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Models;
using Microsoft.Extensions.Logging;

namespace dotnet_code_challenge.Services
{
    /// <summary>
    /// Manager class which will go through all files in a directory
    /// and get all horse details
    /// </summary>
    public class FileProcessManager
    {
        public const string XmlFileType = "xml";
        public const string JSONFileType = "json";
        private readonly Func<string, IFileFeedParser> _serviceAccessor;
        private ILogger<FileProcessManager> _logger;

        public FileProcessManager(Func<string, IFileFeedParser> serviceAccessor, ILogger<FileProcessManager> logger)
        {
            _serviceAccessor = serviceAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Get relevant feeder object by file extension
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public IFileFeedParser GetFileProcessor(string filePath)
        {
            _logger.LogInformation($"GetFileProcessor called with {filePath}");
            var fi = new FileInfo(filePath);

            if (fi == null || !fi.Exists)
            {
                _logger.LogError($"File does not exist");
                throw new FileNotFoundException();
            }

            var fileExt = fi.Extension.ToLower().Split('.').Last();
            return _serviceAccessor(fileExt);
        }

        /// <summary>
        /// Extract all race details then isolate horse data
        /// order by price
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public ICollection<Horse> GetAllHorses(string dirPath)
        {
            _logger.LogInformation($"GetAllHorses called with {dirPath}");
            DirectoryInfo di = new DirectoryInfo(dirPath);
            List<Horse> horses = new List<Horse>();

            if (di == null || !di.Exists)
            {
                _logger.LogError($"No directory exists with {dirPath}");
                throw new DirectoryNotFoundException();
            }

            foreach (var fi in di.GetFiles())
            {
                var feedParser = GetFileProcessor(fi.FullName);
                var races = feedParser.ProcessFileAndGetRaceInfo(fi.FullName);

                foreach (var race in races)
                {
                    horses.AddRange(race.Horses);
                }
                
            }

            return horses.OrderBy(h => h.Price).ToList();
        }
    }
}