using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnet_code_challenge.Helpers;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Models;
using dotnet_code_challenge.Services;
using dotnet_code_challenge.Services.FileProcessors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace dotnet_code_challenge.Test
{
    public class RaceUnitTest
    {
        private readonly Func<string, IFileFeedParser> _serviceAccessor;
        private string _xmlFilePath = "./SupportFiles/Caulfield_Race1.xml";
        private string _jsonFilePath = "./SupportFiles/Wolferhampton_Race1.json";
        private readonly IServiceProvider _serviceProvider;

        public RaceUnitTest()
        {
            var mockConfig = new Mock<IConfiguration>();
            _serviceProvider = DIHelper.BuildDi(mockConfig.Object);
            _serviceAccessor = _serviceProvider.GetRequiredService<Func<string, IFileFeedParser>>();
        }
        
        [Fact]
        public void di_build_output_should_not_be_null()
        {
           Assert.NotNull(_serviceProvider);
        }

        [Fact]
        public void di_should_be_able_get_xml_parser()
        {
            var fileParser = _serviceAccessor("xml");
            
            Assert.NotNull(fileParser);
            Assert.Equal(typeof(XMLFileFeedParser), fileParser.GetType());
        }
        
        [Fact]
        public void di_should_be_able_get_json_parser()
        {
            var fileParser = _serviceAccessor("json");
            
            Assert.NotNull(fileParser);
            Assert.Equal(typeof(JsonFileFeedParser), fileParser.GetType());
        }

        [Fact]
        public void di_throws_KeyNotFoundException_if_type_is_txt()
        {
            Assert.Throws<KeyNotFoundException>(() => _serviceAccessor("txt"));
        }

        [Fact]
        public void di_get_xml_file_parser_for_file_extension_xml()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_xmlFilePath);
            
            Assert.NotNull(fileProcessor);
            Assert.StrictEqual(typeof(XMLFileFeedParser), fileProcessor.GetType());
        }
        
        [Fact]
        public void di_get_json_file_parser_for_file_extension_json()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_jsonFilePath);
            
            Assert.NotNull(fileProcessor);
            Assert.StrictEqual(typeof(JsonFileFeedParser), fileProcessor.GetType());
        }
        
        [Fact]
        public void FileProcessManager_GetAllHorses_should_fire_exception_for_invalid_dir()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            Assert.Throws<DirectoryNotFoundException>(() => fileProcessManager.GetAllHorses("./InvalidDIR"));
        }
        
        [Fact]
        public void FileProcessManager_GetFileProcessor_should_fire_exception_for_invalid_file()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            Assert.Throws<FileNotFoundException>(() => fileProcessManager.GetFileProcessor("./invalidfile.json"));
        }
        
        [Fact]
        public void IFileFeedParser_for_xml_should_fire_exception_for_empty_file_string()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_xmlFilePath);
            Assert.Throws<ArgumentNullException>(() => fileProcessor.ProcessFileAndGetRaceInfo(fullFilePath: ""));
        }
        
        [Fact]
        public void IFileFeedParser_for_json_should_fire_exception_for_empty_file_string()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_jsonFilePath);
            Assert.Throws<ArgumentNullException>(() => fileProcessor.ProcessFileAndGetRaceInfo(fullFilePath: ""));
        }
        
        [Fact]
        public void IFileFeedParser_for_xml_should_fire_exception_for_wrong_file_path()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_xmlFilePath);
            Assert.Throws<FileNotFoundException>(() => fileProcessor.ProcessFileAndGetRaceInfo(fullFilePath: "./wrongFIle.xml"));
        }
        
        [Fact]
        public void IFileFeedParser_for_json_should_fire_exception_for_wrong_file_path()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_jsonFilePath);
            Assert.Throws<FileNotFoundException>(() => fileProcessor.ProcessFileAndGetRaceInfo(fullFilePath: "./wrongFIle.json"));
        }
        
        [Fact]
        public void IFileFeedParser_for_xml_should_return_valid_race_object_with_2_horses()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_xmlFilePath);
            FileInfo fi = new FileInfo(_xmlFilePath);
            ICollection<Race> races = fileProcessor.ProcessFileAndGetRaceInfo(fullFilePath: fi.FullName);

            Assert.NotNull(races);
            Assert.Equal(1, races.Count);
            Assert.Equal(2, races.First().Horses.Count);
        }
        
        [Fact]
        public void IFileFeedParser_for_json_should_return_valid_race_object_with_2_horses()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();

            IFileFeedParser fileProcessor = fileProcessManager.GetFileProcessor(_jsonFilePath);
            FileInfo fi = new FileInfo(_jsonFilePath);
            ICollection<Race> races = fileProcessor.ProcessFileAndGetRaceInfo(fullFilePath: fi.FullName);

            Assert.NotNull(races);
            Assert.Equal(1, races.Count);
            Assert.Equal(2, races.First().Horses.Count);
        }

        [Fact]
        public void FileProcessManager_should_return_4_horses()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();
            ICollection<Horse> result = fileProcessManager.GetAllHorses("./SupportFiles");
            
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
        }
        
        [Fact]
        public void FileProcessManager_should_have_horses_in_ascending_order_of_price()
        {
            var fileProcessManager = _serviceProvider.GetRequiredService<FileProcessManager>();
            ICollection<Horse> result = fileProcessManager.GetAllHorses("./SupportFiles");
            
            Assert.Equal(4.2M, result.First().Price);
            Assert.Equal(12M, result.Last().Price);
        }
    }
}
