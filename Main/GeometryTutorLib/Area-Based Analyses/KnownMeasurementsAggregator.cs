using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A aggregator of all known measurements for a particular problem (angles, segment lengths, etc.)
    /// </summary>
    public class KnownMeasurementsAggregator
    {
        public enum UNITS
        {
            INCH,
            FT,
            MI,
            MM,
            CM,
            M,
            KM
        }

        private List<KeyValuePair<Segment, double>> segments;
        private List<KeyValuePair<Angle, double>> angles;
        private UNITS units;

        public KnownMeasurementsAggregator()
        {
            segments = new List<KeyValuePair<Segment, double>>();
            angles = new List<KeyValuePair<Angle, double>>();
            units = UNITS.CM;
        }

        public double GetSegmentLength(Segment thatSeg)
        {
            foreach (KeyValuePair<Segment, double> segPair in segments)
            {
                if (thatSeg.StructurallyEquals(segPair.Key)) return segPair.Value;
            }

            return -1;
        }

        public bool SegmentLengthKnown(Segment thatSeg)
        {
            return GetSegmentLength(thatSeg) > 0;
        }

        public double GetAngleMeasure(Angle thatAngle)
        {
            foreach (KeyValuePair<Angle, double> anglePair in angles)
            {
                if (thatAngle.Equates(anglePair.Key)) return anglePair.Value;
            }

            return -1;
        }

        public bool AngleMeasureKnown(Angle thatAngle)
        {
            return GetAngleMeasure(thatAngle) > 0;
        }

        public bool AddAngleMeasureDegree(Angle thatAngle, double measure)
        {
            if (AngleMeasureKnown(thatAngle)) return false;

            angles.Add(new KeyValuePair<Angle, double>(thatAngle, measure));

            return true;
        }

        public bool AddAngleMeasureRadian(Angle thatAngle, double measure)
        {
            return AddAngleMeasureDegree(thatAngle, Angle.toDegrees(measure));
        }

        public bool AddSegmentLength(Segment thatSegment, double length)
        {
            if (SegmentLengthKnown(thatSegment)) return false;

            segments.Add(new KeyValuePair<Segment, double>(thatSegment, length));

            return true;
        }

        public void SetMeasurementType(UNITS u)
        {
            units = u;
        }
    }
}
