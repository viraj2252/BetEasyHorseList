using System.Collections.Generic;
using dotnet_code_challenge.Models;

namespace dotnet_code_challenge.Interfaces
{
    public interface IFileFeedParser
    {
        ICollection<Race> ProcessFileAndGetRaceInfo(string fullFilePath);
    }
}