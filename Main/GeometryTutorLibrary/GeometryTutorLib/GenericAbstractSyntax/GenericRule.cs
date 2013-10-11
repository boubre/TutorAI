﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GenericAbstractSyntax
{
    public abstract class GenericRule
    {
        int ruleId;        // Unique ID for each generic
        string commonName; // Name of definition, theorem, or axiom (common vernacular name).

        GenericRule(int id, string name)
        {
            ruleId = id;
            commonName = name;
        }

        //Below lone commented out since it results in temp build errors
        //public abstract Boolean Unify(ConcreteFigure cf);
    }
}