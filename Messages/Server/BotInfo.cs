using System.Collections.Generic;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Messages.Server
{
    public class BotInfo
    {
        public string name;
        public string team;
        public Vector2 position;
        public Vector2 facingDirection;
        public string flag;
        public string currentAction;
        public int? state;
        public double? health;
        public double? seenlast;
        public List<string> visibleEnemies;
        public List<string> seenBy;
    }
}