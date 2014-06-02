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
        public Expr expr { get; private set; }

        public ComplexRegionEquation(Region tar, Region r) : base()
        {
            target = tar;
            expr = new Unary(r);
        }

        public ComplexRegionEquation(SimpleRegionEquation simple) : base()
        {
            target = simple.target;

            expr = new Binary(simple.bigger, simple.op, simple.smaller);
        }

        public ComplexRegionEquation(ComplexRegionEquation complex) : base()
        {
            target = complex.target;

            expr = complex.expr.Copy();
        }

        public void Substitute(Region toFind, Expr toSub)
        {
            expr.Substitute(toFind, toSub);
        }

        public override string ToString()
        {
            return target.ToString() + " = \n" + expr.ToString();
        }

        public int Length
        {
            get
            {
                // If this is an indentity equation (base case in other code),
                // indicate a length of infinity.
                if (expr is Unary)
                {
                    if (target.Equals((expr as Unary).theRegion)) return int.MaxValue;
                }

                // Otherwise, count the number of regions.
                return expr.NumRegions();
            }
        }


        //
        // Class hierarchy to handle expressions (one side of an equation).
        //
        public abstract class Expr
        {
            public Expr() {}
            public abstract Expr Copy();
            public abstract Expr Substitute(Region toFind, Expr toSub);
            public abstract int NumRegions();
        }

        public class Unary : Expr
        {
            public Region theRegion;

            public Unary(Region r) : base() { theRegion = r; }
            public Unary(Unary e) : base() { theRegion = e.theRegion; }
            public override Expr Copy() { return new Unary(this); }
            public override Expr Substitute(Region toFind, Expr toSub) { return theRegion.Equals(toFind) ? toSub : this; }
            public override string ToString() { return theRegion.ToString(); }
            public override int NumRegions() { return 1; }
        }

        public class Binary : Expr
        {
            public Expr leftExp;
            public Expr rightExp;
            public OperationT op;

            public Binary(Expr ell, OperationT op, Expr right) : base()
            {
                leftExp = ell;
                this.op = op;
                rightExp = right;
            }
            public Binary(Region ell, OperationT op, Region r) : this(new Unary(ell), op, new Unary(r)) { }
            public Binary(Binary b) : this (b.leftExp.Copy(), b.op, b.rightExp.Copy()) {}

            public override Expr Copy() { return new Binary(this); }
            public override Expr Substitute(Region toFind, Expr toSub)
            {
                Expr newLeft = leftExp.Substitute(toFind, toSub);
                Expr newRight = leftExp.Substitute(toFind, toSub);
                return new Binary(newLeft, this.op, newRight);
            }
            public override string ToString()
            {
                string operation = op == OperationT.ADDITION ? "+" : "-";

                return "(" + leftExp.ToString() + operation + "\n" + rightExp.ToString() + ")"; 
            }
            public override int NumRegions() { return leftExp.NumRegions() + rightExp.NumRegions(); }
        }
    }
}