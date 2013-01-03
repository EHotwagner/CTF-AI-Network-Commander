using System.Collections.Generic;

namespace CSharpCTFStarter.Messages.Server
{
    public class GameInfo
    {
        public Dictionary<string, Envelope<TeamInfo>> teams;
        public string team;
        public string enemyTeam;
        public Dictionary<string, Envelope<FlagInfo>> flags;
        public Dictionary<string, Envelope<BotInfo>> bots;
        public Envelope<MatchInfo> match;
    }
}