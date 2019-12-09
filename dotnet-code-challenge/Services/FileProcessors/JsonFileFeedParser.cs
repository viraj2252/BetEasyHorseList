using System.Collections.Generic;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Models;

namespace dotnet_code_challenge.Services.FileProcessors
{
    public class JsonFileFeedParser: IFileFeedParser
    {
        public ICollection<Race> ProcessFileAndGetRaceInfo(string fullFilePath)
        {
            throw new System.NotImplementedException();
        }
    }
}