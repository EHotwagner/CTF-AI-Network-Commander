namespace CSharpCTFStarter.Objects
{
    public class FlagPickedUpEvent : GameEvent
    {
		public Bot FlagCarrier { get; internal set; }
		public Flag Flag { get; internal set; }
    }
}