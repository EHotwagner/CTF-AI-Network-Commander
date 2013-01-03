﻿using System.Collections.Generic;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Messages.Client
{
    public class Charge
    {
        public string bot { get; set; }
        public List<Vector2> target { get; set; }
        public string description { get; set; }
    }
}