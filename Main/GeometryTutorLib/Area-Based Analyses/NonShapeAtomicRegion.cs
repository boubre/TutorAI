﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class NonShapeAtomicRegion : AtomicRegion
    {
        //
        // Aggregation class for each segment of an atomic region.
        //
        public enum ConnectionType { SEGMENT, ARC }
        public class Connection
        {
            public Point endpoint1;
            public Point endpoint2;
            public ConnectionType type;

            // The shape which has this connection. 
            public Figure segmentOwner;

            public Connection(Point e1, Point e2, ConnectionType t, Figure owner)
            {
                endpoint1 = e1;
                endpoint2 = e2;
                type = t;
                segmentOwner = owner;
            }
            public bool HasPoint(Point p) { return endpoint1.Equals(p) || endpoint2.Equals(p); }
            public override string ToString()
            {
                return "< " + endpoint1.name + ", " + endpoint2.name + "(" + type + ") >";
            }

            public bool StructurallyEquals(Connection that)
            {
                if (!this.HasPoint(that.endpoint1) || !this.HasPoint(that.endpoint2)) return false;

                if (type != that.type) return false;

                return segmentOwner.StructurallyEquals(that.segmentOwner);
            }
        }

        public List<Connection> connections { get; protected set; }

        public NonShapeAtomicRegion()
        {
            connections = new List<Connection>();
        }

        public void AddConnection(Point e1, Point e2, ConnectionType t, Figure owner)
        {
            connections.Add(new Connection(e1, e2, t, owner));

            Utilities.AddStructurallyUnique<Point>(ownedPoints, e1);
            Utilities.AddStructurallyUnique<Point>(ownedPoints, e2);
        }

        //  |)
        //  | )
        //  |  )
        //  | )
        //  |)
        public bool IsDefinedByChordCircle()
        {
            if (connections.Count != 2) return false;

            if (!HasSegmentConnection() || !HasArcConnection()) return false;

            return AllConnectionsHaveSameEndpoints();
        }

        //  ---\
        //   )  \
        //    )  \
        //     )  |
        //    )  /
        //   )  /
        //  ---/
        public bool IsDefinedByCircleCircle()
        {
            if (connections.Count != 2) return false;

            if (!HasSegmentConnection() && HasArcConnection()) return true;

            return AllConnectionsHaveSameEndpoints();
        }

        public bool DefinesAPolygon()
        {
            return !HasArcConnection();
        }

        private bool HasSegmentConnection()
        {
            foreach (Connection conn in connections)
            {
                if (conn.type == ConnectionType.SEGMENT) return true;
            }

            return false;
        }

        private bool HasArcConnection()
        {
            foreach (Connection conn in connections)
            {
                if (conn.type == ConnectionType.ARC) return true;
            }

            return false;
        }

        private bool AllConnectionsHaveSameEndpoints()
        {
            Point e1 = connections[0].endpoint1;
            Point e2 = connections[1].endpoint2;

            for (int c = 1; c < connections.Count; c++)
            {
                if (!connections[c].HasPoint(e1) || !connections[c].HasPoint(e2)) return false;
            }

            return true;
        }

        public bool HasConnection(Connection that)
        {
            foreach (Connection conn in this.connections)
            {
                if (conn.StructurallyEquals(that)) return true;
            }

            return false;
        }

        ////
        //// Can we put together this atom with the given atomic region?
        //// Does a sequence of sides match?
        ////
        //public AtomicRegion Stitch(AtomicRegion thatAtom)
        //{
        //    return (thatAtom is ShapeAtomicRegion) ? StitchShape(thatAtom as ShapeAtomicRegion) : StitchNonShape(thatAtom as NonShapeAtomicRegion);
        //}

        //private AtomicRegion StitchShape(ShapeAtomicRegion shapeAtom)
        //{
        //    List<Connection> shared = new List<Connection>();
        //    foreach (Connection conn in connections)
        //    {
        //        StitchConnection(conn, shapeAtom);
        //    }
        //}

        //private AtomicRegion StitchConnection(Connection conn, ShapeAtomicRegion shapeAtom)
        //{
        //    if (conn.type == ConnectionType.SEGMENT && shapeAtom.shape is Polygon) return null;
        //    {
        //    }

        //    if (conn.type == ConnectionType.ARC && (shapeAtom.shape is Arc || shapeAtom.shape is Circle))
        //    {
        //        if (shapeAtom.shape.HasSegmentWithEndpoints(conn.endpoint1, conn.endpoint2))
        //        {
        //            shared.Add(conn);
        //        }
        //    }
        //}

        //private AtomicRegion StitchNonShape(NonShapeAtomicRegion nonShapeAtom)
        //{
        //}

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            NonShapeAtomicRegion thatAtom = obj as NonShapeAtomicRegion;
            if (thatAtom == null) return false;

            if (this.connections.Count != thatAtom.connections.Count) return false;

            foreach (Connection conn in this.connections)
            {
                if (!thatAtom.HasConnection(conn)) return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Atom: {");

            for (int c = 0; c < connections.Count; c++)
            {
                str.Append(connections[c].ToString());
                if (c < connections.Count-1) str.Append(", ");
            }

            str.Append(" }");

            return str.ToString();
        }
    }
}
