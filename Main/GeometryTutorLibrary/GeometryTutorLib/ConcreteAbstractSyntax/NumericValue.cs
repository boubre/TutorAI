using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class NumericValue : ArithmeticNode
    {
        public int value { get; private set; }

        public NumericValue() { value = 0; }
        
        public NumericValue(int v) { value = v; }

        public override string ToString()
        {
            return value.ToString();
        }

        public override bool Equals(Object obj)
        {
            NumericValue valObj = obj as NumericValue;
            if (valObj == null) return false;
            return value == valObj.value;
        }
    }
}