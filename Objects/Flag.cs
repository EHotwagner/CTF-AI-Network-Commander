using System;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Objects
{
    public class Flag
    {
		public string Name { get; internal set; }
		public Vector2 Position { get; internal set; }
		public Team Team { get; internal set; }
		public Bot Carrier { get; internal set; }
    }
}