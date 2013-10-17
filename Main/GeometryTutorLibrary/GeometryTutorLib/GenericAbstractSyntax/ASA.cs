using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public class ASA : CongruentTriangleAxiom
    {
        public Boolean MayUnifyWith(GroundedClause c)
        {
            return c is ConcreteTriangle;
        }

        public List<GroundedClause> Instantiate(GroundedClause c)
        {
            return new List<GroundedClause>();
        }
    }
}