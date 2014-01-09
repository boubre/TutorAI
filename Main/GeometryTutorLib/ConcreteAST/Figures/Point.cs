using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A 2D point
    /// </summary>
    public class Point : Figure
    {
        private static int CURRENT_ID = 0;

        public double X { get; private set; }
        public double Y { get; private set; }
        public static readonly double EPSILON = 0.0001;

        /// <summary> A unique identifier for this point. </summary>
        public int ID { get; private set; }

        public string name { get; private set; }

        /// <summary>
        /// Create a new ConcretePoint with the specified coordinates.
        /// </summary>
        /// <param name="name">The name of the point. (Assigned by the UI)</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Point(string n, double x, double y) : base()
        {
            this.ID = CURRENT_ID++;
            name = n != null ? n : "";
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Find the distance between two points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">Tjhe second point</param>
        /// <returns>The distance between the two points</returns>
        public static double calcDistance(Point p1, Point p2)
        {
            return System.Math.Sqrt(System.Math.Pow(p2.X - p1.X, 2) + System.Math.Pow(p2.Y - p1.Y, 2));
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcretePoint [");
            sb.Append(name);
            sb.Append(": (");
            sb.Append(X);
            sb.Append(", ");
            sb.Append(Y);
            sb.Append(")]");
            sb.AppendLine();
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            Point pt = obj as Point;

            if (pt == null) return false;

            return Utilities.CompareValues(pt.X, X) && Utilities.CompareValues(pt.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            Point pt = obj as Point;

            if (pt == null) return false;

            return StructurallyEquals(obj); // && name.Equals(pt.name);
        }

        public override bool Covers(GroundedClause gc)
        {
            if (gc is Point) return this.Equals(gc as Point);

            return false;
        }

        // Make a deep copy of this object; this is actually shallow, but is all that is required.
        public override GroundedClause DeepCopy()
        {
            return (Point)(this.MemberwiseClone());
        }

        public override string ToString() { return name + "(" + X + ", " + Y + ")"; }
    }
}
