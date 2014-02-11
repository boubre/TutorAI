﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicGeometry.UI.GivenWindow;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry;

namespace DynamicGeometry.UI
{
    public class ManageGivensWindow : ChildWindow
    {
        public DrawingHost drawingHost { get; set; }

        private Dictionary<string, AddGivenWindow> givenWindows;
        private Dictionary<string, GroundedClause> currentGivens;
        private ListBox givensList;
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
            currentGivens = new Dictionary<string, GroundedClause>();
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
            givensList = new ListBox();
            givensList.SelectionMode = SelectionMode.Extended;
            givensList.ItemsSource = currentGivens.Keys;
            givensList.MinHeight = 600;
            givensList.MaxHeight = 600;
            givensList.MinWidth = 400;
            givensList.MaxWidth = 400;
            givensList.Margin = new Thickness(0, 0, 0, 10);
            
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
            Grid.SetColumn(givensList, 0);
            Grid.SetRow(givensList, 0);
            grid.Children.Add(givensList);
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
        /// Get a list of the current user-defined givens.
        /// </summary>
        /// <returns>A list of the current user-defined givens.</returns>
        public List<GroundedClause> GetGivens()
        {
            return new List<GroundedClause>(currentGivens.Values);
        }

        /// <summary>
        /// This event executes when the "+" button is clicked.
        /// Will pop up a window so that the user can add the currently selected given in the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGivenBtn_Click(object sender, RoutedEventArgs e)
        {
            givenWindows[addSelection.SelectedValue as string].Show(new DrawingParser(drawingHost.CurrentDrawing), new List<GroundedClause>(currentGivens.Values));
        }

        /// <summary>
        /// This event executes when the "-" button is clicked.
        /// Will remove all selected items in the current givens list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveGivenBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (string selected in givensList.SelectedItems)
            {
                currentGivens.Remove(selected);
            }
        }

        /// <summary>
        /// This even executes when an add given window is closed.
        /// Will add the given if the window was accepted, and remeber the associated clause.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGivenWindow_Close(object sender, EventArgs e)
        {
            AddGivenWindow window = sender as AddGivenWindow;
            if (window.WindowResult == AddGivenWindow.Result.Accept)
            {
                GroundedClause clause = window.Clause;
                currentGivens.Add(clause.ToString(), clause);
            }
        }
    }
}
