using System.Collections.Generic;
using System.Diagnostics;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace LiveGeometry
{
    public class DrawingParser
    {
        private Drawing drawing;
        private Dictionary<IFigure, GroundedClause> parsed;

        public DrawingParser(Drawing drawing)
        {
            this.drawing = drawing;
        }

        public List<GroundedClause> ParseDrawing()
        {
            parsed = new Dictionary<IFigure, GroundedClause>();
            foreach (IFigure figure in drawing.Figures)
                parse(figure);

            foreach (KeyValuePair<IFigure, GroundedClause> kvP in parsed)
            {
                Debug.WriteLine(kvP.Value);
                Debug.WriteLine("--------------------");
            }
            Debug.WriteLine("=====END=====");
            return null;
        }

        private void parse(IFigure figure)
        {
            if (parsed.ContainsKey(figure)) return;
            else if (figure is IPoint) parse(figure as IPoint);
            else if (figure is ILine) parse(figure as ILine);
            else if (figure is Polygon) parse(figure as PolygonBase);
            else if (figure is RegularPolygon) parse(figure as RegularPolygon);
        }

        private void parse(IPoint pt)
        {
            parsed.Add(pt, new ConcretePoint(pt.Name, pt.Coordinates.X, pt.Coordinates.Y));
        }

        private void parse(ILine line)
        {
            IPoint p1 = line.Dependencies.FindPoint(line.Coordinates.P1, 0);
            IPoint p2 = line.Dependencies.FindPoint(line.Coordinates.P2, 0);
            parse(p1 as IFigure);
            parse(p2 as IFigure);
            parsed.Add(line, new ConcreteSegment(parsed[p1] as ConcretePoint, parsed[p2] as ConcretePoint));
        }

        private void parse(PolygonBase pgon)
        {
            if (pgon.VertexCoordinates.Length == 3)
            {
                //Find verticies
                System.Windows.Point[] pts = pgon.VertexCoordinates;
                IPoint[] iPts = new IPoint[3];
                for (int i = 0; i < 3; i++)
                {
                    iPts[i] = pgon.Dependencies.FindPoint(pts[i], 0);
                    parse(iPts[i] as IFigure);
                }
                
                //genereate sides
                ILine[] lines = new ILine[3];
                ConcreteSegment[] csegs = new ConcreteSegment[3];
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    lines[i] = drawing.Figures.FindLine(iPts[i], iPts[j]);
                    if (lines[i] == null) lines[i] = Factory.CreateSegment(drawing, new[] { iPts[i], iPts[j] });
                    parse(lines[i] as IFigure);
                    csegs[i] = parsed[lines[i]] as ConcreteSegment;
                }

                //is it isosceles?
                bool isosceles = false;
                for (int i = 0; i < 3; i++)
                    isosceles = isosceles || (csegs[i].Length == csegs[(i+1)%3].Length);

                if (isosceles)
                    parsed.Add(pgon, new ConcreteIsoscelesTriangle(csegs[0], csegs[1], csegs[2]));
                else
                    parsed.Add(pgon, new ConcreteTriangle(csegs[0], csegs[1], csegs[2]));
            }
        }

        private void parse(RegularPolygon rgon)
        {
            if (rgon.NumberOfSides == 3)
            {
                IPoint center = rgon.Dependencies.FindPoint(rgon.Center, 0);
                IPoint vertex = rgon.Dependencies.FindPoint(rgon.Vertex, 0);

                //Genereate vertex points knowing that the polygon is regular
                IPoint[] pts = new IPoint[3];
                double radius = Math.Distance(vertex.Coordinates.X, center.Coordinates.X, vertex.Coordinates.Y, center.Coordinates.Y);
                double initAngle = Math.GetAngle(
                    new System.Windows.Point(center.Coordinates.X, center.Coordinates.Y), 
                    new System.Windows.Point(vertex.Coordinates.X, vertex.Coordinates.Y));
                double increment = Math.DOUBLEPI / 3.0;
                for (int i = 0; i < 2; i++)
                {
                    double angle = initAngle + (i + 1) * increment;
                    double X = center.Coordinates.X + radius * System.Math.Cos(angle);
                    double Y = center.Coordinates.Y + radius * System.Math.Sin(angle);
                    System.Windows.Point newPt = new System.Windows.Point(X, Y);
                    pts[i] = drawing.Figures.FindPoint(newPt, 0);
                    if (pts[i] == null) pts[i] = Factory.CreateFreePoint(drawing, newPt);
                    parse(pts[i] as IFigure);
                }
                pts[2] = vertex;
                parse(pts[2] as IFigure);

                //generate sides from vertices
                ILine[] lines = new ILine[3];
                ConcreteSegment[] csegs = new ConcreteSegment[3];
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    lines[i] = drawing.Figures.FindLine(pts[i], pts[j]);
                    if (lines[i] == null) lines[i] = Factory.CreateSegment(drawing, new[] { pts[i], pts[j] });
                    parse(lines[i] as IFigure);
                    csegs[i] = parsed[lines[i]] as ConcreteSegment;
                }

                parsed.Add(rgon, new ConcreteEquilateralTriangle(csegs[0], csegs[1], csegs[2]));
            }
        }
    }
}
