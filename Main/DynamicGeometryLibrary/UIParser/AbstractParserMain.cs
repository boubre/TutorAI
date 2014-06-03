using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public abstract class AbstractParserMain
    {
        //
        // Calculate all of the implied components of the figure.
        //
        public ImpliedComponentCalculator implied { get; protected set; }
        protected AtomicRegionIdentifier.AtomicIdentifier atomIdentifier;

        public AbstractParserMain()
        {
            // Reset the factory so we get points that start back at __A.
            PointFactory.Reset();
        }

        //
        // Identify and return all atomic regions
        //
        public List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion> IdentifyAtomicRegions()
        {
            atomIdentifier = new AtomicRegionIdentifier.AtomicIdentifier(implied);

            return atomIdentifier.GetAtomicRegions();
        }



        public GeometryTutorLib.EngineUIBridge.ProblemDescription MakeProblemDescription(List<GroundedClause> givens)
        {
            GeometryTutorLib.EngineUIBridge.ProblemDescription pdesc = new GeometryTutorLib.EngineUIBridge.ProblemDescription();
            pdesc.givens = givens;
            pdesc.points = new List<Point>();
            pdesc.collinear = new List<Collinear>();
            pdesc.triangles = new List<Triangle>();
            pdesc.segments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            pdesc.inMiddles = new List<InMiddle>();

            return pdesc;
        }

        /// <summary>
        /// Return clauses that should be passed to the back-end.
        /// </summary>
        /// <returns>Back-end input.</returns>
        public List<GroundedClause> GetIntrinsics()
        {
            List<GroundedClause> intrinsic = new List<GroundedClause>();
            //Points.ForEach((Point p) => intrinsic.Add(p));
            //Collinear.ForEach((Collinear c) => intrinsic.Add(c));
            //Triangles.ForEach((Triangle t) => intrinsic.Add(t));

            return intrinsic;
        }

        //
        // Given a list of grounded clauses, get the structurally unique.
        //
        public GroundedClause Get(GroundedClause clause)
        {
            if (clause is GeometryTutorLib.ConcreteAST.Point)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Point>(implied.points, clause as GeometryTutorLib.ConcreteAST.Point);
            }
            else if (clause is GeometryTutorLib.ConcreteAST.Segment)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Segment>(implied.segments, clause as GeometryTutorLib.ConcreteAST.Segment);
            }
            else if (clause is Intersection)
            {
                return GeometryTutorLib.Utilities.GetStructurally<Intersection>(implied.ssIntersections, clause as Intersection);
            }
            else if (clause is CircleCircleIntersection)
            {
                return GeometryTutorLib.Utilities.GetStructurally<CircleCircleIntersection>(implied.ccIntersections, clause as CircleCircleIntersection);
            }
            else if (clause is CircleSegmentIntersection)
            {
                return GeometryTutorLib.Utilities.GetStructurally<CircleSegmentIntersection>(implied.csIntersections, clause as CircleSegmentIntersection);
            }
            else if (clause is Angle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<Angle>(implied.angles, clause as Angle);
            }
            else if (clause is InMiddle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<InMiddle>(implied.inMiddles, clause as InMiddle);
            }
            else if (clause is ArcInMiddle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<ArcInMiddle>(implied.arcInMiddle, clause as ArcInMiddle);
            }

            else if (clause is Triangle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Polygon>(implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX], clause as GeometryTutorLib.ConcreteAST.Polygon);
            }
            else if (clause is Quadrilateral)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Polygon>(implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.QUADRILATERAL_INDEX], clause as GeometryTutorLib.ConcreteAST.Polygon);
            }

            return null;
        }

        public Intersection GetIntersection(GeometryTutorLib.ConcreteAST.Segment segment1, GeometryTutorLib.ConcreteAST.Segment segment2)
        {
            Point inter = (Point)Get(segment1.FindIntersection(segment2));

            return (Intersection)Get(new Intersection(inter, segment1, segment2));
        }
    }
}