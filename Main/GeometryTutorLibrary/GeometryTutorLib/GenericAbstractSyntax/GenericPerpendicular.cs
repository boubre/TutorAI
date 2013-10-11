using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class GenericPerpendicular : GenericDefinition
    {
        private GenericSegment gs1 = null;
        private GenericSegment gs2 = null;
        private GenericPoint gIntersection = null;

        public GenericPerpendicular(int id, string name, GenericSegment s1, GenericSegment s2, GenericPoint p) : base(id, name)
        {
            gs1 = s1;
            gs2 = s2;
            gIntersection = p;
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