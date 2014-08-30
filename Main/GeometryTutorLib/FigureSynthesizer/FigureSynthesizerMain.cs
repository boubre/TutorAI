using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib
{
    /// <summary>
    /// Main routine for the search technique to synthesize figures.
    /// </summary>
    public static partial class FigureSynthesizerMain
    {
        public static void SynthesizerMain(Dictionary<ShapeType, int> figureCountMap)
        {
            //
            // Convert the incoming dictionary to a simple list of shapes to process in order.
            //
            List<ShapeType> shapes = ConvertShapeMapToList(figureCountMap);

            //
            // Construct the figure recursively.
            //
            List<FigSynthProblem> problems = SynthesizeFromTemplateAndFigures(shapes, TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA);

            //
            // Debug output of the problems.
            //
            foreach (FigSynthProblem problem in problems)
            {
                System.Diagnostics.Debug.WriteLine(problem.ToString());
            }

            //
            // Construct the problem so that it can be passed to the Solver.
            //
            ConstructProblemsToSolve(problems);
        }

        public static void SynthesizerMain()
        {
            //
            // Make up a list of shapes to compose.
            //
            Dictionary<ShapeType, int> figureCountMap = new Dictionary<ShapeType, int>();

            figureCountMap[ShapeType.SQUARE] = 1;
            figureCountMap[ShapeType.ISO_TRAPEZOID] = 1;


            //
            // Convert the incoming dictionary to a simple list of shapes to process in order.
            //
            List<ShapeType> shapes = ConvertShapeMapToList(figureCountMap);

            //
            // Construct the figure recursively.
            //
            List<FigSynthProblem> problems = SynthesizeFromTemplateAndFigures(shapes, TemplateType.ALPHA_MINUS_BETA);

            //
            // Debug output of the problems.
            //
            foreach (FigSynthProblem problem in problems)
            {
                System.Diagnostics.Debug.WriteLine(problem.ToString());
            }

            //
            // Construct the problem so that it can be passed to the Solver.
            //
            ConstructProblemsToSolve(problems);
        }

        //
        // Given the list of shapes AND a desired template, synthesize.
        //
        public static List<FigSynthProblem> SynthesizeFromTemplateAndFigures(List<ShapeType> shapeList, TemplateType type)
        {
            if (!VerifyInputParameters(shapeList, type)) return new List<FigSynthProblem>();

            return SynthesizeFromTemplate(shapeList, type);
        }

        //
        // Given a template: \alpha - \beta , etc.
        //
        public static List<FigSynthProblem> SynthesizeFromTemplate(List<ShapeType> shapeList, TemplateType type)
        {
            //
            // Construct the default shape for the bigger shape-type specified.
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapeList[0]);

            switch (type)
            {
                //
                // Two Shapes
                //
                case TemplateType.ALPHA_MINUS_BETA:                           // a - b
                    return ConstructSequentialSubtraction(shapeList);

                case TemplateType.ALPHA_PLUS_BETA:                            // a + b
                    throw new NotImplementedException();
                
                //
                // Three Shapes
                //
                case TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA:                 // a + b + c
                    throw new NotImplementedException();

                case TemplateType.ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN:  // a + (b - c)
                    throw new NotImplementedException();

                case TemplateType.LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA:  // (a + b) - c
                    throw new NotImplementedException();

                case TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA:               // a - b - c
                    return ConstructSequentialSubtraction(shapeList);

                case TemplateType.ALPHA_MINUS_BETA_PLUS_GAMMA:                // a - b + c
                    throw new NotImplementedException();

                case TemplateType.ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN: // a - (b - c)
                    return ConstructGroupedSubtraction(shapeList);
            }

            return new List<FigSynthProblem>();
        }

        //
        // Perform a - (b - c) =
        //                         1. a - b 
        //                         2. b - c
        //
        private static List<FigSynthProblem> ConstructGroupedSubtraction(List<ShapeType> shapes)
        {
            //
            // Construct  a - b
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);
            List<FigSynthProblem> aMinusB = SubtractShape(defaultLargeFigure, shapes[1]);

            //
            // For each of the aMinusB problems, the outer bounds is defined by a shape; subtract the new shape.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem top in aMinusB)
            {
                List<FigSynthProblem> bMinusC = SubtractShape(((top as BinarySynthOperation).rightProblem as UnarySynth).figure, shapes[2]);

                foreach (FigSynthProblem bc in bMinusC)
                {
                    newSynths.Add(FigSynthProblem.AppendFigureSubtraction(top, bc));
                }
            }

            return newSynths;
        }

        //
        // Perform a - b - ... - c
        //
        private static List<FigSynthProblem> ConstructSequentialSubtraction(List<ShapeType> shapes)
        {
            //
            // Construct base case
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);

            List<FigSynthProblem> startSynths = SubtractShape(defaultLargeFigure, shapes[1]);

            //
            // Recursive construction of each shape (level in the recursion).
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>(startSynths);
            for (int s = 2; s < shapes.Count; s++)
            {
                newSynths = ConstructSubtraction(newSynths, shapes[s]);
            }

            return newSynths;
        }

        //
        // Recursive case
        //
        private static List<FigSynthProblem> ConstructSubtraction(List<FigSynthProblem> synths, ShapeType shapeType)
        {
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();

            foreach (FigSynthProblem synth in synths)
            {
                newSynths.AddRange(Subtract(synth, shapeType));
            }

            return newSynths;
        }

        //
        // Perform subtraction with a synthesized problem.
        //
        private static List<FigSynthProblem> Subtract(FigSynthProblem synth, ShapeType shapeType)
        {
            List<AtomicRegion> openAtoms = synth.GetOpenRegions();

            //
            // Perform subtraction with all open regions with the new shape.
            //
            List<FigSynthProblem> newSubs = new List<FigSynthProblem>();
            foreach (AtomicRegion atom in openAtoms)
            {
                ShapeAtomicRegion shapeAtom = atom as ShapeAtomicRegion;

                if (shapeAtom != null)
                {
                    newSubs.AddRange(SubtractShape(shapeAtom.shape, shapeType));
                }
            }

            //
            // Combine the existent problem with the newly subtracted region.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem newSub in newSubs)
            {
                // Makes a copy out of the outer problem and appends the subtraction operation.
                newSynths.Add(FigSynthProblem.AppendAtomicSubtraction(synth, newSub));
            }

            //
            // Eliminate symmetric problems.
            //
            return FigSynthProblem.RemoveSymmetric(newSynths);
        }








        //
        // Given a set of points, can we construct any shape?
        // If we can construct a shape, we then have a resultant mess...
        //
        //public static List<FigSynthProblem> SubtractShape(List<Connection> conns, List<Point> points)
        //{
        //    List<FigSynthProblem> newAgg = new List<FigSynthProblem>();

        //    newAgg.AddRange(Triangle.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(IsoscelesTriangle.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(RightTriangle.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(EquilateralTriangle.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Kite.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Quadrilateral.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Trapezoid.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(IsoscelesTrapezoid.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Parallelogram.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Rectangle.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Rhombus.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Square.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Circle.SubtractShape(outerShape, conns, points));
        //    newAgg.AddRange(Sector.SubtractShape(outerShape, conns, points));

        //    return newAgg;
        //}


        //
        // Given a template: \alpha - \beta , etc.
        //
        public static void SynthesizeFromTemplate(TemplateType type)
        {
            switch (type)
            {
                // Two Shapes
                case TemplateType.ALPHA_MINUS_BETA:                           // a - b
                    break;
                case TemplateType.ALPHA_PLUS_BETA:                            // a + b
                    break;
                // Three Shapes
                case TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA:                 // a + b + c
                    break;
                case TemplateType.ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN:  // a + (b - c)
                    break;
                case TemplateType.LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA:  // (a + b) - c
                    break;
                case TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA:               // a - b - c
                    break;
                case TemplateType.ALPHA_MINUS_BETA_PLUS_GAMMA:                // a - b + c
                    break;
                case TemplateType.ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN: // a - (b - c)
                    break;
            }
        }

        //
        // Given a set of Figures, synthesize a new problem
        //
        public static void SynthesizeFromFigures(Dictionary<Figure, int> FigureMap)
        {
            if (FigureMap.Count > 3)
            {
                throw new ArgumentException("Cannot synthesize a figure with more than 3 Figures.");
            }

        }

        //
        // Given a set of points, can we construct any shape?
        // If we can construct a shape, we then have a resultant mess...
        //
        public static List<FigSynthProblem> SubtractShape(Figure outerShape, ShapeType type)
        {
            List<FigSynthProblem> problems = new List<FigSynthProblem>();

            List<Connection> conns = outerShape.MakeAtomicConnections();
            List<Point> points = outerShape.allComposingPoints;

            switch (type)
            {
                case ShapeType.TRIANGLE:
                    problems = Triangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.ISOSCELES_TRIANGLE:
                    problems = IsoscelesTriangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.RIGHT_TRIANGLE:
                    problems = RightTriangle.SubtractShape(outerShape, conns, points);
                    break;

                //case ShapeType.ISO_RIGHT_TRIANGLE:
                //    problems =  Triangle.SubtractShape(outerShape, conns, points);

                case ShapeType.EQUILATERAL_TRIANGLE:
                    problems = EquilateralTriangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.KITE:
                    problems = Kite.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.QUADRILATERAL:
                    problems = Quadrilateral.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.TRAPEZOID:
                    problems = Trapezoid.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.ISO_TRAPEZOID:
                    problems = IsoscelesTrapezoid.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.PARALLELOGRAM:
                    problems = Parallelogram.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.RECTANGLE:
                    problems = Rectangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.RHOMBUS:
                    problems = Rhombus.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.SQUARE:
                    problems = Square.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.CIRCLE:
                    problems = Circle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.SECTOR:
                    problems = Sector.SubtractShape(outerShape, conns, points);
                    break;
            }

            return problems;
        }

        //
        // Given a set of points, can we construct any shape?
        // If we can construct a shape, we then have a resultant mess...
        //
        public static List<FigSynthProblem> AppendShape(List<Point> points, ShapeType type)
        {
            switch (type)
            {
                case ShapeType.TRIANGLE:
                    return Triangle.AppendShape(points);

                case ShapeType.ISOSCELES_TRIANGLE:
                    return IsoscelesTriangle.AppendShape(points);

                case ShapeType.RIGHT_TRIANGLE:
                    return RightTriangle.AppendShape(points);

                //case ShapeType.ISO_RIGHT_TRIANGLE:
                //    return Triangle.AppendShape(points);

                case ShapeType.EQUILATERAL_TRIANGLE:
                    return EquilateralTriangle.AppendShape(points);

                case ShapeType.KITE:
                    return Kite.AppendShape(points);

                case ShapeType.QUADRILATERAL:
                    return Quadrilateral.AppendShape(points);

                case ShapeType.TRAPEZOID:
                    return Trapezoid.AppendShape(points);

                case ShapeType.ISO_TRAPEZOID:
                    return IsoscelesTrapezoid.AppendShape(points);

                case ShapeType.PARALLELOGRAM:
                    return Parallelogram.AppendShape(points);

                case ShapeType.RECTANGLE:
                    return Rectangle.AppendShape(points);

                case ShapeType.RHOMBUS:
                    return Rhombus.AppendShape(points);

                case ShapeType.SQUARE:
                    return Square.AppendShape(points);

                case ShapeType.CIRCLE:
                    return Circle.AppendShape(points);

                case ShapeType.SECTOR:
                    return Sector.AppendShape(points);
            }

            return new List<FigSynthProblem>();
        }

#if HARD_CODED_UI
        private static bool drawnAProblem = false;
#endif
        private static List<GeometryTestbed.FigSynthShadedAreaProblem> ConstructProblemsToSolve(List<FigSynthProblem> problems)
        {
            List<GeometryTestbed.FigSynthShadedAreaProblem> shadedAreaProblems = new List<GeometryTestbed.FigSynthShadedAreaProblem>();

            foreach (FigSynthProblem problem in problems)
            {
                GeometryTestbed.FigSynthShadedAreaProblem shadedProb = ConstructProblem(problem);
                shadedAreaProblems.Add(shadedProb);
                shadedProb.Run();

#if HARD_CODED_UI
                if (!drawnAProblem)
                {
                    UIProblemDrawer.getInstance().draw(shadedProb.MakeUIProblemDescription());
                    return shadedAreaProblems;
                }
                drawnAProblem = true;
#endif
            }

            return shadedAreaProblems;
        }

        private static int figCounter = 1;
        private static GeometryTestbed.FigSynthShadedAreaProblem ConstructProblem(FigSynthProblem problem)
        {
            GeometryTestbed.FigSynthShadedAreaProblem shadedArea = new GeometryTestbed.FigSynthShadedAreaProblem(true, true);
            
            //
            // Name the problem (uniquely).
            //
            shadedArea.SetName("Fig-Synthesized " + (figCounter++));

            //
            // Construct the points.
            //
            List<Point> points = problem.CollectPoints();
            shadedArea.SetPoints(points);

            //
            // Construct the collinear relationships.
            //
            List<Segment> segments;
            List<Collinear> collinear;

            AcquireCollinearAndSegments(problem.CollectSegments(), points, out segments, out collinear); 

            shadedArea.SetSegments(segments);
            shadedArea.SetCollinear(collinear);

            //
            // Construct circles.
            //
            shadedArea.SetCircles(problem.CollectCircles());

            //
            // Invoke the parser.
            //
            shadedArea.InvokeParser();

            //
            // Set the wanted atomic regions.
            //
            shadedArea.SetWantedRegions(shadedArea.GetRemainingRegionsFromParser(problem));

            //
            // Set the known values.
            // Acquire all of the givens using constant propagation for each figure construction.
            //
            shadedArea.SetKnowns(problem.AcquireKnowns());
            
            //
            // Set the problem given clauses.
            //
            List<GroundedClause> givens = problem.GetGivens();
            problem.GetMidpoints().ForEach(m => givens.Add(m));
            shadedArea.SetGivens(givens);

            //
            // Set the actual area of the solution (area of wanted regions).
            //
            shadedArea.SetSolutionArea(problem.GetCoordinateArea());

            return shadedArea;
        }

        //
        // Determine which of the inner Shape points apply to the given connection; add them as collinear points.
        //
        private static void AcquireCollinearAndSegments(List<Segment> segments, List<Point> points,
                                                        out List<Segment> trueSegments, out List<Collinear> collinear)
        {
            trueSegments = new List<Segment>();
            collinear = new List<Collinear>();

            // Maximal segments (containing no subsegments).
            List<Segment> maximal = new List<Segment>();

            //
            // Prune subsegments away
            //
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool max = true;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s1].HasSubSegment(segments[s2]))
                        {
                            max = false;
                            break;
                        }
                    }
                }
                if (max) maximal.Add(segments[s1]);
            }

            //
            // Place all points on appropriate segments for collinearity.
            //
            foreach (Segment max in maximal)
            {
                max.ClearCollinear();
                foreach (Point pt in points)
                {
                    if (max.PointLiesOn(pt))
                    {
                        max.AddCollinearPoint(pt);
                    }
                }
                
                //
                // Convert to collinear or straight-up segments.
                //
                if (max.collinear.Count > 2)
                {
                    collinear.Add(new Collinear(max.collinear));
                }
                else trueSegments.Add(max);
            }
        }
    }
}