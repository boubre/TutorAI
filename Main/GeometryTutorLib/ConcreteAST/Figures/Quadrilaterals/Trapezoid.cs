using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public class Trapezoid : Quadrilateral
    {
        public Segment baseSegment { get; private set; }
        public Segment oppBaseSegment { get; private set; }
        public Segment leftLeg { get; private set; }
        public Segment rightLeg { get; private set; }

        public Angle topLeftBaseAngle { get; private set; }
        public Angle topRightBaseAngle { get; private set; }
        public Angle bottomLeftBaseAngle { get; private set; }
        public Angle bottomRightBaseAngle { get; private set; }

        public Segment median { get; private set; }
        private bool medianChecked = true;
        private bool medianValid = true;
        public bool IsMedianIsValid() { return medianValid; }
        public bool IsMedianChecked() { return medianChecked; }
        public void SetMedianInvalid() { medianValid = false; }
        public void SetMedianChecked(bool val) { medianChecked = val; }

        public Trapezoid(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) {
        }

        public Trapezoid(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (left.IsParallelWith(right))
            {
                if (left.Length > right.Length)
                {
                    baseSegment = left;
                    oppBaseSegment = right;
                    leftLeg = top;
                    rightLeg = bottom;
                }
                else
                {
                    baseSegment = right;
                    oppBaseSegment = left;
                    leftLeg = top;
                    rightLeg = bottom;
                }
            }
            else if (top.IsParallelWith(bottom))
            {
                if (top.Length > bottom.Length)
                {
                    baseSegment = top;
                    oppBaseSegment = bottom;
                    leftLeg = left;
                    rightLeg = right;
                }
                else
                {
                    baseSegment = bottom;
                    oppBaseSegment = top;
                    leftLeg = left;
                    rightLeg = right;
                }
            }
            else
            {
                throw new ArgumentException("Quadrilateral does not define a trapezoid; no sides are parallel: " + this);
            }

            topLeftBaseAngle = GetAngle(new Angle(leftLeg, oppBaseSegment));
            topRightBaseAngle = GetAngle(new Angle(rightLeg, oppBaseSegment));
            bottomLeftBaseAngle = GetAngle(new Angle(leftLeg, baseSegment));
            bottomRightBaseAngle = GetAngle(new Angle(rightLeg, baseSegment));

            FindMedian();
        }

        //
        // Find the figure segment which acts as the median of this trapezoid
        //
        public void FindMedian()
        {
            if (Segment.figureSegments.Count == 0)
            {
                //Segments have not yet been recorded for the figure, wait to check for median
                SetMedianChecked(false);
                return;
            }

            foreach(Segment medianCand in Segment.figureSegments)
            {
                // The median is parallel to the bases.
                if (medianCand.IsParallelWith(this.baseSegment) && medianCand.IsParallelWith(this.oppBaseSegment))
                {
                    // The median must be between the bases and connect to the legs.
                    Point leftIntersection = leftLeg.FindIntersection(medianCand);
                    if (leftLeg.PointIsOnAndExactlyBetweenEndpoints(leftIntersection))
                    {
                        Point rightIntersection = rightLeg.FindIntersection(medianCand);
                        if (rightLeg.PointIsOnAndExactlyBetweenEndpoints(rightIntersection))
                        {
                            // Success, we have a median
                            // Acquire the exact figure segment (if it exists) otherwise the segment which contains the median
                            this.median = Segment.GetFigureSegment(leftIntersection, rightIntersection);

                            // If we have a median at all in the figure
                            if (this.median != null)
                            {
                                // If this is not the exact median, create the exact median. 
                                Segment actualMedian = new Segment(leftIntersection, rightIntersection);
                                if (!this.median.StructurallyEquals(actualMedian))
                                {
                                    this.median = actualMedian;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (this.median == null) this.SetMedianInvalid();

            SetMedianChecked(true);
        }

        //
        // Are the legs congruent (for an isosceles trapezoid)?
        //
        public bool CreateCongruentLegs(CongruentSegments css)
        {
            if (leftLeg.StructurallyEquals(css.cs1) && rightLeg.StructurallyEquals(css.cs2)) return true;
            if (leftLeg.StructurallyEquals(css.cs2) && rightLeg.StructurallyEquals(css.cs1)) return true;

            return false;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Trapezoid thatTrap = obj as Trapezoid;
            if (thatTrap == null) return false;

            if (thatTrap is IsoscelesTrapezoid) return false;

            //return base.StructurallyEquals(obj);
            return base.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            Trapezoid thatTrap = obj as Trapezoid;
            if (thatTrap == null) return false;

            if (thatTrap is IsoscelesTrapezoid) return false;
            
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Trapezoid(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                  bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
