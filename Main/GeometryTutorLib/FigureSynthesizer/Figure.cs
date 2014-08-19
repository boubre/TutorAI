using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract partial class Figure
    {
        // SHOULD BE ABSTRACT
        public virtual List<Figure> ComposeSubtraction(Figure betaInner) { return new List<Figure>(); }
        public virtual List<Figure> ComposeAddition(Figure betaOuter) { return new List<Figure>(); }

        public List<Point> vertices { get; protected set; }
        public List<Point> snapToPoints { get; protected set; }
        public List<Point> allPoints { get; protected set; }




    }
}
