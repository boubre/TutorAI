using System.Windows.Controls;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to configure custom Assumption groups.
    /// </summary>
    public class ParseGroupWindow : ChildWindow
    {
        /// <summary>
        /// Create the window.
        /// </summary>
        public ParseGroupWindow()
        {
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Assumption Groups Configuration";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
        }
    }
}
