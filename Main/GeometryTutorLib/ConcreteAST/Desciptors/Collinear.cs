using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Collinear : Descriptor
    {
        public List<Point> points { get; private set; }

        //
        // We assume the points are ordered how they appear
        // But we verify just in case
        public Collinear(List<Point> pts, string just) : base()
        {
            points = pts;
            justification = just;

            Verify();
        }

        private void Verify()
        {
            if (points.Count < 3) throw new ArgumentException("A Collinear relationship requires at least 3 points: " + this.ToString());

            // Create a segment of the endpoints to compare all points for collinearity
            Segment line = new Segment(points[0], points[points.Count - 1]);

            foreach (Point pt in points)
            {
                if (!line.PointIsOn(pt))
                {
                    throw new ArgumentException("Point " + pt + " is not collinear with line " + line.ToString());
                }

                if (!line.PointIsOnAndBetweenEndpoints(pt))
                {
                    throw new ArgumentException("Point " + pt + " is not between the endpoints of segment " + line.ToString());
                }
            }
        }

        public override bool Equals(Object obj)
        {
            Collinear collObj = obj as Collinear;
            if (collObj == null) return false;

            // Check all points
            foreach (Point pt in collObj.points)
            {
                if (!points.Contains(pt)) return false;
            }
            return true;
        }

        public override string ToString()
        {
            List<String> strings = new List<String>();
            foreach (Point p in points) strings.Add(p.ToString());
            return "Collinear(" + string.Join(",", strings.ToArray()) + ")";
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}
