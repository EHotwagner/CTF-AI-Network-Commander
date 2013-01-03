using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
namespace CSharpCTFStarter.Util
{
    [DebuggerDisplay("{this}")]    
    public class Vector2 : List<double>
    {
        public Vector2()
        {
        }

        public Vector2(double x, double y)
        {
            Add(x);
            Add(y);
        }

        public double X { get { return this[0]; } }
        public double Y { get { return this[1]; } }
        
		/// <summary>
		/// Adds two vectors together
		/// </summary>
        public static Vector2 operator+ (Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

		/// <summary>
		/// Gets the difference between two vectors
		/// </summary>
        public static Vector2 operator- (Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

		/// <summary>
		/// Applies a scalar multiple to a vector
		/// </summary>
        public static Vector2 operator* (Vector2 v, double scalar)
        {
            return new Vector2(v.X*scalar, v.Y*scalar);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

		/// <summary>
		/// Calculates the distance between this vector and another
		/// </summary>
        public double DistanceFrom(Vector2 other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return Math.Sqrt(dx*dx + dy*dy);
        }

		/// <summary>
		/// Calculates the midpoint between this vector and another
		/// </summary>
        public Vector2 MidPoint(Vector2 other)
        {
            return new Vector2((X + other.X)/2, (Y + other.Y)/2);
        }

		/// <summary>
		/// Calculates the angle between the line segment formed between this vector and target1, 
		/// and the line segment formed between this vector and target2.
		/// </summary>
		/// <returns>The calculated angle, in radians</returns>
        public double AngleDiff(Vector2 target1, Vector2 target2)
        {
            var v1 = target1 - this;
            var v2 = target2 - this;
            var dot = v1.Dot(v2);
            var afterDividing = dot/v1.Magnitude()/v2.Magnitude();
            return Math.Acos(afterDividing);
        }

        private double Magnitude()
        {
            return Math.Sqrt(X*X + Y*Y);
        }

        private double Dot(Vector2 other)
        {
            return X*other.X + Y*other.Y;
        }
    }
}