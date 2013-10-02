using System.Diagnostics;
using DynamicGeometry;
using GeometryTutorLib.AbstractSyntax;

namespace LiveGeometry
{
    public class DrawingParser
    {
        private DynamicGeometry.FigureList figures;

        public DrawingParser(DynamicGeometry.FigureList figures)
        {
            this.figures = figures;
        }

        public void ParseDrawing()
        {
            foreach (IFigure f in figures)
            {
                Debug.WriteLine(f.ToString());
            }
        }
    }
}
