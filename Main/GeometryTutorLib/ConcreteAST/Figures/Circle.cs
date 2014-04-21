
namespace GeometryTutorLib.ConcreteAST.Figures
{
    public class Circle : Figure
    {
        public Point Center { get; private set; }
        public double Radius { get; private set; }

        /// <summary>
        /// Create a new circle.
        /// </summary>
        /// <param name="Center">The center of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        public Circle(Point Center, double Radius)
        {
            this.Center = Center;
            this.Radius = Radius;
        }

        /// <returns>A deep copy of this object</returns>
        public override GroundedClause DeepCopy()
        {
            Circle other = (Circle)(this.MemberwiseClone());
            other.Center = (Point)Center.DeepCopy();
            other.Radius = Radius;

            return other;
        }

        /// <summary>
        /// Check for structural equality. The circles should have the same radius and and center.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is structurally equal to this circle.</returns>
        public override bool StructurallyEquals(object obj)
        {
            Circle circle = obj as Circle;
            if (circle == null) return false;

            if (!Utilities.CompareValues(Radius, circle.Radius)) return false;

            if (!Center.StructurallyEquals(circle.Center)) return false;

            return true;
        }

        public override string ToString()
        {
            return "Circle( center: " + Center.ToString() + "; radius: " + Radius + ")";
        }
    }
}
