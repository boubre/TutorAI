using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Figure : GroundedClause
    {
        protected Figure()
        {
            subFigures = new List<Figure>();
            superFigures = new List<Figure>();
            polygonalized = null;
            atoms = new List<AtomicRegion>();
            intersectingPoints = new List<Point>();
        }

        // Can we compute the area of this figure?
        public virtual bool IsComputableArea() { return false; }
        public virtual bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known) { return false; }
        public virtual double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known) { return -1; }

        public const int NUM_SEGS_TO_APPROX_ARC = 72;
        public virtual bool PointLiesInside(Point pt) { return false; }
        public virtual bool PointLiesInOrOn(Point pt) { return false; }
        public virtual List<Segment> Segmentize() { return new List<Segment>(); }
        public virtual List<Point> GetApproximatingPoints() { return new List<Point>(); }

        protected List<Point> intersectingPoints;
        public void AddIntersectingPoint(Point pt) { Utilities.AddStructurallyUnique<Point>(intersectingPoints, pt); }
        public void AddIntersectingPoints(List<Point> pts) { pts.ForEach(p => this.AddIntersectingPoint(p)); }
        public List<Point> GetIntersectingPoints() { return intersectingPoints; }
        public virtual void FindIntersection(Segment that, out Point inter1, out Point inter2) { inter1 = null; inter2 = null; }
        public virtual void FindIntersection(Arc that, out Point inter1, out Point inter2) { inter1 = null; inter2 = null; }
        public virtual List<Connection> MakeAtomicConnections() { return new List<Connection>(); }

        // An ORDERED list of collinear points.
        public List<Point> collinear { get; protected set; }
        public virtual void AddCollinearPoint(Point newPt) { throw new ArgumentException("Only segments or arcs have 'collinearity'"); }
        public virtual void ClearCollinear() { throw new ArgumentException("Only segments or arcs have 'collinearity'"); }

        protected List<Figure> superFigures;
        protected List<Figure> subFigures;
        public void AddSuperFigure(Figure f) { if (!Utilities.HasStructurally<Figure>(superFigures, f)) superFigures.Add(f); }
        public void AddSubFigure(Figure f) { if (!Utilities.HasStructurally<Figure>(subFigures, f)) subFigures.Add(f); }
        public Polygon polygonalized { get; protected set; }
        public List<AtomicRegion> atoms { get; protected set; }
        public virtual Polygon GetPolygonalized() { return null; }

        public void AddAtomicRegion(AtomicRegion atom)
        {
            // Avoid adding an atomic region which is itself
            if (atom is ShapeAtomicRegion)
            {
                if ((atom as ShapeAtomicRegion).shape.StructurallyEquals(this)) return;
            }

            atoms.Add(atom);
        }


        public bool isShared() { return superFigures.Count > 1; }
        public List<Figure> getSuperFigures() { return superFigures; }

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
