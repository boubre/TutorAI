using System;
using System.Collections.Generic;

namespace GeometryTutorLib.GenericInstantiator
{
    //
    // The bridge between the front and back end where the user indicates which axioms, definitions, and theorems to use.
    //
    public static class JustificationSwitch
    {
        public static readonly bool RELATION_TRANSITIVE_SUBSTITUTION = true;
        public static readonly bool TRANSITIVE_SUBSTITUTION = true;
        public static readonly bool ANGLE = true;

        //
        // Axioms
        //
        public static readonly bool AA_SIMILARITY = true;
        public static readonly bool ANGLE_ADDITION_AXIOM = true;
        public static readonly bool ASA = true;
        public static readonly bool CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL = true;
        public static readonly bool CORRESPONDING_ANGLES_OF_PARALLEL_LINES = true;
        public static readonly bool SEGMENT_ADDITION_AXIOM = true;
        public static readonly bool SAS_CONGRUENCE = true;
        public static readonly bool SSS = true;
        public static readonly bool ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT = true;
        public static readonly bool TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE = true;

        //
        // Definitions
        //
        public static readonly bool ALTITUDE_DEFINITION = true;
        public static readonly bool ANGLE_BISECTOR_DEFINITION = true;
        public static readonly bool COMPLEMENTARY_DEFINITION = true;
        public static readonly bool CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION = true;
        public static readonly bool ISOSCELES_TRIANGLE_DEFINITION = true;
        public static readonly bool MEDIAN_DEFINITION = true;
        public static readonly bool MIDPOINT_DEFINITION = true;
        public static readonly bool PERPENDICULAR_DEFINITION = true;
        public static readonly bool PERPENDICULAR_BISECTOR_DEFINITION = true;
        public static readonly bool RIGHT_ANGLE_DEFINITION = true;
        public static readonly bool RIGHT_TRIANGLE_DEFINITION = true;
        public static readonly bool SEGMENT_BISECTOR_DEFINITION = true;
        public static readonly bool SUPPLEMENTARY_DEFINITION = true;
        public static readonly bool STRAIGHT_ANGLE_DEFINITION = true;

        //
        // Theorems
        //
        public static readonly bool AAS = true;
        public static readonly bool ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY = true;
        public static readonly bool ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY = true;
        public static readonly bool ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL = true;
        public static readonly bool ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR = true;
        public static readonly bool ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES = true;
        public static readonly bool ANGLE_BISECTOR_THEOREM = true;
        public static readonly bool CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR = true;
        public static readonly bool CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES = true;
        public static readonly bool CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES = true;
        public static readonly bool EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES = true;
        public static readonly bool EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES = false;
        public static readonly bool HYPOTENUSE_LEG = true;
        public static readonly bool ISOSCELES_TRIANGLE_THEOREM = true;
        public static readonly bool MIDPOINT_THEOREM = true;
        public static readonly bool PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES = true;
        public static readonly bool PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY = true;
        public static readonly bool PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES = true;
        public static readonly bool RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT = true;
        public static readonly bool SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL = true;
        public static readonly bool SAS_SIMILARITY = true;
        public static readonly bool SSS_SIMILARITY = true;
        public static readonly bool SUM_ANGLES_IN_TRIANGLE_180 = false;
        public static readonly bool TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR = true;
        public static readonly bool TRANSITIVE_CONGRUENT_TRIANGLES = true;
        public static readonly bool TRANSITIVE_PARALLEL = true;
        public static readonly bool TRANSITIVE_SIMILAR = true;
        public static readonly bool TRIANGLE_PROPORTIONALITY = true;
        public static readonly bool TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT = true;
        public static readonly bool VERTICAL_ANGLES = true;

        //
        // General Switches
        //
        public static readonly bool SIMILARITY = true;
        public static readonly bool TRIANGLE_CONGREUNCE = true;
        public static readonly bool REFLEXIVE = true;
    }
}
