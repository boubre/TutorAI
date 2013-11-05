using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public abstract class ConcreteFigure : GroundedClause
    {
        protected List<ConcreteFigure> superFigures;
                protected ConcreteFigure()
        {
            superFigures = new List<ConcreteFigure>();
        }

        public bool isShared()
        {
            return superFigures.Count > 1;
        }

        public List<ConcreteFigure> getSuperFigures()
        {
            return superFigures;
        }
    }
}
