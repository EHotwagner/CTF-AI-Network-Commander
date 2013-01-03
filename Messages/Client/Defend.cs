using System;
using System.Collections.Generic;
using CSharpCTFStarter.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CSharpCTFStarter.Messages.Client
{
    public class Defend
    {
        public string bot { get; set; }
        public IEnumerable<IEnumerable<object>> facingDirections { get; set; }
        public string description { get; set; }
    }
}