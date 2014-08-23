using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract partial class FigSynthProblem
    {
        public abstract List<Constraint> GetConstraints();
        public abstract List<Segment> GetAreaVariables();
        public abstract double EvaluateArea(KnownMeasurementsAggregator known);

        //public Figure outerShape { get; protected set; }
        //public void SetOuterShape(Figure outer) { outerShape = outer; }

        //public List<Constraint> outerShapeConstraints { get; protected set; }
        //public void SetOuterShapeConstraints(List<Constraint> cs) { outerShapeConstraints = cs; }

        // The allowable regions that we might insert a figure.
        protected List<AtomicRegion> openRegions;
        public List<AtomicRegion> GetOpenRegions() { return openRegions; }
        public void SetOpenRegions(List<AtomicRegion> rs) { openRegions = rs; }

        //
        // Prune the list of synth objects to only asymmetric scenarios.
        //
        public static List<FigSynthProblem> RemoveSymmetric(List<FigSynthProblem> composed)
        {
            // Base case.
            if (!composed.Any()) return composed;

            // The eventual list of asymmetric scenarios.
            List<FigSynthProblem> unique = new List<FigSynthProblem>();

            // Prime the list of unique synths with the first one.
            unique.Add(composed[0]);

            for (int s = 1; s < composed.Count; s++)
            {
                bool asymmetric = true;
                foreach (FigSynthProblem uniqueAgg in unique)
                {
                    if (uniqueAgg.IsSymmetricTo(composed[s]))
                    {
                        asymmetric = false;
                        break;
                    }
                }

                if (asymmetric) unique.Add(composed[s]);
            }

            return unique;
        }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public virtual bool IsSymmetricTo(FigSynthProblem that)
        {
            // The number of atomic shapes is consistent
            if (this.openRegions.Count != that.openRegions.Count) return false;

            // We must have a 1-1 mapping of the remaining atomic shapes. This is in terms of congruence.
            int numRegions = openRegions.Count;
            bool[] marked = new bool[numRegions];
            foreach (AtomicRegion thisAtom in openRegions)
            {
                bool foundAtom = false;
                for (int a = 0; a < numRegions; a++)
                {
                    if (!marked[a])
                    {
                        if (thisAtom.CoordinateCongruent(that.openRegions[a]))
                        {
                            marked[a] = true;
                            foundAtom = true;
                            break;
                        }
                    }
                }

                if (!foundAtom) return false;
            }

            return true;
        }

        //
        // We have a set of constraints associated with the figure.
        // Also associated is a set of variables in which constraints are defined.
        // 1) Randomly choose one of the variables that defines the area formula, define it by its coordinate-based value.
        // 2) Push it through the constant propagator.
        // 3) From the list of variables, remove which of them that are now known.
        // 4) Repeat until the list of variables is empty.
        //
        public List<Segment> AcquireGivens()
        {
            // Acquire all unknown variables required to calculate the area.
            List<Segment> unknownAreaVars = this.GetAreaVariables();

            // The constraints for this problem.
            List<Constraint> constraints = this.GetConstraints();

            // The values we must state to the user in order to solve the problem.
            List<Segment> assumptions = new List<Segment>();

            //
            // Loop until all variables are known.
            //
            KnownMeasurementsAggregator known = new KnownMeasurementsAggregator();
            while (unknownAreaVars.Any())
            {
                // Acquire a new assumption.
                Segment newAssumption = unknownAreaVars[0];

                // remove that assumption since it is now known; add as an assumption.
                unknownAreaVars.RemoveAt(0);
                assumptions.Add(newAssumption);

                // Set this value as known with its intrinsic (corrdinate-based) length.
                known.AddSegmentLength(newAssumption, newAssumption.Length);

                // Propagate the new information through the constraints.
                ConstantPropagator.Propogate(known, constraints);

                // Check if any of the unknown variables are now known through constant propagation.
                unknownAreaVars = AcquireCurrentUnknowns(known, unknownAreaVars);
            }

            return assumptions;
        }

        //
        // Filter the list of unknowns by any new information.
        //
        private List<Segment> AcquireCurrentUnknowns(KnownMeasurementsAggregator known, List<Segment> unknowns)
        {
            List<Segment> newUnknowns = new List<Segment>();

            foreach (Segment unknown in unknowns)
            {
                if (known.GetSegmentLength(unknown) < 0) newUnknowns.Add(unknown);
            }

            return newUnknowns;
        }


    }

    public abstract class BinarySynthOperation : FigSynthProblem
    {
        public FigSynthProblem leftProblem { get; protected set; }
        public FigSynthProblem rightProblem { get; protected set; }

        public BinarySynthOperation(FigSynthProblem ell, FigSynthProblem r)
        {
            leftProblem = ell;
            rightProblem = r;
        }

        public BinarySynthOperation(Figure ell, Figure r)
        {
            leftProblem = new UnarySynth(ell);
            rightProblem = new UnarySynth(r);
        }

        public override List<Constraint> GetConstraints()
        {
            List<Constraint> constraints = new List<Constraint>();

            constraints.AddRange(leftProblem.GetConstraints());
            constraints.AddRange(rightProblem.GetConstraints());

            return constraints;
        }

        public override List<Segment> GetAreaVariables()
        {
            List<Segment> areaVars = new List<Segment>();

            areaVars.AddRange(leftProblem.GetAreaVariables());
            areaVars.AddRange(rightProblem.GetAreaVariables());

            return areaVars;
        }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            BinarySynthOperation binarySynth = that as BinarySynthOperation;
            if (binarySynth == null) return false;

            // The outer shapes must be congruent.
            if (!this.leftProblem.IsSymmetricTo(binarySynth.leftProblem)) return false;

            // The outer shapes must be congruent.
            if (!this.rightProblem.IsSymmetricTo(binarySynth.rightProblem)) return false;

            // The atomic regions have to match 1-1 and onto.
            return base.IsSymmetricTo(that);
        }
    }

    public class SubtractionSynth : BinarySynthOperation
    {
        public SubtractionSynth(FigSynthProblem ell, FigSynthProblem r) : base(ell, r) { }
        public SubtractionSynth(Figure ell, Figure r) : base(ell, r) { }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            SubtractionSynth subSynth = that as SubtractionSynth;
            if (subSynth == null) return false;

            // The atomic regions have to match 1-1 and onto.
            return base.IsSymmetricTo(that);
        }

        public override double EvaluateArea(KnownMeasurementsAggregator known)
        {
            double leftArea = leftProblem.EvaluateArea(known);
            double rightArea = rightProblem.EvaluateArea(known);

            if (leftArea < 0 || rightArea < 0) return -1;

            return leftArea - rightArea;
        }

        public override string ToString()
        {
            return "( " + leftProblem.ToString() + " - " + rightProblem.ToString() + " )";
        }
    }

    public class AdditionSynth : BinarySynthOperation
    {
        public AdditionSynth(FigSynthProblem ell, FigSynthProblem r) : base(ell, r) { }
        public AdditionSynth(Figure ell, Figure r) : base(ell, r) { }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            AdditionSynth addSynth = that as AdditionSynth;
            if (addSynth == null) return false;

            // The atomic regions have to match 1-1 and onto.
            return base.IsSymmetricTo(that);
        }

        public override double EvaluateArea(KnownMeasurementsAggregator known)
        {
            double leftArea = leftProblem.EvaluateArea(known);
            double rightArea = rightProblem.EvaluateArea(known);

            if (leftArea < 0 || rightArea < 0) return -1;

            return leftArea + rightArea;
        }

        public override string ToString()
        {
            return "( " + leftProblem.ToString() + " + " + rightProblem.ToString() + " )";
        }
    }

    //
    // Unary figure
    //
    public class UnarySynth : FigSynthProblem
    {
        public Figure figure { get; protected set; }

        public UnarySynth(Figure fig) { figure = fig; }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            UnarySynth unarySynth = that as UnarySynth;
            if (unarySynth == null) return false;

            // The outer shapes must be congruent.
            return this.figure.CoordinateCongruent(unarySynth.figure);

            // The atomic regions have to match 1-1 and onto.
            // return base.IsSymmetricTo(that);
        }

        public override List<Constraint> GetConstraints()
        {
            return figure.GetConstraints();
        }

        public override List<Segment> GetAreaVariables()
        {
            return figure.GetAreaVariables();
        }

        public override double EvaluateArea(KnownMeasurementsAggregator known)
        {
            return figure.GetArea(known);
        }

        public override string ToString()
        {
            return figure.ToString();
        }
    }
}