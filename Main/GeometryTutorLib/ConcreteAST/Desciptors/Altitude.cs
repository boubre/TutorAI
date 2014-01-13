using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Altitude : Descriptor
    {
        public Triangle triangle { get; private set; }
        public Segment segment { get; private set; }

        public Altitude(Triangle tri, Segment alt, string just) : base()
        {
            triangle = tri;
            segment = alt;
            justification = just;
        }

        // Does this altitude cover the given clause
        // An altitude covers a segment, triangle, or point
        public override bool Covers(GroundedClause gc)
        {
            // An altitude may fall outside of the triangle, but coverage still occurs
            if (gc is Triangle)
            {
                if (triangle.StructurallyEquals(gc)) return true;
            }

            return segment.Covers(gc) || triangle.Covers(gc);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            Altitude alt = obj as Altitude;
            if (alt == null) return false;
            return triangle.StructurallyEquals(alt.triangle) && segment.StructurallyEquals(alt.segment);
        }

        public override bool Equals(Object obj)
        {
            Altitude alt = obj as Altitude;
            if (alt == null) return false;
            return triangle.Equals(alt.triangle) && segment.Equals(alt.segment) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "Altitude(" + segment.ToString() + ", " + triangle.ToString() + ")";
        }
    }
}
