using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class Equal : GenericDescriptor
    {
        private GenericFigure gs1 = null;
        private GenericFigure gs2 = null;

        public Equal(int id, string name, GenericFigure f1, GenericFigure f2) : base(id, name)
        {
            gs1 = f1;
            gs2 = f2;
        }

        public override  Boolean MayUnifyWith(GroundClause c)
        {
            return false;
        }

        public override List<GroundClause> Instantiate(GroundClause c)
        {
            return new List<GroundClause>();
        }
    }
}