using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
    public abstract class ActualProblem
    {
        // Hard-coded intrinsic problem characteristics
        protected List<GroundedClause> intrinsic;

        // Boolean facts
        protected List<GroundedClause> given;

        // Boolean the book problem is attempting to prove; use in validation of figures / generated problems
        // One <figure, given> pair may contain multiple goals
        protected List<GroundedClause> goals;

        // Formatted Labeling of the Problem
        protected string problemName;

        public bool problemIsOn { get; protected set; }

        //
        // For constructing clauses consistent with the UI.
        //
        protected LiveGeometry.TutorParser.HardCodedParserMain parser;

        protected List<Point> points = new List<Point>();
        protected List<Collinear> collinear = new List<Collinear>();
        protected List<Segment> segments = new List<Segment>();
        protected List<Circle> circles = new List<Circle>();


        //
        // Statistics Generation
        //
        // All statistics returned from the analysis
        protected StatisticsGenerator.FigureStatisticsAggregator figureStats;
        
        public const bool INCOMPLETE = false;
        public const bool COMPLETE = true;
        public bool isComplete;

        // Main routine to run a problem through the system.
        public abstract void Run();
    }
}