namespace CSharpCTFStarter.Objects
{
    public class FlagDroppedEvent : GameEvent
    {
		public Bot PreviousFlagCarrier { get; internal set; }
		public Flag Flag { get; internal set; }
    }
}