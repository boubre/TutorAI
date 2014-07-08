﻿using System;

namespace GeometryTutorLib
{
    //
    // Given a pair of coordinates; generate a unique name for it; return that point object.
    // Names go from A..Z..AA..ZZ..AAA...ZZZ
    //
    public static class PointFactory
    {
        private const string prefix = "_*_";
        private static string currentName = "A";
        private static int numLetters = 1;

        public static GeometryTutorLib.ConcreteAST.Point GeneratePoint(double x, double y)
        {
            return new GeometryTutorLib.ConcreteAST.Point(GetCurrentName(), x, y);
        }

        // Reset for the next problem
        public static void Reset()
        {
            currentName = "A";
            numLetters = 1;
        }

        private static string GetCurrentName()
        {
            string name = prefix + currentName;

            UpdateName();

            return name;
        }

        private static void UpdateName()
        {
            // Restart at the beginning of the alphabet
            if (currentName[0] == 'Z')
            {
                // We rolled over to more letter.
                numLetters++;

                currentName = "";
                for (int i = 0; i < numLetters; i++)
                {
                    currentName += 'A';
                }
            }
            // Simple increment from A to B, etc.
            else
            {
                char alpha = currentName[0];
                alpha++;

                currentName = "";
                for (int i = 0; i < numLetters; i++)
                {
                    currentName += alpha;
                }
            }
        }
    }
}
