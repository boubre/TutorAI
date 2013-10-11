using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class Intersect : GenericDescriptor
    {
        private GenericSegment gs1 = null;
        private GenericSegment gs2 = null;

        public Intersect(int id, string name, GenericSegment s1, GenericSegment s2) : base(id, name)
        {
            gs1 = s1;
            gs2 = s2;
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