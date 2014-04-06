using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Tangent : Descriptor
    {
        public Circle circle { get; protected set; }
        public Segment segment { get; protected set; }

        // These may be null
        public Intersection intersection { get; protected set; }
        public Segment radius { get; protected set; }

        public Tangent() : base() { }
        public Tangent(Circle c, Segment tangent) : base()
        {
            circle = c;
            segment = tangent;

        }

        //public Tangent(Circle c, Segment tangent) : base()
        //{
        //    circle = c;
        //    segment = tangent;
        //    intersection = null;
        //    radius = null;
        //}

        //
        // If a radius exists from the point of tangency to the center, then an intersection is defined.
        //
        public bool DefinesIntersection()
        {
            return intersection == null || radius == null;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Tangent tangent = obj as Tangent;
            if (tangent == null) return false;
            return this.circle.StructurallyEquals(tangent.circle) && this.segment.StructurallyEquals(tangent.segment);
        }

        public override bool Equals(Object obj)
        {
            Tangent tangent = obj as Tangent;
            if (tangent == null) return false;
            return this.circle.Equals(tangent.circle) && this.segment.Equals(tangent.segment) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Tangent(" + circle.ToString() + ", " + segment.ToString() + ") " + justification;
        }
    }
}