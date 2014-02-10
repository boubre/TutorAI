using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DynamicGeometry.UI.GivenWindow
{
    public abstract class AddGivenWindow : ChildWindow
    {
        protected string givenName;

        /// <summary>
        /// Create the window.
        /// </summary>
        /// <param name="givenName">The name of the given being added.</param>
        protected AddGivenWindow(string givenName)
        {
            this.givenName = givenName;
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Add Given: " + givenName;
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Should layout the design for the particular given.
        /// </summary>
        /// <returns>A Grid with UI components that create the given.</returns>
        protected abstract Grid MakeGivenGrid();

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up the grid.

            //Arrange items in the grid and add them to the grid.

            //Set the grid as the content of the window in order to display it.
            //this.Content = grid;
        }
    }
}
