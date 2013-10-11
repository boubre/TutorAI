using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class StraightAngle : GenericDefinition
    {
        private GenericCollinear coll = null;

        public StraightAngle(int id, string name, GenericCollinear c) : base(id, name)
        {
            coll = c;
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