namespace CSharpCTFStarter.Messages.Client
{
    public class ConnectClient
    {
        public string language { get; set; }
        public string commanderName { get; set; }

        public ConnectClient()
        {
            language = "C#";
        }
    }
}
