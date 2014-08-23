using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib
{
    public enum TemplateType
    {
        // Two Shapes
        ALPHA_MINUS_BETA,                           // a - b
        ALPHA_PLUS_BETA,                            // a + b

        // Marker between 2 and 3 elements in the template
        DEMARCATION,

        // Three Shapes
        ALPHA_PLUS_BETA_PLUS_GAMMA,                 // a + b + c
        ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN,  // a + (b - c)
        LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA,  // (a + b) - c
        ALPHA_MINUS_BETA_MINUS_GAMMA,               // a - b - c
        ALPHA_MINUS_BETA_PLUS_GAMMA,                // a - b + c
        ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN, // a - (b - c)
    }

    public enum ShapeType
    {
        TRIANGLE = 0,
        ISOSCELES_TRIANGLE = 1,
        RIGHT_TRIANGLE = 2,
        ISO_RIGHT_TRIANGLE = 3,
        EQUILATERAL_TRIANGLE = 4,

        TRI_DEMARCATION,

        QUADRILATERAL = 5,
        KITE = 6,
        TRAPEZOID = 7,
        ISO_TRAPEZOID = 8,
        PARALLELOGRAM = 9,
        RECTANGLE = 10,
        RHOMBUS = 11,
        SQUARE = 12,

        QUAD_DEMARCATION,

        CIRCLE = 13,
        SECTOR = 14
    }

    public static partial class FigureSynthesizerMain
    {

        //
        // Converts the shape map to a list of shapes to process IN ORDER.
        //
        private static List<ShapeType> ConvertShapeMapToList(Dictionary<ShapeType, int> figureCountMap)
        {
            List<ShapeType> shapes = new List<ShapeType>();

            foreach (KeyValuePair<ShapeType, int> pair in figureCountMap)
            {
                // Add the shape X number of times to the list.
                for (int s = 0; s < pair.Value; s++)
                {
                    shapes.Add(pair.Key);
                }
            }

            //
            // This should change for experimental purposes...
            //
            // Sort the shapes based on the order defined by the ShapeType enumeration.
            //
            //
            shapes.Sort();
            shapes.Reverse();

            return shapes;
        }

        //
        // Ensure that the input set of shapes meets the desired application and template parameters.
        //
        public static bool VerifyInputParameters(List<ShapeType> shapeList, TemplateType type)
        {
            // We have an artificial limitation in the number of figures we combine.
            if (shapeList.Count > 3) throw new ArgumentException("Cannot synthesize a figure with more than 3 Figures.");

            if (type.CompareTo(TemplateType.DEMARCATION) < 0 && shapeList.Count != 2)
            {
                throw new ArgumentException("Expected two figures with a synthesis dictacted by template: " + type);
            }

            if (type.CompareTo(TemplateType.DEMARCATION) > 0 && shapeList.Count < 3)
            {
                throw new ArgumentException("Expected three figures with a synthesis dictacted by template: " + type);
            }

            return true;
        }
    }
}