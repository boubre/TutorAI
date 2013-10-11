using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class AngleAddition : GenericAxiom
    {
        private GenericAngle ga1;
        private GenericAngle ga2;
        private GenericSegment gShared;

        public AngleAddition(int id, string name, GenericAngle a1, GenericAngle a2, GenericSegment shared) : base(id, name)
        {
            ga1 = a1;
            ga2 = a2;
            gShared = shared;
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
