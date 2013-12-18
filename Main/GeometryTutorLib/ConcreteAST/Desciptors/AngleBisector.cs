﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AngleBisector : Bisector
    {
        public Angle angle { get; private set; }
        public Segment bisector { get; private set; }

        public AngleBisector(Angle a, Segment b, string just) : base(just)
        {
            angle = a;
            bisector = b;
        }

        public KeyValuePair<Angle, Angle> GetBisectedAngles()
        {
            Point vertex = angle.GetVertex();

            Point interiorPt = bisector.GetInteriorAnglePoint(angle);
            Point exterioirPt1 = angle.ray1.OtherPoint(vertex);
            Point exterioirPt2 = angle.ray2.OtherPoint(vertex);

            return new KeyValuePair<Angle, Angle>(new Angle(interiorPt, vertex, exterioirPt1), new Angle(interiorPt, vertex, exterioirPt2));
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            AngleBisector ab = obj as AngleBisector;
            if (ab == null) return false;
            return angle.StructurallyEquals(ab.angle) && bisector.StructurallyEquals(ab.bisector);
        }

        public override bool Equals(Object obj)
        {
            AngleBisector ab = obj as AngleBisector;
            if (ab == null) return false;
            return angle.Equals(ab.angle) && bisector.Equals(ab.bisector) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "AngleBisector(" + angle.ToString() +  ", " +  bisector.ToString() + ")";
        }
    }
}
