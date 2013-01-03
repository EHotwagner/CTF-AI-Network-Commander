using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSharpCTFStarter.Util
{
    [DebuggerDisplay("{this}")]
    public class Box : List<Vector2>
    {
        public double LeftX { get { return this[0].X; } }
        public double RightX { get { return this[1].X; } }
        public double TopY { get { return this[0].Y; } }
        public double BottomY { get { return this[1].Y; } }

        public Vector2 Center
        {
            get { return new Vector2((RightX-LeftX) / 2 + LeftX, (BottomY - TopY)/2 + TopY); }
        }

        public override string ToString()
        {
            return "(" + LeftX + ".." + RightX + ", " + TopY + ".." + BottomY + ")";
        }
    }
}