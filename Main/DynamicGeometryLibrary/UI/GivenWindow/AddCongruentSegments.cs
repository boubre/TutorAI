using System;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddCongruentSegments : AddGivenWindow
    {
        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddCongruentSegments() : base("Congruent Segments")
        {
        }

        protected override Grid MakeGivenGrid()
        {
            return new Grid() { MinWidth = 400 };
        }

        protected override void OnShow()
        {
            throw new NotImplementedException();
        }

        protected override GroundedClause MakeClause()
        {
            throw new NotImplementedException();
        }
    }
}
