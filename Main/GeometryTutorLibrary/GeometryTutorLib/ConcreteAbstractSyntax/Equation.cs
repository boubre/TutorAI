using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public abstract class Equation : ArithmeticNode
    {
        public GroundedClause lhs { get; private set; }
        public GroundedClause rhs { get; private set; }
        protected int numSubstitutions; // The number of substitution 'levels' required to create this equation; this is an attempt to cut off infinite substitutions
        public void AddSubstitutionLevel() { numSubstitutions++; }

        public Equation() { }

        public Equation(GroundedClause l, GroundedClause r)
        {
            lhs = l;
            rhs = r;
            numSubstitutions = 0;
        }

        public Equation(GroundedClause l, GroundedClause r, string just)
        {
            lhs = l;
            rhs = r;
            justification = just;
            numSubstitutions = 0;
        }

        public override void Substitute(GroundedClause toFind, GroundedClause toSub)
        {
            if (lhs.Equals(toFind))
            {
                lhs = toSub;
            }
            else
            {
                lhs.Substitute(toFind, toSub);
            }

            if (rhs.Equals(toFind))
            {
                rhs = toSub;
            }
            else
            {
                rhs.Substitute(toFind, toSub);
            }
        }

        public override bool Contains(GroundedClause target)
        {
            //// Check if we have atomic nodes
            //if (lhs is ConcreteAngle && ((ConcreteAngle)lhs).Equals(target)) return true;
            //if (rhs is ConcreteAngle && ((ConcreteAngle)rhs).Equals(target)) return true;

            //if (lhs is ConcreteSegment && ((ConcreteSegment)lhs).Equals(target)) return true;
            //if (rhs is ConcreteSegment && ((ConcreteSegment)rhs).Equals(target)) return true;

            // If a composite node, check accordingly; this will return false if they are atomic
            return lhs.Contains(target) || rhs.Contains(target);
        }

        //
        // Determines if the equation has one side being atomic; no compound expressions
        // returns -1 (left is atomic), 0 (neither atomic), 1 (right is atomic)
        // both atomic: 2
        //
        public const int LEFT_ATOMIC = -1;
        public const int NONE_ATOMIC = 0;
        public const int RIGHT_ATOMIC = 1;
        public const int BOTH_ATOMIC = 2;
        public int OneSideAtomic()
        {
            bool leftIs = lhs is ConcreteAngle || lhs is ConcreteSegment;
            bool rightIs = rhs is ConcreteAngle || rhs is ConcreteSegment;

            if (leftIs && rightIs) return BOTH_ATOMIC;
            if (!leftIs && !rightIs) return NONE_ATOMIC;
            if (leftIs) return LEFT_ATOMIC;
            if (rightIs) return RIGHT_ATOMIC;

            return NONE_ATOMIC;
        }

        public abstract Equation MakeCopy();

        public Equation Flatten()
        {
            Equation flattened = this.MakeCopy();

            

            return flattened;
        }

        public override string ToString()
        {
            return "Equation(" + lhs + " = " + rhs + "): " + justification;
        }
    }
}