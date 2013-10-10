using System.Collections.Generic;
using System.Diagnostics;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace LiveGeometry
{
    public class DrawingParser
    {
        private DynamicGeometry.FigureList figures;

        public DrawingParser(DynamicGeometry.FigureList figures)
        {
            this.figures = figures;
        }

        public GeometryTutorLib.ConcreteAbstractSyntax.Drawing ParseDrawing()
        {
            IEnumerator<IFigure> figureStream = figures.GetEnumerator();
            Stack<GeometryTutorLib.ConcreteAbstractSyntax.Figure> parseStack = new Stack<GeometryTutorLib.ConcreteAbstractSyntax.Figure>();
            while (figureStream.MoveNext())
            {
                if (figureStream.Current is IPoint)
                {
                    IPoint pt = figureStream.Current as IPoint;
                    parseStack.Push(new Point(pt.Coordinates.X, pt.Coordinates.Y));
                }
                else if (figureStream.Current is DynamicGeometry.Polygon)
                {
                    PolygonBase p = figureStream.Current as PolygonBase;
                    int nVerticies = p.VertexCoordinates.Length;
                    if (nVerticies == 3)
                    {
                        Point[] points = new Point[3];
                        for (int i = 0; i < 3; i++) 
                            points[i] = parseStack.Pop() as Point;
                        Line[] lines = new Line[3];
                        double[] lineLengths = new double[3];
                        for (int i = 0; i < 3; i++)
                        {
                            lines[i] = new Line(points[i], points[(i + 1) % 3], LineType.Segment);
                            lineLengths[i] = LineLength(lines[i]);
                        }

                        bool isosceles = false;
                        bool right = false;
                        for (int i = 0; i < 3; i++)
                        {
                            int j = (i + 1) % 3;
                            isosceles = isosceles || (lineLengths[i] == lineLengths[j]);
                            Tuple<double, double> v1 = new Tuple<double, double>(lines[i].Point1.X - lines[i].Point2.X, lines[i].Point1.Y - lines[i].Point2.Y);
                            Tuple<double, double> v2 = new Tuple<double, double>(lines[j].Point1.X - lines[j].Point2.X, lines[j].Point1.Y - lines[j].Point2.Y);
                            right = right || (v1.Item1 * v2.Item1 + v1.Item2 * v2.Item2) == 0;
                        }

                        TriangleType type;
                        if (isosceles && right) type = TriangleType.IsoscelesRight;
                        else if (isosceles) type = TriangleType.Isosceles;
                        else if (right) type = TriangleType.Right;
                        else type = TriangleType.Scalene;

                        parseStack.Push(new Triangle(lines[0], lines[1], lines[2], type));

                        //eat unneeded input 
                        for (int i = 0; i < 3; i++) 
                            figureStream.MoveNext();
                    }
                }
                else if (figureStream.Current is RegularPolygon)
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
                }
                else if (figureStream.Current is ILine)
                {
                    ILine l = figureStream.Current as ILine;
                    LineType type;
                    if (l is Ray) type = LineType.Ray;
                    else if (l is Segment) type = LineType.Segment;
                    else type = LineType.Line;

                    parseStack.Push(new Line(parseStack.Pop() as Point, parseStack.Pop() as Point, type));
                }
            }

            GeometryTutorLib.ConcreteAbstractSyntax.FigureList fl = null;

            while (!parseStack.IsEmpty())
                fl = new GeometryTutorLib.ConcreteAbstractSyntax.FigureList(parseStack.Pop(), fl);

            GeometryTutorLib.ConcreteAbstractSyntax.Drawing root = new GeometryTutorLib.ConcreteAbstractSyntax.Drawing(fl);
            return root;
        }

        private double LineLength(Line l) 
        {
            return LineLength(l.Point1, l.Point2);
        }

        private double LineLength(Point p1, Point p2)
        {
            return System.Math.Sqrt(System.Math.Pow((p1.X - p2.X), 2) + System.Math.Pow((p1.Y - p2.Y), 2));
        }
    }
}
