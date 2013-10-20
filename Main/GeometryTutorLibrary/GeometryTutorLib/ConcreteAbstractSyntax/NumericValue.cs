using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class NumericValue : ArithmeticNode
    {
        public int value { get; private set; }

        public NumericValue() : base() { value = 0; }

        public NumericValue(int v) : base() { value = v; }

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

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        // Make a deep copy of this object; this is really a shallow copy since it
        // is only an integer wrapper
        public override GroundedClause DeepCopy()
        {
            return (NumericValue)this.MemberwiseClone();
        }
    }
}