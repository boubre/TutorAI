using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentArcsHaveCongruentChords : Theorem
    {
        private readonly static string FORWARD_NAME = "Congruent Chords have Congruent Arcs";
        private static Hypergraph.EdgeAnnotation forwardAnnotation = new Hypergraph.EdgeAnnotation(FORWARD_NAME, JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS);

        private readonly static string CONVERSE_NAME = "Congruent Arcs have Congruent Chords";
        private static Hypergraph.EdgeAnnotation converseAnnotation = new Hypergraph.EdgeAnnotation(CONVERSE_NAME, JustificationSwitch.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS);

        public static void Clear()
        {
            candidateCongruentArcs.Clear();
            candidateCongruentSegments.Clear();
        }

        private static List<CongruentArcs> candidateCongruentArcs = new List<CongruentArcs>();
        private static List<CongruentSegments> candidateCongruentSegments = new List<CongruentSegments>();

        //               A
        //              /)
        //             /  )
        //            /    )
        // center:   O      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               C
        //
        //               D
        //              /)
        //             /  )
        //            /    )
        // center:   Q      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               F
        //
        // Congruent(Arc(A, C), Arc(D, F)) -> Congruent(Segment(AC), Segment(DF))
        //
        // Congruent(Segment(AC), Segment(DF)) -> Congruent(Arc(A, C), Arc(D, F))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentSegments)
            {
                newGrounded.AddRange(InstantiateForwardPartOfTheorem(clause as CongruentSegments));
            }

            if (clause is CongruentArcs)
            {
                newGrounded.AddRange(InstantiateConversePartOfTheorem(clause as CongruentArcs));
            }

            return newGrounded;
        }

        //               A
        //              /)
        //             /  )
        //            /    )
        // center:   O      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               C
        //
        //               D
        //              /)
        //             /  )
        //            /    )
        // center:   Q      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               F
        //
        // Congruent(Segment(AC), Segment(DF)) -> Congruent(Arc(A, C), Arc(D, F))
        //
        private static List<EdgeAggregator> InstantiateForwardPartOfTheorem(CongruentSegments cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Acquire the circles for which the segments are chords.
            //
            List<Circle> circles1 = Circle.GetChordCircles(cas.cs1);
            List<Circle> circles2 = Circle.GetChordCircles(cas.cs2);

            //
            // Make all possible combinations of arcs congruent
            //
            foreach (Circle circle1 in circles1)
            {
                // Make an arc out of the chord and circle
                MinorArc arc1 = new MinorArc(circle1, cas.cs1.Point1, cas.cs1.Point2);

                foreach (Circle circle2 in circles2)
                {
                    // Make an arc out of the chord and circle
                    MinorArc arc2 = new MinorArc(circle1, cas.cs1.Point1, cas.cs1.Point2);

                    // Construct the congruence
                    GeometricCongruentArcs gcas = new GeometricCongruentArcs(arc1, arc2);

                    // For hypergraph
                    List<GroundedClause> antecedent = new List<GroundedClause>();
                    antecedent.Add(cas.cs1);
                    antecedent.Add(cas.cs2);
                    antecedent.Add(cas);

                    newGrounded.Add(new EdgeAggregator(antecedent, gcas, forwardAnnotation));
                }
            }

            return newGrounded;
        }

        //               A
        //              /)
        //             /  )
        //            /    )
        // center:   O      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               C
        //
        //               D
        //              /)
        //             /  )
        //            /    )
        // center:   Q      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               F
        //
        // Congruent(Arc(A, C), Arc(D, F)) -> Congruent(Segment(AC), Segment(DF))
        //
        private static List<EdgeAggregator> InstantiateConversePartOfTheorem(CongruentArcs cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Acquire the chords for this specific pair of arcs (endpoints of arc and segment are the same).
            //
            Segment chord1 = Segment.GetFigureSegment(cas.ca1.endpoint1, cas.ca1.endpoint2);
            Segment chord2 = Segment.GetFigureSegment(cas.ca2.endpoint1, cas.ca2.endpoint2);

            if (chord1 == null || chord2 == null) return newGrounded;

            // Construct the congruence
            GeometricCongruentSegments gcss = new GeometricCongruentSegments(chord1, chord2);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(chord1);
            antecedent.Add(chord2);
            antecedent.Add(cas);

            newGrounded.Add(new EdgeAggregator(antecedent, gcss, forwardAnnotation));

            return newGrounded;
        }
    }
}