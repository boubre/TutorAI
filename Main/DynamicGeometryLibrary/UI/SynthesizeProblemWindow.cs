using System.Windows;
using System.Windows.Controls;

namespace DynamicGeometry.UI
{
    public class SynthesizeProblemWindow : ChildWindow
    {
        /// <summary>
        /// Create the window.
        /// </summary>
        public SynthesizeProblemWindow()
        {
            Initialize();
            MakeGivens();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Synthesize Problem";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up the grid.
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            

            //Set the grid as the content of the window in order to display it.
            this.Content = grid;
        }

        /// <summary>
        /// Create the add given windows and set up the list of givens.
        /// </summary>
        private void MakeGivens()
        {
            
        }
    }
}
