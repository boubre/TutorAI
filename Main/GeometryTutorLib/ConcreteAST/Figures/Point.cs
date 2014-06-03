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

        //
        // Assumes our points represent vectors in std position
        //
        public static double CrossProduct(Point thisPoint, Point thatPoint)
        {
            return thisPoint.X * thatPoint.Y - thisPoint.Y * thatPoint.X;
        }

        //
        // Angle measure (in degrees) between two vectors in standard position.
        //
        public static double AngleBetween(Point thisPoint, Point thatPoint)
        {
            return new Angle(thisPoint, new Point("", 0, 0), thatPoint).measure;
        }

        public static Point MakeVector(Point tail, Point head) { return new Point("", head.X - tail.X, head.Y - tail.Y); }
        public static Point GetOppositeVector(Point v) { return new Point("", -v.X, -v.Y); }

        public int Quadrant()
        {
            if (Utilities.CompareValues(X, 0) && Utilities.CompareValues(Y, 0)) return 0;
            if (Utilities.GreaterThan(X, 0) && Utilities.GreaterThan(Y, 0)) return 1;
            if (Utilities.CompareValues(X, 0) && Utilities.GreaterThan(Y, 0)) return 12;
            if (Utilities.LessThan(X, 0) && Utilities.GreaterThan(Y, 0)) return 2;
            if (Utilities.LessThan(X, 0) && Utilities.CompareValues(Y, 0)) return 23;
            if (Utilities.LessThan(X, 0) && Utilities.CompareValues(Y, 0)) return 3;
            if (Utilities.CompareValues(X, 0) && Utilities.LessThan(Y, 0)) return 34;
            if (Utilities.GreaterThan(X, 0) && Utilities.LessThan(Y, 0)) return 4;
            if (Utilities.GreaterThan(X, 0) && Utilities.CompareValues(Y, 0)) return 41;

            return -1;
        }

        //
        // Returns a radian angle measurement between [-pi / 2, 3 pi / 2]. 
        //
        public static double GetStandardAngleWithCenter(Point center, Point other)
        {
            Point stdVector = new Point("", other.X - center.X, other.Y - center.Y);

            double angle = System.Math.Atan2(stdVector.Y, stdVector.X);

            //// Add to the angle to place it into the proper quadrant.
            //switch (stdVector.Quadrant())
            //{
            //    case 0:
            //    case 1:
            //    case 14:
            //    case 12:
            //    case 34:
            //    case 4:
            //        break;
            //    case 2:
            //    case 23:
            //    case 3:
            //        angle += Math.PI;
            //        break;
            //}

            return angle;
        }

        //
        // Maintain a public repository of all segment objects in the figure.
        //
        public static void Clear()
        {
            figurePoints.Clear();
        }
        public static List<Point> figurePoints = new List<Point>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Point) figurePoints.Add(clause as Point);
        }
        public static Point GetFigurePoint(Point candPoint)
        {
            foreach (Point p in figurePoints)
            {
                if (p.StructurallyEquals(candPoint)) return p;
            }

            return null;
        }
        //public static Point GetFigurePointOrCreate(Point candPoint)
        //{
        //    // Get the existent point
        //    Point p = GetFigurePoint(candPoint);
            
        //    if (p != null) return p;

        //    // Create a new point
        //    return EngineUIBridge.PointFactory.GeneratePoint(candPoint.X, candPoint.Y);
        //}

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

        //
        // One-dimensional betweeness
        //
        public static bool Between(double val, double a, double b)
        {
            if (a >= val && val <= b) return true;
            if (b >= val && val <= a) return true;

            return false;
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

        public override int GetHashCode() { return base.GetHashCode(); }

        // Make a deep copy of this object; this is actually shallow, but is all that is required.
        public override GroundedClause DeepCopy() { return (Point)(this.MemberwiseClone()); }

        public override string ToString() { return name + "(" + string.Format("{0:N3}", X) + ", " + string.Format("{0:N3}", Y) + ")"; }

        /// <summary>
        /// p1 < p2 : -1
        /// p1 == p2 : 0
        /// p1 > p2 : 1
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int LexicographicOrdering(Point p1, Point p2)
        {
            // X's first
            if (p1.X < p2.X) return -1;

            if (p1.X > p2.X) return 1;

            // Y's second
            if (p1.Y < p2.Y) return -1;

            if (p1.Y > p2.Y) return 1;

            // Equal points
            return 0;
        }
    }
}
