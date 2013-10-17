﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    /// <summary>
    /// Describes two lines as having equal length
    /// </summary>
    public class EqualSegments : Equal
    {
        public ConcreteSegment segment1 { get; private set; }
        public ConcreteSegment segment2 { get; private set; }

        /// <summary>
        /// Create a new Equal descriptor
        /// </summary>
        /// <param name="s1">A segment</param>
        /// <param name="s2">A segment with the same length as the previous segment</param>
        public EqualSegments(ConcreteSegment s1, ConcreteSegment s2, string just)
        {
            this.segment1 = s1;
            this.segment2 = s2;
            justification = just;
        }

        internal void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            Indent(sb, tabDepth);
            sb.Append("Equal");
            sb.AppendLine();
            segment1.BuildUnparse(sb, tabDepth + 1);
            segment2.BuildUnparse(sb, tabDepth + 1);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            EqualSegments eq = obj as EqualSegments;
            if (eq == null) return false;
            return eq.segment1.Equals(segment1) && eq.segment1.Equals(segment1);
        }

        public override string ToString()
        {
            return "Equal(" + segment1.ToString() + ", " + segment2.ToString() + "): " + justification;
        }
    }
}
