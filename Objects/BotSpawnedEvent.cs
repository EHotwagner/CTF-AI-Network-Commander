namespace CSharpCTFStarter.Objects
{
    public class BotSpawnedEvent : GameEvent
    {
		public Bot SpawnedBot { get; internal set; }
    }
}