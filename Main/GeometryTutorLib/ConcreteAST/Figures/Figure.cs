using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Figure : GroundedClause
    {
        protected List<Figure> superFigures;
        
        protected Figure()
        {
            superFigures = new List<Figure>();
        }

        public bool isShared()
        {
            return superFigures.Count > 1;
        }

        public List<Figure> getSuperFigures()
        {
            return superFigures;
        }
    }
}
