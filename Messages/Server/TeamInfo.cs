using System.Collections.Generic;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Messages.Server
{
    public class TeamInfo
    {
        public string name;
        public string flag;
        public List<string> members;
        public Vector2 flagSpawnLocation;
        public Vector2 flagScoreLocation;
        public Box botSpawnArea;
    }
}