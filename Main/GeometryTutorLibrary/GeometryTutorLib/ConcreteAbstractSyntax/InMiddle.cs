﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class InMiddle : Descriptor
    {
        public ConcretePoint point { get; private set; }
        public ConcreteSegment segment { get; private set; }

        /// <summary>
        /// Create a new InMiddle statement
        /// </summary>
        /// <param name="p">A point that lies on the segment</param>
        /// <param name="segment">A segment</param>
        public InMiddle(ConcretePoint p, ConcreteSegment segment, string just)
        {
            this.point = p;
            this.segment = segment;
            justification = just;
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("InMiddle");
            sb.AppendLine();
            point.BuildUnparse(sb, tabDepth + 1);
            segment.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            InMiddle im = obj as InMiddle;
            if (im == null) return false;
            return im.point.Equals(point) && im.segment.Equals(segment);
        }

        public override string ToString()
        {
            return "InMiddle(" + point.ToString() + ", " + segment.ToString() + "): " + justification;
        }
    }
}
