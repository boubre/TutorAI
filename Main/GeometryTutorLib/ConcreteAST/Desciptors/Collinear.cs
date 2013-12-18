using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Collinear : Descriptor
    {
        public List<Point> points { get; private set; }

        public Collinear(List<Point> pts, string just) : base()
        {
            points = pts;
            justification = just;
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
