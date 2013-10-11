using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class SSS : GenericAxiom
    {
        private GenericTriangle gt1 = null;
        private GenericTriangle gt2 = null;

        public SSS(int id, string name, GenericTriangle t1, GenericTriangle t2) : base(id, name)
        {
            gt1 = t1;
            gt2 = t2;
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