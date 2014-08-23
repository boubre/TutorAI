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
        public static void SynthesizerMain(/* Dictionary<ShapeType, int> figureCountMap */)
        {
            //
            // Make up a list of shapes to compose.
            //
            Dictionary<ShapeType, int> figureCountMap = new Dictionary<ShapeType, int>();

            figureCountMap[ShapeType.RECTANGLE] = 1;
            figureCountMap[ShapeType.TRAPEZOID] = 1;
            figureCountMap[ShapeType.RIGHT_TRIANGLE] = 1;

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
            // Acquire all of the givens using constant propagation for each figure construction.
            //
            foreach (FigSynthProblem problem in problems)
            {
                problem.AcquireGivens();
            }
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
                    // return SubtractShape(defaultLargeFigure, shapeList[1], type);

                case TemplateType.ALPHA_PLUS_BETA:                            // a + b
                    break;
                
                //
                // Three Shapes
                //
                case TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA:                 // a + b + c
                    break;
                case TemplateType.ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN:  // a + (b - c)
                    break;
                case TemplateType.LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA:  // (a + b) - c
                    break;
                case TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA:               // a - b - c
                    // return ConstructSequentialSubtraction(shapeList, type);
                    break;
                case TemplateType.ALPHA_MINUS_BETA_PLUS_GAMMA:                // a - b + c
                    break;
                case TemplateType.ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN: // a - (b - c)
                    break;
            }

            return new List<FigSynthProblem>();
        }

        //
        // Perform a - b - ... - c
        //
        private static List<FigSynthProblem> ConstructSequentialSubtraction(List<ShapeType> shapes)
        {
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);

            //List<FigSynthProblem> synths =  SubtractShape(defaultLargeFigure, defaultLargeFigure.MakeAtomicConnections(),
            //                                              defaultLargeFigure.allComposingPoints, shapes[1]);
            List<FigSynthProblem> synths = SubtractShape(defaultLargeFigure, shapes[1]);

            //List

            for (int s = 2; s < shapes.Count; s++)
            {
                foreach (FigSynthProblem synth in synths)
                {
                }
            }

            return new List<FigSynthProblem>();
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
                    // newSubs.AddRange(SubtractShape(shapeAtom.shape, shapeAtom.shape.MakeAtomicConnections(), shapeAtom.shape.allComposingPoints, shapeType));
                }
            }

            //
            // Eliminate symmetric problems? Will they occur here?
            //

            //
            // Combine the existent problem with the newly subtracted region.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem problem in newSubs)
            {
                // Make a copy out of the outer problem.
            //    new FigSynthProblem(synth, problem);
            }

            return newSynths;
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
    }
}