using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Models;
using Microsoft.Extensions.Logging;

namespace dotnet_code_challenge.Services
{
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

        public IFileFeedParser GetFileProcessor(string xmlFilePath)
        {
            throw new NotImplementedException();
        }

        public ICollection<Horse> GetAllHorses(string supportfiles)
        {
            throw new NotImplementedException();
        }
    }
}