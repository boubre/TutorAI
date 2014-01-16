using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Intersection : Descriptor
    {
        public Point intersect { get; protected set; }
        public Segment lhs { get; protected set; }
        public Segment rhs { get; protected set; }

        public Intersection() : base() { }

        public Intersection(Point i, Segment l, Segment r, string just) : base()
        {
            intersect = i;
            lhs = l;
            rhs = r;
            justification = just;
        }

        //
        // If an endpoint of one segment is on the other segment
        //
        public bool StandsOn()
        {
            return Segment.Between(rhs.Point1, lhs.Point1, lhs.Point2) ||
                   Segment.Between(rhs.Point2, lhs.Point1, lhs.Point2) ||
                   Segment.Between(lhs.Point1, rhs.Point1, rhs.Point2) ||
                   Segment.Between(lhs.Point2, rhs.Point1, rhs.Point2);
        }

        //
        // The intersection creates a  T
        //
        //   |
        //   |__________
        //   |
        //   |
        //
        // Returns the non-collinear point, if it exists
        //
        public Point CreatesTShape()
        {
            if (StandsOnEndpoint()) return null;

            // Find the non-collinear end-point
            if (lhs.PointIsOnAndBetweenEndpoints(rhs.Point1)) return rhs.Point2;
            if (lhs.PointIsOnAndBetweenEndpoints(rhs.Point2)) return rhs.Point1;
            if (rhs.PointIsOnAndBetweenEndpoints(lhs.Point1)) return lhs.Point2;
            if (rhs.PointIsOnAndBetweenEndpoints(lhs.Point2)) return lhs.Point1;

            return null;
        }

        //                   top
        //                    o
        //  offStands  oooooooe
        //                    e
        //offEndpoint   eeeeeee
        //                    o
        //                 bottom
        //                       Returns: <offEndpoint, offStands>
        public KeyValuePair<Point, Point> CreatesSimplePIShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            // Restrict to desired combination
            if (this.StandsOnEndpoint() && thatInter.StandsOnEndpoint()) return nullPair;

            //
            // Determine which is the stands and which is the endpoint
            //
            Intersection endpointInter = null;
            Intersection standsInter = null;
            if (this.StandsOnEndpoint() && thatInter.StandsOn())
            {
                endpointInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.StandsOnEndpoint() && this.StandsOn())
            {
                endpointInter = this;
                standsInter = thatInter;
            }
            else return nullPair;

            //
            // Avoid Some shapes
            //
            Segment transversal = this.AcquireTransversal(thatInter);
            Segment transversalStands = standsInter.GetCollinearSegment(transversal);

            Point top = null;
            Point bottom = null;
            if (Segment.Between(standsInter.intersect, transversalStands.Point1, endpointInter.intersect))
            {
                top = transversalStands.Point1;
                bottom = transversalStands.Point2;
            }
            else
            {
                top = transversalStands.Point2;
                bottom = transversalStands.Point1;
            }
            
            // Avoid: ____  Although this shouldn't happen since both intersections do not stand on endpoints
            //        ____|
            //
            // Also avoid Simple F-Shape
            //
            if (transversal.HasPoint(top) || transversal.HasPoint(bottom)) return nullPair;

            // Determine S shape
            Point offStands = standsInter.CreatesTShape();

            Segment parallelEndpoint = endpointInter.OtherSegment(transversal);
            Point offEndpoint = parallelEndpoint.OtherPoint(endpointInter.intersect);


            Segment crossingTester = new Segment(offStands, offEndpoint);
            Point intersection = transversal.FindIntersection(crossingTester);

                                                                       // S-shape    // PI-Shape
            return transversal.PointIsOnAndBetweenEndpoints(intersection) ? nullPair : new KeyValuePair<Point, Point>(offEndpoint, offStands);
        }

        //
        // Creates an F-Shape
        //   top
        //    _____ offEnd     <--- Stands on Endpt
        //   |
        //   |_____ offStands  <--- Stands on 
        //   |
        //   | 
        //  bottom
        //   Order of non-collinear points is order of intersections: <this, that>
        public KeyValuePair<Point, Point> CreatesFShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            // Avoid both standing on an endpoint
            if (this.StandsOnEndpoint() && thatInter.StandsOnEndpoint()) return nullPair;

            Intersection endpt = null;
            Intersection standsOn = null;

            if (this.StandsOnEndpoint() && thatInter.StandsOn())
            {
                endpt = this;
                standsOn = thatInter;
            }
            else if (thatInter.StandsOnEndpoint() && this.StandsOn())
            {
                endpt = thatInter;
                standsOn = this;
            }
            else return nullPair;

            Segment transversal = this.AcquireTransversal(thatInter);
            Segment transversalStands = standsOn.GetCollinearSegment(transversal);

            //
            // Determine Top and bottom to avoid PI shape
            //
            Point top = null;
            Point bottom = null;
            if (Segment.Between(standsOn.intersect, transversalStands.Point1, endpt.intersect))
            {
                bottom = transversalStands.Point1;
                top = transversalStands.Point2;
            }
            else
            {
                bottom = transversalStands.Point2;
                top = transversalStands.Point1;
            }

            // Avoid: ____  Although this shouldn't happen since both intersections do not stand on endpoints
            //        ____|
            if (transversal.HasPoint(top) && transversal.HasPoint(bottom)) return nullPair;

            // Also avoid Simple PI-Shape
            //
            if (!transversal.HasPoint(top) && !transversal.HasPoint(bottom)) return nullPair;

            // Find the two points that make the points of the F 
            Segment parallelEndPt = endpt.OtherSegment(transversal);
            Segment parallelStands = standsOn.OtherSegment(transversal);

            Point offEnd = transversal.PointIsOn(parallelEndPt.Point1) ? parallelEndPt.Point2 : parallelEndPt.Point1;
            Point offStands = transversal.PointIsOn(parallelStands.Point1) ? parallelStands.Point2 : parallelStands.Point1;

            // Check this is not a crazy F
            //        _____
            //       |
            //   ____| 
            //       |
            //       | 
            Point intersection = transversal.FindIntersection(new Segment(offEnd, offStands));

            if (transversal.PointIsOnAndBetweenEndpoints(intersection)) return nullPair;

            // Return in the order of 'off' points: <this, that>
            return this.Equals(endpt) ? new KeyValuePair<Point, Point>(offEnd, offStands) : new KeyValuePair<Point, Point>(offStands, offEnd);
        }

        //
        // Creates a PI
        //
        //   |______
        //   |
        //   |______
        //   |
        //
        //   Order of non-collinear points is order of intersections: <this, that>
        public KeyValuePair<Point, Point> CreatesPIShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            Point thisNonCollinear = this.CreatesTShape();
            Point thatNonCollinear = thatInter.CreatesTShape();

            if (thisNonCollinear == null || thatNonCollinear == null) return nullPair;

            //
            // Verify that the shape is PI and not an S-shape; look for the intersection point NOT between the endpoints of the transversal 
            //
            // The transversal should be valid
            Segment transversal = this.AcquireTransversal(thatInter);
            Point intersection = transversal.FindIntersection(new Segment(thisNonCollinear, thatNonCollinear));

            // S-shape
            if (transversal.PointIsOnAndBetweenEndpoints(intersection)) return nullPair;

            // PI-Shape
            return new KeyValuePair<Point, Point>(thisNonCollinear, thatNonCollinear);            
        }

        //
        //                    o
        //                    eoooooooo  offStands
        //                    e
        //offEndpoint   eeeeeee
        //                    o
        //                       Returns: <offEndpoint, offStands>
        public KeyValuePair<Point, Point> CreatesSimpleSShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            // Restrict to desired combination
            if (this.StandsOnEndpoint() && thatInter.StandsOnEndpoint()) return nullPair;

            //
            // Determine which is the stands and which is the endpoint
            //
            Intersection endpointInter = null;
            Intersection standsInter = null;
            if (this.StandsOnEndpoint() && thatInter.StandsOn())
            {
                endpointInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.StandsOnEndpoint() && this.StandsOn())
            {
                endpointInter = this;
                standsInter = thatInter;
            }
            else return nullPair;

            // Determine S shape
            Point offStands = standsInter.CreatesTShape();
            Segment transversal = this.AcquireTransversal(thatInter);
            Segment parallelEndpoint = endpointInter.OtherSegment(transversal);
            Point offEndpoint = parallelEndpoint.OtherPoint(endpointInter.intersect);

            Segment crossingTester = new Segment(offStands, offEndpoint);
            Point intersection = transversal.FindIntersection(crossingTester);
            
            return transversal.PointIsOnAndBetweenEndpoints(intersection) ? new KeyValuePair<Point, Point>(offEndpoint, offStands) : nullPair;
        }

        //
        // Creates a Leaner-Shape (a bench you can sit on)
        //
        //                 top
        //                  |______ tipStands
        //                  |
        //   tipEndpt ______|
        //
        //   Returns <tipEndpoint, tipStands>
        public KeyValuePair<Point, Point> CreatesLeanerShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            if (this.StandsOnEndpoint() && thatInter.StandsOnEndpoint()) return nullPair;

            //
            // Determine which is the stands and which is the endpoint
            //
            Intersection endpointInter = null;
            Intersection standsInter = null;
            if (this.StandsOnEndpoint() && thatInter.StandsOn())
            {
                endpointInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.StandsOnEndpoint() && this.StandsOn())
            {
                endpointInter = thatInter;
                standsInter = this;
            }
            else return nullPair;

            //
            // Acquire Points
            //
            Point tipStands = standsInter.CreatesTShape();

            Segment transversal = this.AcquireTransversal(thatInter);
            Segment parallelEndpoint = endpointInter.OtherSegment(transversal);

            Point tipEndpoint = parallelEndpoint.OtherPoint(endpointInter.intersect);

            // Determine sides
            Segment crossingTester = new Segment(tipEndpoint, tipStands);
            Point intersection = transversal.FindIntersection(crossingTester);

            // F-Shape
            if (!transversal.PointIsOnAndBetweenEndpoints(intersection)) return nullPair;

            // Desired Leaner shape
            return new KeyValuePair<Point, Point>(tipEndpoint, tipStands);
        }

        //
        // Creates an S-Shape
        //
        //         |______
        //         |
        //   ______|
        //         |
        //
        //   Order of non-collinear points is order of intersections: <this, that>
        public KeyValuePair<Point, Point> CreatesStandardSShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            Point thisNonCollinear = this.CreatesTShape();
            Point thatNonCollinear = thatInter.CreatesTShape();

            if (thisNonCollinear == null || thatNonCollinear == null) return nullPair;

            //
            // Verify that the shape is PI and not an S-shape; look for the intersection point NOT between the endpoints of the transversal 
            //
            // The transversal should be valid
            Segment transversal = this.AcquireTransversal(thatInter);
            Point intersection = transversal.FindIntersection(new Segment(thisNonCollinear, thatNonCollinear));

            // PI-shape
            if (!transversal.PointIsOnAndBetweenEndpoints(intersection)) return nullPair;

            // S-Shape
            return new KeyValuePair<Point, Point>(thisNonCollinear, thatNonCollinear);
        }

        //
        // Creates a basic S-Shape with standsOnEndpoints
        //
        //                  ______ offThat
        //                 |
        //   offThis ______|
        //
        // Return <offThis, offThat>
        public KeyValuePair<Point, Point> CreatesBasicSShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            if (!this.StandsOnEndpoint()) return nullPair;
            if (!thatInter.StandsOnEndpoint()) return nullPair;

            //
            // Determine offThis and offThat
            //
            Segment transversal = this.AcquireTransversal(thatInter);

            Segment parallelThis = this.OtherSegment(transversal);
            Segment parallelThat = thatInter.OtherSegment(transversal);

            Point offThis = transversal.PointIsOnAndBetweenEndpoints(parallelThis.Point1) ? parallelThis.Point2 : parallelThis.Point1;
            Point offThat = transversal.PointIsOnAndBetweenEndpoints(parallelThat.Point1) ? parallelThat.Point2 : parallelThat.Point1;

            // Avoid C-like scenario
            Segment crossingTester = new Segment(offThis, offThat);
            Point intersection = transversal.FindIntersection(crossingTester);

            // C-shape
            if (!transversal.PointIsOnAndBetweenEndpoints(intersection)) return nullPair;

            // S-Shape
            return new KeyValuePair<Point, Point>(offThis, offThat);
        }

        //
        // Creates a basic S-Shape with standsOnEndpoints
        //
        //   offThis   ______
        //                   |
        //   offThat   ______|
        //
        // Return <offThis, offThat>
        public KeyValuePair<Point, Point> CreatesBasicCShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            if (!this.StandsOnEndpoint()) return nullPair;
            if (!thatInter.StandsOnEndpoint()) return nullPair;

            //
            // Determine offThis and offThat
            //
            Segment transversal = this.AcquireTransversal(thatInter);

            Segment parallelThis = this.OtherSegment(transversal);
            Segment parallelThat = thatInter.OtherSegment(transversal);

            Point offThis = transversal.PointIsOnAndBetweenEndpoints(parallelThis.Point1) ? parallelThis.Point2 : parallelThis.Point1;
            Point offThat = transversal.PointIsOnAndBetweenEndpoints(parallelThat.Point1) ? parallelThat.Point2 : parallelThat.Point1;

            // Avoid S-shape scenario
            Segment crossingTester = new Segment(offThis, offThat);
            Point intersection = transversal.FindIntersection(crossingTester);

            // S-shape
            if (transversal.PointIsOnAndBetweenEndpoints(intersection)) return nullPair;

            // C-Shape
            return new KeyValuePair<Point, Point>(offThis, offThat);
        }


        //
        // Creates a Chair
        //
        // |     |                  |
        // |_____|____   leftInter  |_________ tipOfT
        // |                        |     |
        // |                        |     |
        //                         off   tipOfT
        //
        //                                bottomInter
        //
        //                                               <leftInter, bottomInter>
        // Returns the legs of the chair in specific ordering: <off, bottomTip>
        public KeyValuePair<Point, Point> CreatesChairShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            // Both intersections must be standing on (and not endpoints)
            if (!this.StandsOn() || !thatInter.StandsOn()) return nullPair;
            if (this.StandsOnEndpoint() || thatInter.StandsOnEndpoint()) return nullPair;

            Point thisTipOfT = this.CreatesTShape();
            Point thatTipOfT = thatInter.CreatesTShape();

            Segment transversal = this.AcquireTransversal(thatInter);

            Intersection leftInter = null;
            Intersection bottomInter = null;

            // Avoid:
            // |
            // |______
            // |     |
            // |     |
            // this is leftInter
            Point bottomTip = null;
            if (transversal.PointIsOn(thisTipOfT))
            {
                if (transversal.PointIsOnAndBetweenEndpoints(thisTipOfT)) return nullPair;

                leftInter = this;
                bottomInter = thatInter;
                bottomTip = thisTipOfT;
            }
            // thatInter is leftInter
            else if (transversal.PointIsOn(thatTipOfT))
            {
                if (transversal.PointIsOnAndBetweenEndpoints(thatTipOfT)) return nullPair;

                leftInter = thatInter;
                bottomInter = this;
                bottomTip = thisTipOfT;
            }
            // Otherwise, this indicates a PI-shaped scenario 
            else return nullPair;

            //
            // Returns the bottom of the legs of the chair
            //
            Segment parallelLeft = leftInter.OtherSegment(transversal);
            Segment crossingTester = new Segment(parallelLeft.Point1, bottomTip);
            Point intersection = transversal.FindIntersection(crossingTester);

            Point off = transversal.PointIsOnAndBetweenEndpoints(intersection) ? parallelLeft.Point2 : parallelLeft.Point1;

            return new KeyValuePair<Point, Point>(off, bottomTip);
        }

        //
        // Creates an H-Shape
        //
        // |     |
        // |_____|
        // |     |
        // |     |
        //
        public bool CreatesHShape(Intersection thatInter)
        {
            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return false;

            // Both intersections must be standing on (and not endpoints)
            if (!this.StandsOn() || !thatInter.StandsOn()) return false;
            if (this.StandsOnEndpoint() || this.StandsOnEndpoint()) return false;

            Point thisTipOfT = this.CreatesTShape();
            Point thatTipOfT = thatInter.CreatesTShape();

            Segment transversal = this.AcquireTransversal(thatInter);

            // The tips of the intersections must be within the transversal (at the endpoint) for an H
            if (!transversal.PointIsOnAndBetweenEndpoints(thisTipOfT)) return false;
            if (!transversal.PointIsOnAndBetweenEndpoints(thatTipOfT)) return false;

            return true;
        }

        //
        // Creates a shape like a crazy person flying
        //
        // |     |
        // |_____|___ off
        // |     |
        // |     |
        //
        // Similar to H-shape with an extended point
        // Returns the 'larger' intersection that contains the point: off
        public KeyValuePair<Intersection, Point> CreatesFlyingShape(Intersection thatInter)
        {
            KeyValuePair<Intersection, Point> nullPair = new KeyValuePair<Intersection,Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            // Both intersections must be standing on (and not endpoints)
            if (!this.StandsOn() || !thatInter.StandsOn()) return nullPair;
            if (this.StandsOnEndpoint() || thatInter.StandsOnEndpoint()) return nullPair;

            Point thisTipOfT = this.CreatesTShape();
            Point thatTipOfT = thatInter.CreatesTShape();

            Segment transversal = this.AcquireTransversal(thatInter);

            // We have an H-Shape if the tips of the intersections are at the endpoints of the transversal
            if (transversal.PointIsOnAndBetweenEndpoints(thisTipOfT) && transversal.PointIsOnAndBetweenEndpoints(thatTipOfT)) return nullPair;

            Intersection retInter = null;
            Point off = null;
            if (transversal.PointIsOnAndBetweenEndpoints(thisTipOfT))
            {
                retInter = thatInter;
                off = thatTipOfT;
            }
            else
            {
                retInter = this;
                off = thisTipOfT;
            }

            return new KeyValuePair<Intersection, Point>(retInter, off);
        }

        // Corresponding angles if:
        //                      offRightEnd
        // standsOn (o)   o       e
        //                o       e    standsOnEndpoint (e)
        // offLeftEnd  eeeoeeeeeeee
        //                o
        //                o           
        //
        // Returns <offLeftEnd, offRightEnd>
        //
        public KeyValuePair<Point, Point> CreatesSimpleTShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            if (this.StandsOnEndpoint() && thatInter.StandsOnEndpoint()) return nullPair;

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection endpointInter = null;
            Intersection standsInter = null;
            if (this.StandsOnEndpoint() && thatInter.StandsOn())
            {
                endpointInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.Crossing() && this.StandsOnEndpoint())
            {
                endpointInter = thatInter;
                standsInter = this;
            }
            else return nullPair;

            //
            // Determine if the endpoint intersection extends beyond the stands parallel line
            //
            Segment transversal = this.AcquireTransversal(thatInter);
            Segment transversalEndpoint = endpointInter.GetCollinearSegment(transversal);

            if (transversal.PointIsOnAndBetweenEndpoints(transversalEndpoint.Point1) &&
                transversal.PointIsOnAndBetweenEndpoints(transversalEndpoint.Point2)) return nullPair;

            //
            // Acquire the returning points
            //
            Segment parallelEndpoint = endpointInter.OtherSegment(transversal);
            Point offLeftEnd = transversalEndpoint.OtherPoint(endpointInter.intersect);
            Point offRightEnd = parallelEndpoint.OtherPoint(endpointInter.intersect);

            return new KeyValuePair<Point, Point>(offLeftEnd, offRightEnd);
        }


        //
        // Creates a shape like an extended t
        //     offCross                          offCross  
        //      |                                   |
        // _____|____                         ______|______
        //      |                                   |
        //      |_____ offStands     offStands _____|
        //
        // Returns <offStands, offCross>
        public KeyValuePair<Point, Point> CreatesCrossedTShape(Intersection thatInter)
        {
            KeyValuePair<Point, Point> nullPair = new KeyValuePair<Point, Point>(null, null);

            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return nullPair;

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (this.Crossing() && thatInter.StandsOnEndpoint())
            {
                crossingInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.Crossing() && this.StandsOnEndpoint())
            {
                crossingInter = thatInter;
                standsInter = this;
            }
            else return nullPair;

            //
            // Acquire the returning points
            //
            Segment transversal = this.AcquireTransversal(thatInter);

            Segment parallelStands = standsInter.OtherSegment(transversal);
            Point offStands = transversal.PointIsOn(parallelStands.Point1) ? parallelStands.Point2 : parallelStands.Point1;

            Segment transversalCross = crossingInter.GetCollinearSegment(transversal);
            Point offCross = Segment.Between(crossingInter.intersect, transversalCross.Point1, standsInter.intersect) ? transversalCross.Point1 : transversalCross.Point2; 

            return new KeyValuePair<Point, Point>(offStands, offCross);
        }

        //
        // Creates a flying shape using a CROSSING intersection
        //     offCross
        //        |
        //  ______|______ <--crossingInter
        //        |
        //   _____|_____  <--standsInter
        //
        // Returns <offCross>
        //
        public Point CreatesFlyingShapeWithCrossing(Intersection thatInter)
        {
            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return null;

            // We hould not have have endpoint standing as that is handled elsewhere
            if (this.StandsOnEndpoint() || thatInter.StandsOnEndpoint()) return null;

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (this.Crossing() && thatInter.StandsOn())
            {
                crossingInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.Crossing() && this.StandsOn())
            {
                crossingInter = thatInter;
                standsInter = this;
            }
            else return null;

            Segment transversal = this.AcquireTransversal(thatInter);

            // Stands on intersection must have BOTH points not on the transversal line
            //        |
            //  ______|______
            //        |
            //        |_____
            //        |
            //        |
            if (!transversal.PointIsOn(standsInter.CreatesTShape())) return null;
            
            // Success, we have the desired shape
            // Acquire return point: offCross
            Segment transversalCrossing = crossingInter.GetCollinearSegment(transversal);

            return Segment.Between(crossingInter.intersect, transversalCrossing.Point1, standsInter.intersect) ?
                   transversalCrossing.Point1 : transversalCrossing.Point2;
        }

        //
        // Creates a flying shape using a CROSSING intersection
        //     offCross
        //        |
        //  ______|______
        //        |
        //        |_____
        //        |
        //        |
        //
        // Returns <offCross>
        //
        public Point CreatesExtendedChairShape(Intersection thatInter)
        {
            // A valid transversal is required for this shape
            if (!this.CreatesAValidTransversalWith(thatInter)) return null;

            // We hould not have have endpoint standing as that is handled elsewhere
            if (this.StandsOnEndpoint() || thatInter.StandsOnEndpoint()) return null;

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (this.Crossing() && thatInter.StandsOn())
            {
                crossingInter = this;
                standsInter = thatInter;
            }
            else if (thatInter.Crossing() && this.StandsOn())
            {
                crossingInter = thatInter;
                standsInter = this;
            }
            else return null;

            Segment transversal = this.AcquireTransversal(thatInter);

            // Avoid this shape:
            //        |
            //  ______|______
            //        |
            //   _____|_____
            if (transversal.PointIsOn(standsInter.CreatesTShape())) return null;

            // Success, we have the desired shape
            // Acquire return point
            Segment transversalCrossing = crossingInter.GetCollinearSegment(transversal);

            return Segment.Between(crossingInter.intersect, transversalCrossing.Point1, standsInter.intersect) ?
                   transversalCrossing.Point1 : transversalCrossing.Point2;
        }

        //
        // If an endpoint of one segment is on the other segment
        //
        public bool StandsOnEndpoint()
        {
            if (lhs.Point1.Equals(rhs.Point1) || lhs.Point1.Equals(rhs.Point2)) return true;
            if (lhs.Point2.Equals(rhs.Point1) || lhs.Point2.Equals(rhs.Point2)) return true;

            return false;
        }

        public bool Crossing()
        {
            return !StandsOn() && !StandsOnEndpoint();
        }

        public bool IsStraightAngleIntersection()
        {
            return !StandsOnEndpoint();
        }


        //
        // If an endpoint of one segment is on the other segment
        //
        public bool HasSegment(Segment s)
        {
            return lhs.StructurallyEquals(s) || rhs.StructurallyEquals(s);
        }

        public Segment GetCollinearSegment(Segment thatSegment)
        {
            if (lhs.IsCollinearWith(thatSegment)) return lhs;
            if (rhs.IsCollinearWith(thatSegment)) return rhs;

            return null;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public Segment CommonSegment(Intersection thatInter)
        {
            if (lhs.StructurallyEquals(thatInter.lhs) || lhs.IsCollinearWith(thatInter.lhs)) return lhs;
            if (lhs.StructurallyEquals(thatInter.rhs) || lhs.IsCollinearWith(thatInter.rhs)) return lhs;
            if (rhs.StructurallyEquals(thatInter.lhs) || rhs.IsCollinearWith(thatInter.lhs)) return rhs;
            if (rhs.StructurallyEquals(thatInter.rhs) || rhs.IsCollinearWith(thatInter.rhs)) return rhs;

            return null;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public Segment CommonSegment(Parallel thatParallel)
        {
            if (lhs.StructurallyEquals(thatParallel.segment1) || lhs.IsCollinearWith(thatParallel.segment1)) return lhs;
            if (lhs.StructurallyEquals(thatParallel.segment2) || lhs.IsCollinearWith(thatParallel.segment2)) return lhs;
            if (rhs.StructurallyEquals(thatParallel.segment1) || rhs.IsCollinearWith(thatParallel.segment1)) return rhs;
            if (rhs.StructurallyEquals(thatParallel.segment2) || rhs.IsCollinearWith(thatParallel.segment2)) return rhs;

            return null;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public bool InducesNonStraightAngle(Angle thatAngle)
        {
            // The given vertex must match the intersection point of the two lines intersecting
            if (!intersect.Equals(thatAngle.GetVertex())) return false;

            //   /
            //  /
            // /_______
            //
            if (this.StandsOnEndpoint())
            {
                return thatAngle.Equates(new Angle(lhs.OtherPoint(intersect), intersect, rhs.OtherPoint(intersect)));
            }
            //          /
            //         /
            // _______/_______
            //
            else if (this.StandsOn())
            {
                Point off = this.CreatesTShape();
                Segment baseSegment = lhs.PointIsOnAndExactlyBetweenEndpoints(off) ? lhs : rhs;

                if (thatAngle.Equates(new Angle(baseSegment.Point1, intersect, off))) return true;
                if (thatAngle.Equates(new Angle(baseSegment.Point2, intersect, off))) return true;
            }
            //         /
            // _______/_______
            //       /
            //      /
            else if (this.Crossing())
            {
                if (thatAngle.Equates(new Angle(lhs.Point1, intersect, rhs.Point1))) return true;
                if (thatAngle.Equates(new Angle(lhs.Point1, intersect, rhs.Point2))) return true;
                if (thatAngle.Equates(new Angle(lhs.Point2, intersect, rhs.Point1))) return true;
                if (thatAngle.Equates(new Angle(lhs.Point2, intersect, rhs.Point2))) return true;
            }

            return false;
        }

        //
        // If a common segment exists (a transversal that cuts across both intersections), return that common segment
        //
        public Angle GetInducedNonStraightAngle(CongruentAngles congAngles)
        {
            if (this.InducesNonStraightAngle(congAngles.ca1)) return congAngles.ca1;
            if (this.InducesNonStraightAngle(congAngles.ca2)) return congAngles.ca2;

            return null;
        }
        public Angle GetInducedNonStraightAngle(Supplementary supp)
        {
            if (this.InducesNonStraightAngle(supp.angle1)) return supp.angle1;
            if (this.InducesNonStraightAngle(supp.angle2)) return supp.angle2;

            return null;
        }

        //
        // Are both angles induced by this intersection either as vertical angles or adjacent angles
        //
        public bool InducesBothAngles(CongruentAngles conAngles)
        {
            return this.InducesNonStraightAngle(conAngles.ca1) && this.InducesNonStraightAngle(conAngles.ca2);
        }

        public Segment OtherSegment(Segment thatSegment)
        {
            if (lhs.Equals(thatSegment)) return rhs;
            if (rhs.Equals(thatSegment)) return lhs;

            if (lhs.IsCollinearWith(thatSegment)) return rhs;
            if (rhs.IsCollinearWith(thatSegment)) return lhs;

            return null;
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            Perpendicular perp = gc as Perpendicular;
            if (perp == null) return false;
            return intersect.Equals(perp.intersect) && ((lhs.StructurallyEquals(perp.lhs) && rhs.StructurallyEquals(perp.rhs)) ||
                                                        (lhs.StructurallyEquals(perp.rhs) && rhs.StructurallyEquals(perp.lhs)));
        }

        public override bool Covers(GroundedClause gc)
        {
            if (gc is Point) return intersect.Equals(gc as Point) || lhs.Covers(gc) || rhs.Covers(gc);
            else if (gc is Segment) return lhs.Covers(gc) || rhs.Covers(gc);
            // An intersection covers a triangle if a triangle covers the intersection (the intersection
            // point is a vertex and a segment is a side of the triangle)
            else if (gc is Triangle) return (gc as Triangle).Covers(this);

            InMiddle im = gc as InMiddle;
            if (im == null) return false;
            return intersect.Covers(im.point) && (lhs.Covers(im.segment) || rhs.Covers(im.segment));
        }

        //
        // Is the given segment collinear with this intersection
        //
        public bool ImpliesRay(Segment s)
        {
            if (!intersect.Equals(s.Point1) && !intersect.Equals(s.Point2)) return false;

            return lhs.IsCollinearWith(s) || rhs.IsCollinearWith(s);
        }

        public bool DefinesBothRays(Segment thatSeg1, Segment thatSeg2)
        {
            return ImpliesRay(thatSeg1) && ImpliesRay(thatSeg2);
        }

        public bool CreatesAValidTransversalWith(Intersection thatInter)
        {
            Segment transversal = this.AcquireTransversal(thatInter);
            if (transversal == null) return false;

            // Ensure the non-traversal segments align with the parallel segments
            Segment nonTransversalThis = this.OtherSegment(transversal);
            Segment nonTransversalThat = thatInter.OtherSegment(transversal);

            Segment thisTransversalSegment = this.OtherSegment(nonTransversalThis);
            Segment thatTransversalSegment = thatInter.OtherSegment(nonTransversalThat);

            // Parallel lines should not coincide
            if (nonTransversalThis.IsCollinearWith(nonTransversalThat)) return false;

            // Avoid:
            //      |            |
            //    __|    ________|
            //      |            |
            //      |            |

            // Both intersections (transversal segments) must contain the actual transversal
            return thatTransversalSegment.HasSubSegment(transversal) && thisTransversalSegment.HasSubSegment(transversal);
        }

        //
        // Returns the exact transversal between the intersections
        //
        public Segment AcquireTransversal(Intersection thatInter)
        {
            // The two intersections should not be at the same vertex
            if (intersect.Equals(thatInter.intersect)) return null;

            Segment common = CommonSegment(thatInter);
            if (common == null) return null;

            // A legitimate transversal must belong to both intersections (is a subsegment of one of the lines)
            Segment transversal = new Segment(this.intersect, thatInter.intersect);

            Segment thisTransversal = this.GetCollinearSegment(transversal);
            Segment thatTransversal = thatInter.GetCollinearSegment(transversal);

            if (!thisTransversal.HasSubSegment(transversal)) return null;
            if (!thatTransversal.HasSubSegment(transversal)) return null;

            return transversal;
        }

        public override bool StructurallyEquals(Object obj)
        {
            if (obj is Perpendicular) return (obj as Perpendicular).StructurallyEquals(this);

            Intersection inter = obj as Intersection;
            if (inter == null) return false;
            return intersect.Equals(inter.intersect) && ((lhs.StructurallyEquals(inter.lhs) && rhs.StructurallyEquals(inter.rhs)) ||
                                                         (lhs.StructurallyEquals(inter.rhs) && rhs.StructurallyEquals(inter.lhs)));
        }

        public override bool Equals(Object obj)
        {
            if (obj is Perpendicular) return (obj as Perpendicular).Equals(this);

            Intersection inter = obj as Intersection;
            if (inter == null) return false;
            return StructurallyEquals(inter) && base.Equals(inter);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Intersection(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + "): " + justification;
        }

        //  A      B
        //   \    /
        //    \  /
        //     \/
        //     /\ X
        //    /  \
        //   /    \
        //  C      D
        //
        // Intersection(X, Segment(A, D), Segment(B, C)) -> Supplementary(Angle(A, X, B), Angle(B, X, D))
        //                                                  Supplementary(Angle(B, X, D), Angle(D, X, C))
        //                                                  Supplementary(Angle(D, X, C), Angle(C, X, A))
        //                                                  Supplementary(Angle(C, X, A), Angle(A, X, B))
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> InstantiateSupplementary(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (!(c is Intersection)) return newGrounded;

            Intersection newIntersection = c as Intersection;

            // The situation looks like this:
            //  |
            //  |
            //  |_______
            //
            if (newIntersection.StandsOnEndpoint()) return newGrounded;

            // The situation looks like this:
            //       |
            //       |
            //  _____|_______
            //
            if (newIntersection.StandsOn())
            {
                Point up = null;
                Point left = null;
                Point right = null;
                if (newIntersection.lhs.HasPoint(newIntersection.intersect))
                {
                    up = newIntersection.lhs.OtherPoint(newIntersection.intersect);
                    left = newIntersection.rhs.Point1;
                    right = newIntersection.rhs.Point2;
                }
                else
                {
                    up = newIntersection.rhs.OtherPoint(newIntersection.intersect);
                    left = newIntersection.lhs.Point1;
                    right = newIntersection.lhs.Point2;
                }

                Angle newAngle1 = new Angle(left, newIntersection.intersect, up);
                Angle newAngle2 = new Angle(right, newIntersection.intersect, up);

                Supplementary supp = new Supplementary(newAngle1, newAngle2, "Definition of Supplementary");
                supp.SetNotASourceNode();
                supp.SetNotAGoalNode();

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(Utilities.MakeList<GroundedClause>(newIntersection), supp));
            }

            //
            // The situation is standard and results in 4 supplementary relationships
            //
            else
            {
                Angle newAngle1 = new Angle(newIntersection.lhs.Point1, newIntersection.intersect, newIntersection.rhs.Point1);
                Angle newAngle2 = new Angle(newIntersection.lhs.Point1, newIntersection.intersect, newIntersection.rhs.Point2);
                Angle newAngle3 = new Angle(newIntersection.lhs.Point2, newIntersection.intersect, newIntersection.rhs.Point1);
                Angle newAngle4 = new Angle(newIntersection.lhs.Point2, newIntersection.intersect, newIntersection.rhs.Point2);

                Supplementary supp1 = new Supplementary(newAngle1, newAngle2, "Definition of Supplementary");
                Supplementary supp2 = new Supplementary(newAngle2, newAngle4, "Definition of Supplementary");
                Supplementary supp3 = new Supplementary(newAngle3, newAngle4, "Definition of Supplementary");
                Supplementary supp4 = new Supplementary(newAngle3, newAngle1, "Definition of Supplementary");
                supp1.SetNotASourceNode();
                supp1.SetNotAGoalNode();
                supp2.SetNotASourceNode();
                supp2.SetNotAGoalNode();
                supp3.SetNotASourceNode();
                supp3.SetNotAGoalNode();
                supp4.SetNotASourceNode();
                supp4.SetNotAGoalNode();

                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(newIntersection);

                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp1));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp2));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp3));
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, supp4));
            }

            return newGrounded;
        }
    }
}