using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddParallelLines : AddGivenWindow
    {
        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddParallelLines() : base("Parallel Lines")
        {
        }

        protected override Grid MakeGivenGrid()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnShow()
        {
            throw new System.NotImplementedException();
        }

        protected override GroundedClause MakeClause()
        {
            throw new System.NotImplementedException();
        }
    }
}
