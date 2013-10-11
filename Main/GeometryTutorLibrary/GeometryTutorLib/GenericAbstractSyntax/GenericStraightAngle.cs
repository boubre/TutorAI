using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class GenericStraightAngle : GenericDefinition
    {
        private GenericPoint gp1 = null;
        private GenericPoint gp2 = null;
        private GenericPoint gp3 = null;

        public GenericStraightAngle(int id, string name, GenericPoint p1, GenericPoint p2, GenericPoint p3) : base(id, name)
        {
            gp1 = p1;
            gp2 = p2;
            gp3 = p3;
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