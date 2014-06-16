using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Figure : GroundedClause
    {
        protected List<Figure> superFigures;
        public Polygon polygonalized { get; protected set; }
        public List<AtomicRegion> atoms { get; protected set; }

        public void AddAtomicRegion(AtomicRegion atom)
        {
            //
            // Avoid adding an atomic region which is itself
            //
            if (atom is ShapeAtomicRegion)
            {
                if ((atom as ShapeAtomicRegion).shape.StructurallyEquals(this)) return;
            }

            atoms.Add(atom); 
        }

        protected Figure()
        {
            superFigures = new List<Figure>();
            polygonalized = null;
            atoms = new List<AtomicRegion>();
        }

        // Can we compute the area of this figure?
        public virtual bool IsComputableArea() { return false; }
        public virtual bool CanAreabeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known) { return false; }
        public virtual double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known) { return -1; }

        public const int NUM_SEGS_TO_APPROX_ARC = 72;
        public virtual bool PointLiesInside(Point pt) { return false; }
        public virtual bool PointLiesInOrOn(Point pt) { return false; }
        public virtual List<Segment> Segmentize() { return new List<Segment>(); }
        public virtual List<Point> GetApproximatingPoints() { return new List<Point>(); }

        public bool isShared()
        {
            return superFigures.Count > 1;
        }

        //public virtual bool HasSegmentWithEndpoints(Point e1, Point e2) { return false; }

        public List<Figure> getSuperFigures()
        {
            return superFigures;
        }

        public virtual Polygon GetPolygonalized() { return null; }

        //
        // Is this figure (or atomic region approx by polygon) completely contained in the given figure?
        //
        public bool Contains(Polygon thatPoly)
        {
            //
            // Special Cases:
            //
            // Disambiguate Major Sector from Minor Sector
            if (this is Sector && (this as Sector).theArc is MajorArc)
            {
                // Not only do all the points need to be inside, the midpoints do as well.
                foreach(Segment side in thatPoly.orderedSides)
                {
                    Point midpt = side.Midpoint();
                    if (!this.PointLiesInOrOn(midpt)) return false;
                }
            }

            //
            // General Case
            //
            foreach (Point thatPt in thatPoly.points)
            {
                if (!this.PointLiesInOrOn(thatPt)) return false;
            }

            return true;
        }
    }
}
