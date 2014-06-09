using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to receive hints and enter solutions.
    /// </summary>
    public class EnterSolutionWindow : ChildWindow
    {
        /// <summary>
        /// Create the window.
        /// </summary>
        public EnterSolutionWindow()
        {
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Enter Solution";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            TextBlock test = new TextBlock();
            test.Text = "This is a test";

            //Set element locations in grid and add to grid
            Grid.SetColumn(test, 0);
            Grid.SetRow(test, 1);
            grid.Children.Add(test);

            //Set the content of the window to be the newly designed layout
            this.Content = grid;
        }
    }
}
