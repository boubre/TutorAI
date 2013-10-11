using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class GenericCongruent : GenericDescriptor
    {
        private GenericFigure gf1 = null;
        private GenericFigure gf2 = null;

        public GenericCongruent(int id, string name, GenericFigure f1, GenericFigure f2) : base(id, name)
        {
            gf1 = f1;
            gf2 = f2;
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