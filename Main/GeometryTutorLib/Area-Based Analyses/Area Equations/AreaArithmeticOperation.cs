using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaArithmeticOperation : AreaArithmeticNode
    {
        protected Region leftExp;
        protected Region rightExp;

        public AreaArithmeticOperation() : base() { }

        public AreaArithmeticOperation(Region l, Region r) : base()
        {
            leftExp = l;
            rightExp = r;
        }

        //public override List<AtomicRegion> CollectTerms()
        //{
        //    List<AtomicRegion> list = new List<AtomicRegion>();

        //    //list.AddRange(leftExp.CollectTerms());

        //    //foreach (Region gc in rightExp.CollectTerms())
        //    //{
        //    //    Region copyGC = gc.DeepCopy();

        //    //    list.Add(copyGC);
        //    //}

        //    return list;
        //}

        //public override bool Contains(Region newG)
        //{
        //    return leftExp.Contains(newG) || rightExp.Contains(newG);
        //}

        //public override void Substitute(Region toFind, Region toSub)
        //{
        //    if (leftExp.Equals(toFind))
        //    {
        //        leftExp = toSub;
        //    }
        //    else
        //    {
        //        leftExp.Substitute(toFind, toSub);
        //    }

        //    if (rightExp.Equals(toFind))
        //    {
        //        rightExp = toSub;
        //    }
        //    else
        //    {
        //        rightExp.Substitute(toFind, toSub);
        //    }
        //}

        //// Make a deep copy of this object
        //public override Region DeepCopy()
        //{
        //    ArithmeticOperation other = (ArithmeticOperation)(this.MemberwiseClone());
        //    other.leftExp = leftExp.DeepCopy();
        //    other.rightExp = rightExp.DeepCopy();

        //    return other;
        //}

        //public override string ToString()
        //{
        //    return "(" + leftExp.ToString() + " + " + rightExp.ToString() + ")";
        //}

        public override bool Equals(Object obj)
        {
            AreaArithmeticOperation aao = obj as AreaArithmeticOperation;
            if (aao == null) return false;

            return leftExp.Equals(aao.leftExp) && rightExp.Equals(aao.rightExp) ||
                   leftExp.Equals(aao.rightExp) && rightExp.Equals(aao.leftExp) && base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}