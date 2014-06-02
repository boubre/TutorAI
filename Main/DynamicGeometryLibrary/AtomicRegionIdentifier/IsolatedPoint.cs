using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.AtomicRegionIdentifier
{
    public class IsolatedPoint : Primitive
    {
        Point thePoint;

        public IsolatedPoint(Point pt)
        {
            thePoint = pt;
        }

        public override string ToString()
        {
            return "Point { " + thePoint.ToString() + " }";
        }
    }
}