using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class ConcreteCollinear : GroundedClause
    {
        public List<ConcretePoint> points { get; private set; }

        public ConcreteCollinear(List<ConcretePoint> pts)
        {
            points = pts;
        }

        public override bool Equals(Object obj)
        {
            ConcreteCollinear collObj = obj as ConcreteCollinear;
            if (collObj == null) return false;

            // Check all points
            foreach (ConcretePoint pt in collObj.points)
            {
                if (!points.Contains(pt)) return false;
            }
            return true;
        }

        public override string ToString()
        {
            List<String> strings = new List<String>();
            foreach (ConcretePoint p in points) 
                strings.Add(p.ToString());
            return "Collinear(" + string.Join(",", strings.ToArray()) + ")";
        }
    }
}
