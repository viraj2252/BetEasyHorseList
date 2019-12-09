using System;
using System.Collections;
using System.Collections.Generic;

namespace dotnet_code_challenge.Models
{
    public class Race
    {
        public string Name { get; set; }
        public DateTime StartTimeUTC { get; set; }
        public int NumberOfRunners { get; set; }
        public ICollection<Horse> Horses { get; set; }
    }
}