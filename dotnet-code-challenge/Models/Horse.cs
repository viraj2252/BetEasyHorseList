using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace dotnet_code_challenge.Models
{
    public class Horse
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}