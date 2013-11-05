using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;
using GeometryTutorLib.GenericInstantiator;
using System.Diagnostics;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.Pebbler;

namespace Geometry_Testbed
{
    class Program
    {
        private static void Generate(GroundedClause c)
        {
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newClauses = SAS.Instantiate(c);

            if (newClauses == null || !newClauses.Any())
            {
                // Console.WriteLine("No new clauses created.");
            }
            else
            {
                foreach (KeyValuePair<List<GroundedClause>, GroundedClause> gc in newClauses)
                {
                    //Console.WriteLine(gc.ToString());
                }

                //               Console.ReadLine();

                //
                // Cycle the 'new', deduced clauses
                //
                //foreach (GroundedClause newGC in newClauses)
                //{
                //    Generate(newGC);
                //}
            }
        }

        private static void TestSumAnglesInTriangle()
        {
            ConcretePoint a = new ConcretePoint("A", 0, 3);
            ConcretePoint m = new ConcretePoint("M", 2, 1.5);
            ConcretePoint b = new ConcretePoint("B", 4, 3);

            ConcreteTriangle t = new ConcreteTriangle(a, m, b);

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(m);
            clauses.Add(b);
            clauses.Add(t);

            Instantiator instantiator = new Instantiator();
            instantiator.Instantiate(clauses);

            //            graph.ConstructGraph();
            //graph.DebugDumpClauses();
        }

        private static void TestMidpointTheoremFigure()
        {
            ConcretePoint a = new ConcretePoint("A", 0, 3);
            ConcretePoint m = new ConcretePoint("M", 2, 1.5);
            ConcretePoint b = new ConcretePoint("B", 4, 3);

            ConcreteSegment segment = new ConcreteSegment(a, b);

            ConcreteMidpoint mid = new ConcreteMidpoint(m, segment, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(m);
            clauses.Add(b);
            clauses.Add(segment);
            clauses.Add(mid);

            //Instantiator instantiator = new Instantiator();
            //Hypergraph<GroundedClause, int> graph = instantiator.Instantiate(clauses);

            //graph.DebugDumpClauses();

            //PebblerHypergraph<GroundedClause> pebblerGraph = graph.GetPebblerHypergraph();

            //pebblerGraph.DebugDumpClauses();

            //int[] srcArr = { 2 };
            //List<int> src = new List<int>(srcArr);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        private static void TestFigureSix()
        {
            ConcretePoint x = new ConcretePoint("X", 0, 0);
            ConcretePoint m = new ConcretePoint("M", 2, 0);
            ConcretePoint y = new ConcretePoint("Y", 4, 0);
            ConcretePoint t = new ConcretePoint("T", -1, 2);
            ConcretePoint n = new ConcretePoint("N", 1, 2); 
            ConcretePoint z = new ConcretePoint("Z", -2, 2);

            ConcreteTriangle ztn = new ConcreteTriangle(z, t, n);
            ConcreteTriangle tnm = new ConcreteTriangle(t, n, m);
            ConcreteTriangle nmy = new ConcreteTriangle(n, m, y);
            ConcreteTriangle txm = new ConcreteTriangle(t, x, m);

            ConcreteSegment zx = new ConcreteSegment(z, x);
            ConcreteSegment zy = new ConcreteSegment(z, y);
            ConcreteSegment xy = new ConcreteSegment(x, y);

            ConcreteMidpoint midT = new ConcreteMidpoint(t, zx, "Given");
            ConcreteMidpoint midN = new ConcreteMidpoint(n, zy, "Given");
            ConcreteMidpoint midM = new ConcreteMidpoint(m, xy, "Given");

            ConcreteCongruentSegments ccss1 = new ConcreteCongruentSegments(new ConcreteSegment(t, n), new ConcreteSegment(x, m), "Given");
            ConcreteCongruentSegments ccss2 = new ConcreteCongruentSegments(new ConcreteSegment(n, m), new ConcreteSegment(t, x), "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(x);
            clauses.Add(m);
            clauses.Add(y);
            clauses.Add(t);
            clauses.Add(n);
            clauses.Add(z);
            clauses.Add(ztn);
            clauses.Add(tnm);
            clauses.Add(nmy);
            clauses.Add(txm);
            clauses.Add(zx);
            clauses.Add(zy);
            clauses.Add(xy);
            clauses.Add(midT);
            clauses.Add(midN);
            clauses.Add(midM);
            clauses.Add(ccss1);
            clauses.Add(ccss2);

            Instantiator instantiator = new Instantiator();
            instantiator.Instantiate(clauses);

            //graph.ConstructGraph();
            //graph.ConstructGraphRepresentation();
            //graph.DebugDumpClauses();

            int[] srcArr = { 2, 3, 4, 5, 6, 7, 8 };
            List<int> src = new List<int>(srcArr);
            int[] goalArr = { 36 };
            List<int> goals = new List<int>(goalArr);

            //graph.ConstructPath(src, goals);
        }

        private static void TestEntireFigure()
        {
            ConcretePoint a = new ConcretePoint("A", 0, 3);
            ConcretePoint m = new ConcretePoint("M", 2, 1.5);
            ConcretePoint b = new ConcretePoint("B", 4, 3);
            ConcretePoint c = new ConcretePoint("C", 4, 0);
            ConcretePoint d = new ConcretePoint("D", 0, 0);

            ConcreteSegment diagonal1 = new ConcreteSegment(a, c);
            ConcreteSegment diagonal2 = new ConcreteSegment(b, d);

            ConcreteTriangle rightOne = new ConcreteTriangle(a, c, d);
            ConcreteTriangle rightTwo = new ConcreteTriangle(b, c, d);

            ConcreteTriangle isoOne = new ConcreteTriangle(a, m, d);
            ConcreteTriangle isoTwo = new ConcreteTriangle(b, m, c);

            ConcreteTriangle bottomIso = new ConcreteTriangle(c, m, d);

            Intersection inter = new Intersection(m, diagonal1, diagonal2, "Given");

            ConcreteMidpoint mid1 = new ConcreteMidpoint(m, diagonal1, "Given");
            ConcreteMidpoint mid2 = new ConcreteMidpoint(m, diagonal2, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(a);
            clauses.Add(m);
            clauses.Add(b);
            clauses.Add(c);
            clauses.Add(d);
            clauses.Add(diagonal1);
            clauses.Add(diagonal2);
            clauses.Add(rightOne);
            clauses.Add(rightTwo);
            clauses.Add(isoOne);
            clauses.Add(isoTwo);
            clauses.Add(bottomIso);
            clauses.Add(inter);
            clauses.Add(mid1);
            clauses.Add(mid2);

            //Instantiator instantiator = new Instantiator();
            //Hypergraph<GroundedClause, int> graph = instantiator.Instantiate(clauses);

            ////graph.ConstructGraph();
            ////graph.ConstructGraphRepresentation();
            //graph.DebugDumpClauses();

            //PebblerHypergraph<GroundedClause> pebblerGraph = graph.GetPebblerHypergraph();

            //int[] srcArr = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            //List<int> src = new List<int>(srcArr);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);
        }

        static void Main(string[] args)
        {
            HLTest1();
            //TestMidpointTheoremFigure();
            //SASTest1();
            //TestSumAnglesInTriangle();
            //TestFigureSix();
            //TestEntireFigure();
            //TestSimplification();
            //TestSimplificationConstants();
            //TestSimpleSubstitution();
        }

        private static void HLTest1()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint p13 = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p11, p12);
            ConcreteSegment s12 = new ConcreteSegment(p12, p13);
            ConcreteTriangle t1 = new ConcreteTriangle(p11, p12, p13);
            t1.SetProvenToBeRight();

            ConcretePoint p21 = new ConcretePoint("D", 4, 0);
            ConcretePoint p22 = new ConcretePoint("E", 4, 2);
            ConcretePoint p23 = new ConcretePoint("F", 7, 0);
            ConcreteSegment s21 = new ConcreteSegment(p21, p22);
            ConcreteSegment s22 = new ConcreteSegment(p22, p23);
            ConcreteTriangle t2 = new ConcreteTriangle(p21, p22, p23);
            t2.SetProvenToBeRight();

            //
            // Congruent Segments and Angle
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s11, s21, "Given");

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s22, "Given");

            List<GroundedClause> clauses = new List<GroundedClause>();

            clauses.Add(p11);
            clauses.Add(p12);
            clauses.Add(p13);
            clauses.Add(s11);
            clauses.Add(s12);
            clauses.Add(t1);

            clauses.Add(p21);
            clauses.Add(p22);
            clauses.Add(p23);
            clauses.Add(s21);
            clauses.Add(s22);
            clauses.Add(t2);

            clauses.Add(ccs1);
            clauses.Add(ccs2);

            GeometryTutorLib.BridgeUItoBackEnd.AnalyzeFigure(clauses);

        }

        private static void TestSimpleSubstitution()
        {
            ConcretePoint a = new ConcretePoint("A", 0, 3);
            ConcretePoint b = new ConcretePoint("B", 0, 0);
            ConcretePoint c = new ConcretePoint("C", 3, 0);
            ConcretePoint d = new ConcretePoint("D", 7, 3);
            ConcretePoint e = new ConcretePoint("E", 7, 0);
            ConcretePoint f = new ConcretePoint("F", 10, 0);

            ConcreteAngle ang1 = new ConcreteAngle(a, b, c);
            ConcreteAngle ang2 = new ConcreteAngle(d, e, f);

            AngleMeasureEquation ame1 = new AngleMeasureEquation(ang1, new NumericValue(90));
            AngleMeasureEquation ame2 = new AngleMeasureEquation(ang2, new NumericValue(90));

            Substitution.Instantiate(ame1);
            Substitution.Instantiate(ame2);
        }

        private static void TestSimplificationConstants()
        {
            Addition suml = new Addition(new NumericValue(0), new NumericValue(1));
            Addition sumr = new Addition(new NumericValue(9), new NumericValue(8));
            for (int i = 2; i < 10; i++)
            {
                suml = new Addition(suml, new NumericValue(i));
                sumr = new Addition(sumr, new NumericValue(10 - i));
            }

            SegmentEquation se = new SegmentEquation(sumr, sumr);

            //Simplification.Instantiate(se);
        }

        private static void TestSimplification()
        {
            ConcretePoint a = new ConcretePoint("A", 0, 3);
            ConcretePoint m = new ConcretePoint("M", 2, 1.5);
            ConcretePoint b = new ConcretePoint("B", 4, 3);
            ConcretePoint c = new ConcretePoint("C", 4, 0);
            ConcretePoint d = new ConcretePoint("D", 0, 0);

            ConcreteSegment segmentAM = new ConcreteSegment(a, m);
            ConcreteSegment segmentMB = new ConcreteSegment(m, b);
            ConcreteSegment segmentAB = new ConcreteSegment(a, b);

            NumericValue two = new NumericValue(2);

            Addition add = new Addition(segmentAM, segmentAM);
            Addition add2 = new Addition(segmentAM, segmentAM);
            Addition add3 = new Addition(add, add2);
            Multiplication product = new Multiplication(two, add3);

            SegmentEquation se = new SegmentEquation(add3, product);

           // Simplification.Instantiate(se);
        }

        private static void TestStraightAngleDefinition()
        {
            List<ConcretePoint> pts = new List<ConcretePoint>();
            string[] names = { "A", "B", "C", "D", "E", "F", "G", "H" };

            for (int i = 0; i < 6; i++)
            {
                pts.Add(new ConcretePoint(names[i], i, 2 * i));
            }

            ConcreteCollinear coll = new ConcreteCollinear(pts);

            Generate(coll);
        }

        private static void TestMidpointDefinition()
        {
            ConcretePoint pt = new ConcretePoint("M", 0, 0);
            ConcretePoint end1 = new ConcretePoint("A", -2, -2);
            ConcretePoint end2 = new ConcretePoint("C", 2, 2);
            ConcreteSegment segment = new ConcreteSegment(end1, end2);
            ConcreteMidpoint mid = new ConcreteMidpoint(pt, segment, "Given");

            Generate(mid);
        }

        private static void TestAngleAdditionAxiom()
        {
            ConcretePoint top = new ConcretePoint("A", 0, 2);
            ConcretePoint vertex = new ConcretePoint("V", 0, 0);
            ConcretePoint right = new ConcretePoint("B", 2, 0);
            ConcreteAngle firstQuad = new ConcreteAngle(top, vertex, right);

            ConcretePoint bottom = new ConcretePoint("C", 0, -2);
            ConcretePoint left = new ConcretePoint("D", -2, 0);
            ConcreteAngle thirdQuad = new ConcreteAngle(left, vertex, bottom);

            ConcreteAngle fourthQuad = new ConcreteAngle(right, vertex, bottom);

            Generate(firstQuad);
            Generate(thirdQuad);
            Generate(fourthQuad);
        }

        private static void TestSegmentAdditionAxiom()
        {
            ConcretePoint top = new ConcretePoint("A", 0, 2);
            ConcretePoint origin = new ConcretePoint("V", 0, 0);
            ConcretePoint right = new ConcretePoint("B", 2, 0);
            ConcretePoint bottom = new ConcretePoint("C", 0, -2);
            ConcretePoint left = new ConcretePoint("D", -2, 0);

            ConcreteSegment vertical = new ConcreteSegment(top, bottom);
            ConcreteSegment horizontal = new ConcreteSegment(left, right);
            InMiddle mid1 = new InMiddle(origin, vertical, "Given");
            InMiddle mid2 = new InMiddle(origin, horizontal, "Given");

            Generate(mid1);
            Generate(mid2);
        }

        private static void TestVerticalAnglesTheorem()
        {
            ConcretePoint top = new ConcretePoint("A", 0, 2);
            ConcretePoint origin = new ConcretePoint("V", 0, 0);
            ConcretePoint right = new ConcretePoint("B", 2, 0);
            ConcretePoint bottom = new ConcretePoint("C", 0, -2);
            ConcretePoint left = new ConcretePoint("D", -2, 0);

            ConcreteSegment vertical = new ConcreteSegment(top, bottom);
            ConcreteSegment horizontal = new ConcreteSegment(left, right);
            Intersection inter = new Intersection(origin, vertical, horizontal, "Given");

            Generate(inter);

            top = new ConcretePoint("A", 2, 1);
            origin = new ConcretePoint("V", 0, 0);
            right = new ConcretePoint("B", 1, 2);
            bottom = new ConcretePoint("C", -2, -1);
            left = new ConcretePoint("D", -1, -2);

            ConcreteSegment seg1 = new ConcreteSegment(top, bottom);
            ConcreteSegment seg2 = new ConcreteSegment(left, right);
            inter = new Intersection(origin, seg1, seg2, "Given");

            Generate(inter);
        }

        private static void TestIsoscelesTriangleDefinition()
        {
            ConcretePoint a = new ConcretePoint("A", -2, 0);
            ConcretePoint b = new ConcretePoint("B", 0, 2);
            ConcretePoint c = new ConcretePoint("C", 2, 0);
            ConcreteSegment seg1 = new ConcreteSegment(a, b);
            ConcreteSegment seg2 = new ConcreteSegment(b, c);

            ConcreteTriangle tri = new ConcreteTriangle(a, b, c);

            ConcreteCongruentSegments ccss = new ConcreteCongruentSegments(seg1, seg2, "Given");

            Generate(ccss);
            Generate(tri);
        }

        private static void TestSubstitution()
        {
            ConcretePoint a = new ConcretePoint("A", -2, 0);
            ConcretePoint b = new ConcretePoint("B", 0, 2);
            ConcretePoint c = new ConcretePoint("C", 2, 0);
            ConcreteSegment seg1 = new ConcreteSegment(a, b);
            ConcreteSegment seg2 = new ConcreteSegment(b, c);
            SegmentEquation segEq = new SegmentEquation(seg1, seg2, "Given");

            Generate(segEq);

            ConcretePoint d = new ConcretePoint("D", -2, -1);
            ConcretePoint e = new ConcretePoint("E", 0, -3);
            ConcretePoint f = new ConcretePoint("F", 2, -1);
            ConcreteSegment seg3 = new ConcreteSegment(d, e);
            ConcreteSegment seg4 = new ConcreteSegment(e, f);
            SegmentEquation segEq2 = new SegmentEquation(seg3, seg4, "Given");

            Generate(segEq2);

            SegmentEquation segEq3 = new SegmentEquation(seg2, seg3, "Given");

            Generate(segEq3);
        }

        //
        // Tests if the same triangle is congruent to itself
        //
        private static void SSSTest6()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p1 = new ConcretePoint("A", 0, 0);
            ConcretePoint p2 = new ConcretePoint("B", 0, 2);
            ConcretePoint p3 = new ConcretePoint("C", 3, 0);

            ConcreteSegment left = new ConcreteSegment(p1, p2);
            ConcreteSegment bottom = new ConcreteSegment(p1, p3);
            ConcreteSegment diagonal = new ConcreteSegment(p2, p3);
            ConcreteTriangle t1 = new ConcreteTriangle(left, bottom, diagonal, "Given");

            Generate(t1);

            ConcreteTriangle t2 = new ConcreteTriangle(left, bottom, diagonal, "Given");

            Generate(t2);

            //
            // Congruent Segments
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(left, left, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(bottom, bottom, "Given");
            Generate(ccs2);

            ConcreteCongruentSegments ccs3 = new ConcreteCongruentSegments(diagonal, diagonal, "Given");
            Generate(ccs3);
        }

        //
        // Tests two triangles in which two points are shared
        //
        private static void SSSTest5()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint shared1 = new ConcretePoint("B", 0, 2);
            ConcretePoint shared2 = new ConcretePoint("C", 3, 0);

            ConcreteSegment left = new ConcreteSegment(p11, shared1);
            ConcreteSegment bottom = new ConcreteSegment(p11, shared2);
            ConcreteSegment diagonal = new ConcreteSegment(shared1, shared2);
            ConcreteTriangle t1 = new ConcreteTriangle(left, bottom, diagonal, "Given");

            Generate(t1);

            ConcretePoint pt = new ConcretePoint("D", 3, 2);
            ConcreteSegment top = new ConcreteSegment(pt, shared1);
            ConcreteSegment right = new ConcreteSegment(pt, shared2);
            ConcreteTriangle t2 = new ConcreteTriangle(top, right, diagonal, "Given");

            Generate(t2);

            //
            // Congruent Segments
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(top, bottom, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(left, right, "Given");
            Generate(ccs2);

            ConcreteCongruentSegments ccs3 = new ConcreteCongruentSegments(diagonal, diagonal, "Given");
            Generate(ccs3);
        }

        //
        // Tests two triangles in which one point is shared
        //
        private static void SSSTest4()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint shared = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p11, p12);
            ConcreteSegment s12 = new ConcreteSegment(p11, shared);
            ConcreteSegment s13 = new ConcreteSegment(p12, shared);
            ConcreteTriangle t1 = new ConcreteTriangle(s11, s12, s13, "Given");

            Generate(t1);

            ConcretePoint p21 = new ConcretePoint("D", 6, 0);
            ConcretePoint p22 = new ConcretePoint("E", 6, 2);
            ConcreteSegment s21 = new ConcreteSegment(p21, p22);
            ConcreteSegment s22 = new ConcreteSegment(p21, shared);
            ConcreteSegment s23 = new ConcreteSegment(p22, shared);
            ConcreteTriangle t2 = new ConcreteTriangle(s21, s22, s23, "Given");

            Generate(t2);

            //
            // Congruent Segments
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s11, s21, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s22, "Given");
            Generate(ccs2);

            ConcreteCongruentSegments ccs3 = new ConcreteCongruentSegments(s13, s23, "Given");
            Generate(ccs3);
        }

        private static void SSSTest3()
        {
            //
            // Tri ABC is congruent to Tri DEF ; order of points and segments changed
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint p13 = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p12, p11);
            ConcreteSegment s12 = new ConcreteSegment(p11, p13);
            ConcreteSegment s13 = new ConcreteSegment(p13, p12);
            ConcreteTriangle t1 = new ConcreteTriangle(s13, s11, s12, "Given");

            Generate(t1);

            ConcretePoint p21 = new ConcretePoint("D", 6, 0);
            ConcretePoint p22 = new ConcretePoint("E", 6, 2);
            ConcretePoint p23 = new ConcretePoint("F", 3, 2);
            ConcreteSegment s21 = new ConcreteSegment(p22, p21);
            ConcreteSegment s22 = new ConcreteSegment(p21, p23);
            ConcreteSegment s23 = new ConcreteSegment(p23, p22);
            ConcreteTriangle t2 = new ConcreteTriangle(s22, s21, s23, "Given");

            Generate(t2);

            //
            // Congruent Segments
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s21, s11, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s23, "Given");
            Generate(ccs2);

            ConcreteCongruentSegments ccs3 = new ConcreteCongruentSegments(s22, s13, "Given");
            Generate(ccs3);
        }

        private static void SSSTest2()
        {
            //
            // Tri ABC is congruent to Tri DEF ; segments congruences seen first
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint p13 = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p11, p12);
            ConcreteSegment s12 = new ConcreteSegment(p11, p13);
            ConcreteSegment s13 = new ConcreteSegment(p12, p13);
            ConcreteTriangle t1 = new ConcreteTriangle(s11, s12, s13, "Given");

            ConcretePoint p21 = new ConcretePoint("D", 4, 0);
            ConcretePoint p22 = new ConcretePoint("E", 4, 2);
            ConcretePoint p23 = new ConcretePoint("F", 7, 0);
            ConcreteSegment s21 = new ConcreteSegment(p21, p22);
            ConcreteSegment s22 = new ConcreteSegment(p21, p23);
            ConcreteSegment s23 = new ConcreteSegment(p22, p23);
            ConcreteTriangle t2 = new ConcreteTriangle(s21, s22, s23, "Given");

            //
            // Congruent Segments
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s11, s21, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s22, "Given");
            Generate(ccs2);

            ConcreteCongruentSegments ccs3 = new ConcreteCongruentSegments(s13, s23, "Given");
            Generate(ccs3);

            Generate(t1);
            Generate(t2);
        }


        private static void SSSTest1()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint p13 = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p11, p12);
            ConcreteSegment s12 = new ConcreteSegment(p11, p13);
            ConcreteSegment s13 = new ConcreteSegment(p12, p13);
            ConcreteTriangle t1 = new ConcreteTriangle(s11, s12, s13, "Given");

            Generate(t1);

            ConcretePoint p21 = new ConcretePoint("D", 4, 0);
            ConcretePoint p22 = new ConcretePoint("E", 4, 2);
            ConcretePoint p23 = new ConcretePoint("F", 7, 0);
            ConcreteSegment s21 = new ConcreteSegment(p21, p22);
            ConcreteSegment s22 = new ConcreteSegment(p21, p23);
            ConcreteSegment s23 = new ConcreteSegment(p22, p23);
            ConcreteTriangle t2 = new ConcreteTriangle(s21, s22, s23, "Given");

            Generate(t2);

            //
            // Congruent Segments
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s11, s21, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s22, "Given");
            Generate(ccs2);

            ConcreteCongruentSegments ccs3 = new ConcreteCongruentSegments(s13, s23, "Given");
            Generate(ccs3);
        }

        private static void SASTest1()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint p13 = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p11, p12);
            ConcreteSegment s12 = new ConcreteSegment(p12, p13);
            ConcreteAngle ang1 = new ConcreteAngle(p11, p12, p13);
            ConcreteTriangle t1 = new ConcreteTriangle(p11, p12, p13);

            Generate(t1);

            ConcretePoint p21 = new ConcretePoint("D", 4, 0);
            ConcretePoint p22 = new ConcretePoint("E", 4, 2);
            ConcretePoint p23 = new ConcretePoint("F", 7, 0);
            ConcreteSegment s21 = new ConcreteSegment(p21, p22);
            ConcreteSegment s22 = new ConcreteSegment(p22, p23);
            ConcreteAngle ang2 = new ConcreteAngle(p21, p22, p23);
            ConcreteTriangle t2 = new ConcreteTriangle(p21, p22, p23);

            Generate(t2);

            //
            // Congruent Segments and Angle
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s11, s21, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s22, "Given");
            Generate(ccs2);

            ConcreteCongruentAngles cca = new ConcreteCongruentAngles(ang1, ang2, "Given");
            Generate(cca);
        }

        private static void SASTest2()
        {
            //
            // Tri ABC is congruent to Tri DEF
            //
            ConcretePoint p11 = new ConcretePoint("A", 0, 0);
            ConcretePoint p12 = new ConcretePoint("B", 0, 2);
            ConcretePoint p13 = new ConcretePoint("C", 3, 0);
            ConcreteSegment s11 = new ConcreteSegment(p11, p12);
            ConcreteSegment s12 = new ConcreteSegment(p12, p13);
            ConcreteAngle ang1 = new ConcreteAngle(p11, p12, p13);
            ConcreteTriangle t1 = new ConcreteTriangle(p11, p12, p13);



            ConcretePoint p21 = new ConcretePoint("D", 4, 0);
            ConcretePoint p22 = new ConcretePoint("E", 4, 2);
            ConcretePoint p23 = new ConcretePoint("F", 7, 0);
            ConcreteSegment s21 = new ConcreteSegment(p21, p22);
            ConcreteSegment s22 = new ConcreteSegment(p22, p23);
            ConcreteAngle ang2 = new ConcreteAngle(p21, p22, p23);
            ConcreteTriangle t2 = new ConcreteTriangle(p21, p22, p23);

            //
            // Congruent Segments and Angle
            //
            ConcreteCongruentSegments ccs1 = new ConcreteCongruentSegments(s11, s21, "Given");
            Generate(ccs1);

            ConcreteCongruentSegments ccs2 = new ConcreteCongruentSegments(s12, s22, "Given");
            Generate(ccs2);

            ConcreteCongruentAngles cca = new ConcreteCongruentAngles(ang1, ang2, "Given");
            Generate(cca);

            Generate(t1);
            Generate(t2);
        }
    }
}