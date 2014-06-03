using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    //
    // An equation of the form: target = a +- b +- c +- ... +- d
    //
    public class ComplexRegionEquation
    {
        public Region target { get; private set; }
        public Exp expr { get; private set; }

        public class Exp
        {
            public Region leftRegion;
            public Region rightRegion;

            // May be null if 
            public Exp leftExp;
            public Exp rightExp;
            public OperationT op;

            public Exp(Region ell, OperationT op, Region r)
            {
                leftExp = null;
                rightExp = null;

                leftRegion = ell;
                rightRegion = r;
                this.op = op;
            }
        }

        public ComplexRegionEquation(SimpleRegionEquation simple) : base()
        {
            target = simple.target;

            expr = new Exp(simple.bigger, simple.op, simple.smaller);
        }
    }
}