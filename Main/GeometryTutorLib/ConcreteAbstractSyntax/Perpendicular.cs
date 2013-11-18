using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Special case of intersection
    /// </summary>
    public class Perpendicular : Intersection
    {
        public Perpendicular intersection { get; private set; }

        public override string ToString()
        {
            return "Perpendicular Intersect(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + "): " + justification;
        }

        public override bool Equals(Object obj)
        {
            Intersection inter = obj as Intersection;
            if (inter == null) return false;
            return intersect.Equals(inter.intersect) && lhs.Equals(inter.lhs) && rhs.Equals(inter.rhs);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}
