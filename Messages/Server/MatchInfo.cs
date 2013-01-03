using System.Collections.Generic;

namespace CSharpCTFStarter.Messages.Server
{
    public class MatchInfo
    {
    	public double timePassed;
        public double timeRemaining;
        public double timeToNextRespawn;
    	public Dictionary<string, double> scores;
        public List<Envelope<MatchCombatEvent>> combatEvents;
    }
}