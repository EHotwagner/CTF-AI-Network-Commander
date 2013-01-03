using System;
using System.Collections.Generic;
using System.Linq;
using CSharpCTFStarter.Messages.Client;
using CSharpCTFStarter.Util;

namespace CSharpCTFStarter.Objects
{
    public class Bot
    {
        private readonly Action<object> _actionCallback;

        public Bot(Action<object> actionCallback = null)
        {
            _actionCallback = actionCallback;
        }

        public string Name { get; internal set; }
		public Vector2 Position { get; internal set; }
		public Vector2 FacingDirection { get; internal set; }
		public IEnumerable<Bot> VisibleEnemies { get; internal set; }
		public IEnumerable<Bot> SeenBy { get; internal set; }
		public Flag CarriedFlag { get; internal set; }
		public string CurrentAction { get; internal set; }
		public BotState? State { get; internal set; }
		public double? LastSeen { get; internal set; }
		public Team Team { get; internal set; }
		public double? Health { get; internal set; }

        public void Attack(IEnumerable<Vector2> targets, Vector2 lookAt, string description)
        {
            _actionCallback.Invoke(new Attack
            {
                bot = Name,
                description = description,
                target = targets.ToList(),
                lookAt = lookAt
            });
        }

        public void Attack(Vector2 target, Vector2 lookAt, string description)
        {
            Attack(new[] { target }, lookAt, description);
        }


        public void Attack(Vector2 target, string description)
        {
            Attack(new[] { target }, null, description);
        }

        public void Run(IEnumerable<Vector2> targets, string description)
        {
            _actionCallback.Invoke(new Move()
                                       {
                                           bot = Name,
                                           description = description,
                                           target = targets.ToList()
                                       });
        }

        public void Run(Vector2 target, string description)
        {
            Run(new[] { target }, description);
        }

        public void Charge(IEnumerable<Vector2> targets, string description)
        {
            _actionCallback.Invoke(new Charge()
                    {
                        bot = Name,
                        description = description,
                        target =targets.ToList()
                    });
        }

        public void Charge(Vector2 target, string description)
        {
            Charge(new[] { target }, description);
        }

        public override string ToString()
        {
            return Name;
        }

        public void Defend(Vector2 target, string description)
        {
            Defend(new[] {target}, description);
        }
        
        public void Defend(IEnumerable<Vector2> targets, string description)
        {
            Defend(targets.Select(target => Tuple.Create(target, 1.0)), description);
        }

        public void Defend(IEnumerable<Tuple<Vector2, double>> targetDurationPairs, string description)
        {
            _actionCallback.Invoke(new Defend
            {
                bot = Name,
                description = description,
                facingDirections = targetDurationPairs.Select(pair => new object[] { pair.Item1 - Position, pair.Item2 }).ToArray()
            });
        }
    }
}
