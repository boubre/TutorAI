using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class InMiddle : GenericDescriptor
    {
        private GenericPoint gp = null;
        private GenericSegment gs = null;

        public InMiddle(int id, string name, GenericPoint p, GenericSegment s) : base(id, name)
        {
            gp = p;
            gs = s;
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
