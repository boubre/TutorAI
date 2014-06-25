//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace GeometryTutorLib.ConcreteAST
//{
//    /// <summary>
//    /// Describes a point that lies on a segmant.
//    /// </summary>
//    public class AlgebraicSegmentRatio : SegmentRatio
//    {
//        public AlgebraicSegmentRatio(Segment segment1, Segment segment2) : base(segment1, segment2) { }

//        public override bool IsAlgebraic() { return true; }
//        public override bool IsGeometric() { return false; }

//        public override bool Equals(Object obj)
//        {
//            AlgebraicSegmentRatio aps = obj as AlgebraicSegmentRatio;
//            if (aps == null) return false;
//            return base.Equals(aps);
//        }

//        public override int GetHashCode() { return base.GetHashCode(); }

//        public override string ToString()
//        {
//            return "AlgebraicProportional(" + largerSegment.ToString() + " < " + dictatedProportion + " > " + smallerSegment.ToString() + ") " + justification;
//        }
//    }
//}
