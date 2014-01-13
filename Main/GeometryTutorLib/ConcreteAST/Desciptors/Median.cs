using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Median : Descriptor
    {
        public Segment medianSegment { get; private set; }
        public Triangle theTriangle { get; private set; }

        public Median(Segment segment, Triangle thatTriangle, string just)
        {
            medianSegment = segment;
            theTriangle = thatTriangle;
            justification = just;
        }

        // Does this median cover the given clause
        public override bool Covers(GroundedClause gc)
        {
            // A median is with respec to a triangle, therefore, even though it falls inside the triangle, a median still covers it.
            if (gc is Triangle)
            {
                if (theTriangle.StructurallyEquals(gc)) return true;
            }

            return medianSegment.Covers(gc) || theTriangle.Covers(gc);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            Median med = obj as Median;
            if (med == null) return false;
            return theTriangle.StructurallyEquals(med.theTriangle) && medianSegment.StructurallyEquals(med.medianSegment);
        }

        public override bool Equals(Object obj)
        {
            Median med = obj as Median;
            if (med == null) return false;
            return theTriangle.Equals(med.theTriangle) && medianSegment.Equals(med.medianSegment) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "Median(" + medianSegment.ToString() + ", " + theTriangle.ToString() + "): " + justification;
        }
    }
}
