using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// A 2D point
    /// </summary>
    public class ConcretePoint : GroundedClause
    {
        private static int CURRENT_ID = 0;

        public double X { get; private set; }
        public double Y { get; private set; }
        
        /// <summary> A unique identifier for this point. </summary>
        public int ID { get; private set; }

        public String Name { get; private set; }

        /// <summary>
        /// Create a new ConcretePoint with the specified coordinates.
        /// </summary>
        /// <param name="name">The name of the point. (Assigned by the UI)</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public ConcretePoint(String name, double x, double y)
        {
            this.ID = CURRENT_ID++;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Find the distance between two points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">Tjhe second point</param>
        /// <returns>The distance between the two points</returns>
        public static double calcDistance(ConcretePoint p1, ConcretePoint p2)
        {
            return System.Math.Sqrt(System.Math.Pow(p2.X - p1.X, 2) + System.Math.Pow(p2.Y - p1.Y, 2)); ;
        }

        internal override void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("ConcretePoint [");
            sb.Append(Name);
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

        public override bool Equals(GroundedClause obj)
        {
            ConcretePoint pt = obj as ConcretePoint;
            if (pt == null) return false;
            return pt.ID == ID;
        }
    }
}
