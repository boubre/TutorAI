using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Figure : GroundedClause
    {
        protected List<Figure> superFigures;
        public List<Point> ownedPoints { get; protected set; }

        protected Figure()
        {
            superFigures = new List<Figure>();
            ownedPoints = new List<Point>();
        }

        public virtual bool IsPointOwned(Point pt) { return false; }

        public void AddOwnedPoint(Point pt)
        {
            ownedPoints.Add(pt);
        }

        //
        // Do the owned sets of points equata?
        //
        public bool DefinedByAtoms(List<Point> atomOwnedPoints)
        {
            if (ownedPoints.Count != atomOwnedPoints.Count) return false;

            foreach (Point atomOwned in atomOwnedPoints)
            {
                if (!Utilities.HasStructurally<Point>(ownedPoints, atomOwned)) return false;
            }

            return true;
        }

        public bool isShared()
        {
            return superFigures.Count > 1;
        }

        //public virtual bool HasSegmentWithEndpoints(Point e1, Point e2) { return false; }

        public List<Figure> getSuperFigures()
        {
            return superFigures;
        }
    }
}
