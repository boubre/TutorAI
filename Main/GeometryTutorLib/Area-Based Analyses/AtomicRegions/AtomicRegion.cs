using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public abstract class AtomicRegion
    {
        public List<Point> perimeterPoints { get; protected set; }
        public List<Figure> containingFigures { get; protected set; }

        // A version of this region that is an approximate polygon.
        public Polygon polygonalized { get; protected set; }

        public AtomicRegion()
        {
            perimeterPoints = new List<Point>();
            containingFigures = new List<Figure>();
            polygonalized = null;
            thisArea = -1;
        }

        public abstract Polygon GetPolygonalized();

        public virtual void AddOwnedFigure(Figure f) { containingFigures.Add(f); }


        public abstract bool PointLiesInside(Point pt);

        //
        // Can the area of this region be calculated?
        //
        public abstract bool IsComputableArea();
        public virtual double GetArea(KnownMeasurementsAggregator known) { return thisArea; }
        protected double thisArea;


        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            AtomicRegion thatAtom = obj as AtomicRegion;
            if (thatAtom == null) return false;

            return true;
        }
    }
}
