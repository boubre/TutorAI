using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AtomicRegion
    {
        public List<GeometryTutorLib.ConcreteAST.Figure> constituentShapes { get; private set; }

        public AtomicRegion()
        {
            constituentShapes = new List<ConcreteAST.Figure>();
        }

        public bool AddBasicShape(GeometryTutorLib.ConcreteAST.Figure shape)
        {
            if (constituentShapes.Contains(shape)) return false;

            constituentShapes.Add(shape);

            return false;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            AtomicRegion thatAtom = obj as AtomicRegion;
            if (thatAtom == null) return false;

            return Utilities.EqualSets<GeometryTutorLib.ConcreteAST.Figure>(constituentShapes, thatAtom.constituentShapes);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("AtomicRegion: {");

            foreach (GeometryTutorLib.ConcreteAST.Figure shape in constituentShapes)
            {
                str.Append(shape + ", ");
            }

            str.AppendLine(" }");

            return str.ToString();
        }
    }
}
