using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class GenericSegment : GenericFigure
    {
        private GenericPoint gp1 = null;
        private GenericPoint gp2 = null;

        public GenericSegment(int id, string name, GenericPoint p1, GenericPoint p2) : base(id, name)
        {
            gp1 = p1;
            gp2 = p2;
        }

        public override Boolean MayUnifyWith(GroundClause c)
        {
            return false;
        }

        public override List<GroundClause> Instantiate(GroundClause c)
        {
            return new List<GroundClause>();
        }
    }
}