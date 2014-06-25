//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace GeometryTutorLib.ConcreteAST
//{
//    public class GeometricSegmentRatio : SegmentRatio
//    {
//        public GeometricSegmentRatio(Segment segment1, Segment segment2) : base(segment1, segment2) { }

//        public override bool IsAlgebraic() { return false; }
//        public override bool IsGeometric() { return true; }

//        public override bool Equals(Object obj)
//        {
//            GeometricSegmentRatio aps = obj as GeometricSegmentRatio;
//            if (aps == null) return false;

//            return base.Equals(aps);
//        }

//        public override int GetHashCode() { return base.GetHashCode(); }

//        public override string ToString()
//        {
//            return "GeometricProportional(" + largerSegment.ToString() + " < " + dictatedProportion + " > " + smallerSegment.ToString() + ") " + justification;
//        }
//    }
//}
