using System.Collections.Generic;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Objects
{
    public class Team
    {
		public string Name { get; internal set; }
		public Flag Flag { get; internal set; }
		public Vector2 FlagSpawnLocation { get; internal set; }
		public Vector2 ScoreLocation { get; internal set; }
		public Box BotSpawnLocation { get; internal set; }
		public ICollection<Bot> Members { get; internal set; }
    }
}