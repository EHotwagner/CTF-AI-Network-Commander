namespace CSharpCTFStarter.Objects
{
    public class BotKilledEvent : GameEvent
    {
		public Bot KillerBot { get; internal set; }
		public Bot DeadBot { get; internal set; }
    }
}