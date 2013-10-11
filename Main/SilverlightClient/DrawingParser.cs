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
