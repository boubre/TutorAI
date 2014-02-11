using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicGeometry.UI.GivenWindow;

namespace DynamicGeometry.UI
{
    public class ManageGivensWindow : ChildWindow
    {
        private Dictionary<string, AddGivenWindow> givenWindows;
        private ListBox currentGivens;
        private ComboBox addSelection;

        /// <summary>
        /// Create the window.
        /// </summary>
        public ManageGivensWindow()
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
            this.Title = "Manage Givens";
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

            //Set up list of current givens
            currentGivens = new ListBox();
            currentGivens.SelectionMode = SelectionMode.Extended;
            currentGivens.MinHeight = 600;
            currentGivens.MaxHeight = 600;
            currentGivens.MinWidth = 400;
            currentGivens.MaxWidth = 400;
            currentGivens.Margin = new Thickness(0, 0, 0, 10);
            
            //Set up add and remove panel
            StackPanel addRemPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            addSelection = new ComboBox();
            addSelection.ItemsSource = givenWindows.Keys;
            addSelection.SelectedItem = addSelection.Items[0];
            addSelection.Margin = new Thickness(0, 0, 5, 0);
            addSelection.MinWidth = 340;
            addRemPanel.Children.Add(addSelection);
            Button addBtn = new Button();
            addBtn.Content = "+";
            addBtn.Click += new RoutedEventHandler(AddGivenBtn_Click);
            addBtn.Margin = new Thickness(0, 0, 5, 0);
            addBtn.MinWidth = 25;
            addRemPanel.Children.Add(addBtn);
            Button remBtn = new Button();
            remBtn.Content = "-";
            remBtn.Click += new RoutedEventHandler(RemoveGivenBtn_Click);
            remBtn.MinWidth = 25;
            addRemPanel.Children.Add(remBtn);

            //Arrange items in the grid and add them to the grid.
            Grid.SetColumn(currentGivens, 0);
            Grid.SetRow(currentGivens, 0);
            grid.Children.Add(currentGivens);
            Grid.SetColumn(addRemPanel, 0);
            Grid.SetRow(addRemPanel, 1);
            grid.Children.Add(addRemPanel);

            //Set the grid as the content of the window in order to display it.
            this.Content = grid;
        }

        /// <summary>
        /// Create the add given windows and set up the list of givens.
        /// </summary>
        private void MakeGivens()
        {
            givenWindows = new Dictionary<string, AddGivenWindow>();
            givenWindows.Add("Congruent Segments", new AddCongruentSegments());
            givenWindows.Add("Congruent Angles", null);
            givenWindows.Add("Segment Bisector", null);
            givenWindows.Add("Angle Bisector", null);
            givenWindows.Add("Right Angle", null);
            givenWindows.Add("Parallel Lines", null);
            givenWindows.Add("Isosceles Triangle", null);
            givenWindows.Add("Equilateral Triangle", null);
            givenWindows.Add("Congruent Triangles", null);
            givenWindows.Add("Similar Triangles", null);

            foreach (AddGivenWindow w in givenWindows.Values)
            {
                if (w == null) { continue; } //Temporary line. TODO: Remove once all givens implemented
                w.Closed += new EventHandler(AddGivenWindow_Close);
            }
        }

        /// <summary>
        /// This event executes when the "+" button is clicked.
        /// Will pop up a window so that the user can add the currently selected given in the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGivenBtn_Click(object sender, RoutedEventArgs e)
        {
            givenWindows[addSelection.SelectedValue as string].Show();
        }

        /// <summary>
        /// This event executes when the "-" button is clicked.
        /// Will remove all selected items in the current givens list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveGivenBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// This even executes when an add given window is closed.
        /// Will add the given if the window was accepted, and remeber the associated clause.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGivenWindow_Close(object sender, EventArgs e)
        {
        }
    }
}
