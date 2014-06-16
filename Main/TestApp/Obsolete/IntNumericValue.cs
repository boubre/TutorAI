using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class NumericValue : NumericValue
    {
        public NumericValue() : base() { }
        public NumericValue(int v) : base(v) {}
        public override string ToString() { return value.ToString(); }

        public override bool Equals(Object obj)
        {
            NumericValue valObj = obj as NumericValue;
            if (valObj == null) return false;

            return base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}