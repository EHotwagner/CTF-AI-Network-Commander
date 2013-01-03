using CSharpCTFStarter.Objects;

namespace CSharpCTFStarter
{
    public interface ICommander
    {
		/// <summary>
		/// Called at application startup, used to determine the player's name
		/// </summary>
        string GetName();

		/// <summary>
		/// Called at the end of a match, allows the commander to clean up any resources
		/// </summary>
        void ShutDown();

		/// <summary>
		/// Called at the start of a match, allows the commander to do any initialization work
		/// </summary>
        void Initialize(Game game);

		/// <summary>
		/// Called every game tick, allows the commander to requery the game object and
		/// send commands to the bot objects
		/// </summary>
        void Tick();
    }
}
