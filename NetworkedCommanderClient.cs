using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using CSharpCTFStarter.Messages;
using CSharpCTFStarter.Messages.Client;
using CSharpCTFStarter.Messages.Server;
using CSharpCTFStarter.Objects;
using Newtonsoft.Json;

namespace CSharpCTFStarter
{
    public class NetworkedCommanderClient
    {
        private readonly TcpClient _client;
        private readonly ICommander _commander;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private Game _game;

        private NetworkedCommanderClient(TcpClient client, ICommander commander)
        {
            _client = client;
            _commander = commander;

            NetworkStream networkStream;
            try
            {
                networkStream = _client.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            _reader = new StreamReader(networkStream, Encoding.ASCII);
            _writer = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };
        }

        private void Run()
        {
            DoHandshaking();
            Initialize();
            while (true)
            {
                var line = ReadLine();
                if (line == "<tick>") Tick();
                else if (line == "<shutdown>")
                {
                    _commander.ShutDown();
                    return;
                }
                else
                {
                    throw new Exception("Expected <tick> or <shutdown>; received: " + line);
                }
            }
        }

        private void Tick()
        {
            var gameInfoJson = ReadLine();
            var gameInfo = ReadJson<GameInfo>(gameInfoJson);
            _game.Update(gameInfo);
            _commander.Tick();
        }

        private void DoHandshaking()
        {
            Expect("<connect>");

            var connectServerJson = ReadLine();
            var connectServerMsg = ReadJson<ConnectServer>(connectServerJson);
            const string EXPECTED_PROTOCOL_VERSION = "1.3";
            if (EXPECTED_PROTOCOL_VERSION != connectServerMsg.protocolVersion)
            {
                throw new Exception(string.Format("Expected protocol version: {0}, server protocol version: {1}"
                    , EXPECTED_PROTOCOL_VERSION, connectServerMsg.protocolVersion));
            }

            var connectString = "<connect>\n" + WriteJson(new ConnectClient { commanderName = _commander.GetName() });

            WriteLine(connectString);
        }

        private void Initialize()
        {
            Expect("<initialize>");
            var levelInfoJson = ReadLine();
            var levelInfo = ReadJson<LevelInfo>(levelInfoJson);
            var gameInfoJson = ReadLine();
            var gameInfo = ReadJson<GameInfo>(gameInfoJson);

            _game = new Game(gameInfo, levelInfo, WriteAnyObject);
            _commander.Initialize(_game);

            WriteLine("<ready>");
        }

        private void Expect(string expected)
        {
            var msg = ReadLine();
            if (msg != expected) throw new Exception("Expected: " + expected + "; got: " + msg);
        }

        private static T ReadJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<Envelope<T>>(json).__value__;
        }

        private void WriteAnyObject(object obj)
        {
            string envelopeJson;

            if (obj is Attack) envelopeJson = WriteJson((Attack)obj);
            else if (obj is Charge) envelopeJson = WriteJson((Charge)obj);
            else if (obj is Defend) envelopeJson = WriteJson((Defend)obj);
            else if (obj is Move) envelopeJson = WriteJson((Move)obj);
            else throw new Exception("Don't know how to write object: " + obj);

            WriteLine("<command>\n" + envelopeJson);
        }

        private static string WriteJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(new Envelope<T>(obj));
        }

        private string ReadLine()
        {
            string line;
            try
            {
                line = _reader.ReadLine();
            }
            catch
            {
                _commander.ShutDown();
                throw;
            }

            return line;
        }

        private void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

        public static int Main(string[] args)
        {
            string host = "localhost";
            int port = 41041;
            string commanderName = null;

            if (args.Length == 1)
            {
                commanderName = args[0];
            }
            else if (args.Length == 3)
            {
                host = args[0];
                port = int.Parse(args[1]);
                commanderName = args[2];
            }
            else if (args.Length == 0)
            {
                // default commandername (null)
            }
            else if (args.Length == 2)
            {
                host = args[0];
                port = int.Parse(args[1]);
                // default commandername (null)
            }
            else
            {
                Console.WriteLine("Usage: CSharpCTFStarter [<hostname> <port>] [commander_name]");
                Console.WriteLine("e.g. CSharpCTFStarter localhost 41041 My.MyCommander (Make sure that My.dll is present!)");
                return 1;
            }

            var commander = commanderName == null
                                ? LoadSingleCommander()
                                : LoadCommanderByName(commanderName);

            if (commander == null)
            {
                Console.WriteLine("Unable to create commander");
                return 1;
            }

            while (true)
            {
                Console.Write("Waiting for server ");
                TcpClient tcpClient;

                while (true)
                {
                    Console.Write(".");
                    try
                    {
                        tcpClient = new TcpClient(host, port);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(2000);
                    }
                }
                Console.WriteLine("Connected!");

                new NetworkedCommanderClient(tcpClient, commander).Run();
                Console.WriteLine("Done");
                tcpClient.Close();
            }
        }

        private static ICommander LoadSingleCommander()
        {
            ICommander retVal = null;

            var folder = new DirectoryInfo(Environment.CurrentDirectory);
            foreach (var dll in folder.GetFiles("*.dll").Union(folder.GetFiles("*.exe")))
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(dll.FullName);
                }
                catch
                {
                    continue;
                }
                foreach (var type in assembly.GetTypes())
                {
                    if (type == typeof(ICommander)) continue;
                    if (!typeof (ICommander).IsAssignableFrom(type)) continue;

                    if (retVal != null)
                        throw new Exception("Too many commander types present");
                    retVal = (ICommander) Activator.CreateInstance(type);
                }

            }
            return retVal;
        }

        private static ICommander LoadCommanderByName(string commanderName)
        {
            var folder = new DirectoryInfo(Environment.CurrentDirectory);
            foreach (var dll in folder.GetFiles("*.dll").Union(folder.GetFiles("*.exe")))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dll.FullName);
                    var commanderType = assembly.GetType(commanderName, false);
                    if (commanderType == null) continue;
                    return (ICommander)Activator.CreateInstance(commanderType);
                }
                catch { }
            }
            return null;
        }
    }
}
