using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public abstract class GenericFigure : GenericRule
    {
        public GenericFigure(int id, string name) : base(id, name) { }
    }
}
