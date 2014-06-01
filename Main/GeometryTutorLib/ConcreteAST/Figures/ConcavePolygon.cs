using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a concave polygon (which consists of n >= 4 segments)
    /// </summary>
    public class ConcavePolygon : Polygon
    {
        public ConcavePolygon() { }

        public ConcavePolygon(List<Segment> segs, List<Point> pts, List<Angle> angs)
        {
            orderedSides = segs;
            points = pts;
            angles = angs;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            ConcavePolygon thatPoly = obj as ConcavePolygon;
            if (thatPoly == null) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            ConcavePolygon thatPoly = obj as ConcavePolygon;
            if (thatPoly == null) return false;

            return base.Equals(obj);
        }
    }
}
