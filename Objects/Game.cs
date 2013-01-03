using System;
using System.Collections.Generic;
using System.Linq;
using CSharpCTFStarter.Messages.Server;

namespace CSharpCTFStarter.Objects
{
    public class Game
    {
        private readonly Action<object> _actionCallback;
        private readonly Dictionary<string, Bot> _botDict = new Dictionary<string, Bot>();
        private readonly Dictionary<string, Flag> _flagDict = new Dictionary<string, Flag>();
        private readonly Dictionary<string, Team> _teamDict = new Dictionary<string, Team>();

		public Team Us { get; internal set; }
		public Team Enemy { get; internal set; }
		public int FieldHeight { get; internal set; }
		public int FieldWidth { get; internal set; }
		public int[][] BlockHeights { get; internal set; }

		public IDictionary<BotState, double> FOVAngles { get; internal set; }
		public double CharacterRadius { get; internal set; }
		public double WalkingSpeed { get; internal set; }
		public double RunningSpeed { get; internal set; }
		public double FiringDistance { get; internal set; }
		public double GameLength { get; internal set; }
		public double InitializationTime { get; internal set; }

		public double ElapsedTime { get; internal set; }
		public double TimeRemaining { get; internal set; }
		public double TimeToNextRespawn { get; internal set; }
		public double OurScore { get; internal set; }
		public double EnemyScore { get; internal set; }

		public Queue<GameEvent> UnProcessedEvents { get; internal set; }
		
        public IEnumerable<Bot> AliveBots
        {
            get
            {
                return Us.Members
                    .Where(b => b.Health!=null && b.Health>0);
            }
        }

        public IEnumerable<Bot> AvailableBots
        {
            get
            {
                return Us.Members
                    .Where(b => b.Health != null && b.Health > 0 && b.State == BotState.Idle);
            }
        }


    	public Game(GameInfo gameInfo, LevelInfo levelInfo, Action<object> actionCallback)
        {
            _actionCallback = actionCallback;
            FieldHeight = levelInfo.height;
            FieldWidth = levelInfo.width;
            BlockHeights = levelInfo.blockHeights;

    	    FOVAngles = new Dictionary<BotState, double>();
    	    for (int i = 0; i < levelInfo.fieldOfViewAngles.Count; i++)
    	    {
    	        FOVAngles.Add((BotState) i, levelInfo.fieldOfViewAngles[i]);
    	    }
            CharacterRadius = levelInfo.characterRadius;
            WalkingSpeed = levelInfo.walkingSpeed;
            RunningSpeed = levelInfo.runningSpeed;
            FiringDistance = levelInfo.firingDistance;
            GameLength = levelInfo.gameLength;
            InitializationTime = levelInfo.initializationTime;

            UnProcessedEvents = new Queue<GameEvent>();

            foreach (var teamInfo in gameInfo.teams.Select(env => env.Value.__value__))
            {
                var team = new Team
                               {
                                   Name = teamInfo.name,
                                   FlagSpawnLocation = teamInfo.flagSpawnLocation,
                                   ScoreLocation = teamInfo.flagScoreLocation,
                                   BotSpawnLocation = teamInfo.botSpawnArea,
                                   Members = new List<Bot>()
                               };

                foreach (var member in teamInfo.members)
                {
                    var bot = new Bot(_actionCallback) {Name = member, Team = team};
                    _botDict.Add(member, bot);
                    team.Members.Add(bot);
                }

                _teamDict.Add(teamInfo.name, team);
            }

            foreach (var flagInfo in gameInfo.flags.Select(env => env.Value.__value__))
            {
                var flag = new Flag
                               {
                                   Team = _teamDict[flagInfo.team], 
                                   Name = flagInfo.name
                               };
                flag.Team.Flag = flag;
                
                _flagDict.Add(flagInfo.name, flag);
            }
			
            Us = _teamDict.Values.Single(team => team.Name == gameInfo.team);
            Enemy = _teamDict.Values.Single(team => team.Name == gameInfo.enemyTeam);
            Update(gameInfo);
        }

        public void Update(GameInfo gameInfo)
        {
        	ElapsedTime = gameInfo.match.__value__.timePassed;
            TimeRemaining = gameInfo.match.__value__.timeRemaining;
            TimeToNextRespawn = gameInfo.match.__value__.timeToNextRespawn;
        	OurScore = gameInfo.match.__value__.scores[Us.Name];
        	EnemyScore = gameInfo.match.__value__.scores[Enemy.Name];

            foreach (var flagInfo in gameInfo.flags.Select(env => env.Value.__value__))
            {
                var flag = _flagDict[flagInfo.name];
                flag.Position = flagInfo.position;
                flag.Carrier = flagInfo.carrier == null ? null : _botDict[flagInfo.carrier];
            }

            foreach (var botInfo in gameInfo.bots.Select(env => env.Value.__value__))
            {
                var bot = _botDict[botInfo.name];
                bot.CarriedFlag = botInfo.flag == null ? null : _flagDict[botInfo.flag];
                bot.CurrentAction = botInfo.currentAction;
                bot.FacingDirection = botInfo.facingDirection;
                bot.LastSeen = botInfo.seenlast;
                bot.Position = botInfo.position;
                bot.SeenBy = botInfo.seenBy.Select(enemyName => _botDict[enemyName]);
                bot.State = (BotState?) botInfo.state;
                bot.Health = botInfo.health;
                bot.VisibleEnemies = botInfo.visibleEnemies.Select(enemyName => _botDict[enemyName]);
            }

            foreach (var matchEventInfo in gameInfo.match.__value__.combatEvents.Select(env => env.__value__))
            {
                GameEvent @event;
                var subject = matchEventInfo.subject;
                var instigator = matchEventInfo.instigator;

                switch (matchEventInfo.type)
                {
                    case 1: // bot killed
                        @event = new BotKilledEvent {DeadBot = _botDict[subject], KillerBot = _botDict[instigator]};
                        break;
                    case 2: // flag picked up
                        @event = new FlagPickedUpEvent {FlagCarrier = _botDict[instigator], Flag = _flagDict[subject]};
                        break;
                    case 3: // flag dropped
                        @event = new FlagDroppedEvent {Flag = _flagDict[subject], PreviousFlagCarrier = _botDict[instigator]};
                        break;
					case 4: // flag captured
                		@event = new FlagCapturedEvent {Flag = _flagDict[subject]};
                		break;
					case 5: // flag restored
                		@event = new FlagRestoredEvent {Flag = _flagDict[subject]};
                		break;
                    case 6: // bot spawned (guessing?)
                        @event = new BotSpawnedEvent {SpawnedBot = _botDict[subject]};
                        break;
                    default:
                        continue; // ignore
                }
                @event.Time = matchEventInfo.time;
                UnProcessedEvents.Enqueue(@event);
            }
        }

    }

}