using System.Collections.Generic;
using System.Diagnostics;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace LiveGeometry
{
    public class DrawingParser
    {
        private Drawing drawing;

        public DrawingParser(Drawing drawing)
        {
            this.drawing = drawing;
        }

        public List<GroundedClause> ParseDrawing()
        {
            Dictionary<IFigure, GroundedClause> parsed = new Dictionary<IFigure, GroundedClause>();
            foreach (IFigure figure in drawing.Figures)
            {
                if (figure is IPoint)
                {
                    IPoint pt = figure as IPoint;
                    parsed.Add(pt, new ConcretePoint(pt.ToString(), pt.Coordinates.X, pt.Coordinates.Y));
                }
                else if (figure is ILine)
                {
                    ILine line = figure as ILine;
                }
                else if (figure is Polygon)
                {
                    PolygonBase pgon = figure as PolygonBase;
                }
                else if (figure is RegularPolygon)
                {
                    RegularPolygon rgon = figure as RegularPolygon;
                }
            }

            foreach (KeyValuePair<IFigure, GroundedClause> kvP in parsed)
            {
                Debug.WriteLine(kvP.Value);
                Debug.WriteLine("--------------------");
            }
            Debug.WriteLine("=====END=====");
            return null;
                /*else if (figureStream.Current is RegularPolygon)
                {
                    RegularPolygon rp = figureStream.Current as RegularPolygon;
                    Point vertex = parseStack.Pop() as Point;
                    Point center = parseStack.Pop() as Point;

                    Point[] points = new Point[3];
                    double radius = LineLength(center, vertex);
                    double initAngle = DynamicGeometry.Math.GetAngle(new System.Windows.Point(center.X, center.Y), new System.Windows.Point(vertex.X, vertex.Y));
                    double increment = DynamicGeometry.Math.DOUBLEPI / 3.0;
                    for (int i = 0; i < 2; i++)
                    {
                        double angle = initAngle + (i + 1) * increment;
                        double X = center.X + radius * System.Math.Cos(angle);
                        double Y = center.Y + radius * System.Math.Sin(angle);
                        points[i] = new Point(X, Y);
                    }
                    points[2] = vertex;

                    Line[] lines = new Line[3];
                    for (int i = 0; i < 3; i++)
                        lines[i] = new Line(points[i], points[(i + 1) % 3], LineType.Segment);

                    parseStack.Push(new Triangle(lines[0], lines[1], lines[2], TriangleType.Equilateral));
                } */
        }
    }
}
