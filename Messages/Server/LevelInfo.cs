using System.Collections.Generic;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Messages.Server
{
    public class LevelInfo
    {
        public int width;
        public int height ;
        public List<string> teamNames ;
        public Dictionary<string, Vector2> flagSpawnLocations ;
        public int[][] blockHeights;
        public Dictionary<string, Box> botSpawnAreas ;

        public List<double> fieldOfViewAngles;
        public double characterRadius;
        public double walkingSpeed;
        public double runningSpeed;
        public double firingDistance;
        public double gameLength;
        public double initializationTime;
    }
}