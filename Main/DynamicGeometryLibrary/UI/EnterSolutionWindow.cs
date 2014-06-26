using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicGeometry.UI.GivenWindow;
using GeometryTutorLib.EngineUIBridge;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to receive hints and enter solutions.
    /// </summary>
    public class EnterSolutionWindow : ChildWindow
    {
        public DrawingHost drawingHost { get; set; }

        private Dictionary<string, GroundedClause> currentGivens;
        private Dictionary<string, AddGivenWindow> givenWindows;
        private List<CheckBox> affectedCheckboxes = new List<CheckBox>();
        private ListBox solutionList;
        private ListBox justList;
        private ListBox hintList;
        private ComboBox addSolution;
        private ComboBox addJustification;

        CheckBox[] checkboxes = new CheckBox[5];


        /// <summary>
        /// Create the window.
        /// </summary>
        public EnterSolutionWindow()
        {
            Initialize();
            MakeSolutions();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Enter Solution Window";
            currentGivens = new Dictionary<string, GroundedClause>();
            this.MaxHeight = 650;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {


            //checkbox instantiate
            checkboxes[0] = new CheckBox();

            //Set up grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });


            //Set up list of current solutions
            solutionList = new ListBox();
            solutionList.SelectionMode = SelectionMode.Extended;
            solutionList.ItemsSource = currentGivens.Keys;
            solutionList.MinHeight = 100;
            solutionList.MaxHeight = 100;
            solutionList.MinWidth = 230;
            solutionList.MaxWidth = 230;
            solutionList.Margin = new Thickness(0, 0, 5, 5);

            //Set up list of current justifications
            justList = new ListBox();
            justList.SelectionMode = SelectionMode.Extended;
            justList.ItemsSource = currentGivens.Keys;
            justList.MinHeight = 100;
            justList.MaxHeight = 100;
            justList.MinWidth = 230;
            justList.MaxWidth = 230;
            justList.Margin = new Thickness(0, 0, 5, 5);

            //Set up hint list title text block
            TextBlock hint = new TextBlock();
            hint.Text = "Hint:";
            hint.Margin = new Thickness(0, 10, 5, 0);
            hint.TextAlignment = TextAlignment.Left;

            //Set up list of current hints
            hintList = new ListBox();
            hintList.SelectionMode = SelectionMode.Extended;
            hintList.ItemsSource = currentGivens.Keys;
            hintList.MinHeight = 100;
            hintList.MaxHeight = 100;
            hintList.MinWidth = 230;
            hintList.MaxWidth = 230;
            hintList.Margin = new Thickness(0, 0, 5, 5);

            //Set up add and remove panel
            StackPanel addRemPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            addSolution = new ComboBox();
            addSolution.ItemsSource = givenWindows.Keys;
            addSolution.SelectedItem = addSolution.Items[0];
            addSolution.Margin = new Thickness(0, 5, 0, 5);
            addSolution.MinWidth = 160;
            addSolution.MinHeight = 25;
            addSolution.MaxHeight = 25;
            addRemPanel.Children.Add(addSolution);
            Button addBtn = new Button();
            addBtn.Content = "+";
            addBtn.Click += new RoutedEventHandler(AddSolutionBtn_Click);
            addBtn.Margin = new Thickness(2, 5, 0, 5);
            addBtn.MinWidth = 25;
            addBtn.MinHeight = 25;
            addBtn.MaxHeight = 25;
            addRemPanel.Children.Add(addBtn);
            Button remBtn = new Button();
            remBtn.Content = "-";
            remBtn.Click += new RoutedEventHandler(RemoveSolutionBtn_Click);
            remBtn.Margin = new Thickness(2, 5, 0, 5);
            remBtn.MinWidth = 25;
            remBtn.MinHeight = 25;
            remBtn.MaxHeight = 25;
            addRemPanel.Children.Add(remBtn);

            //Set up justification add and remove panel
            StackPanel justAddPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            addJustification = new ComboBox();
            addJustification.ItemsSource = givenWindows.Keys;
            addJustification.SelectedItem = addSolution.Items[0];
            addJustification.Margin = new Thickness(0, 5, 0, 5);
            addJustification.MinWidth = 177;
            addJustification.MinHeight = 25;
            addJustification.MaxHeight = 25;
            justAddPanel.Children.Add(addJustification);
            Button justAddBtn = new Button();
            justAddBtn.Content = "+";
            justAddBtn.Click += new RoutedEventHandler(AddSolutionBtn_Click);
            justAddBtn.Margin = new Thickness(2, 5, 0, 5);
            justAddBtn.MinWidth = 25;
            justAddBtn.MinHeight = 25;
            justAddBtn.MaxHeight = 25;
            justAddPanel.Children.Add(justAddBtn);
            Button justRemBtn = new Button();
            justRemBtn.Content = "-";
            justRemBtn.Click += new RoutedEventHandler(RemoveSolutionBtn_Click);
            justRemBtn.Margin = new Thickness(2, 5, 0, 5);
            justRemBtn.MinWidth = 25;
            justRemBtn.MinHeight = 25;
            justRemBtn.MaxHeight = 25;
            justAddPanel.Children.Add(justRemBtn);

            // Create a CheckBox object
            CheckBox solutionCheckBox = new CheckBox();
            solutionCheckBox.Width = 20;
            solutionCheckBox.Height = 20;
            affectedCheckboxes.Add(solutionCheckBox);

            // Create a CheckBox object
            CheckBox otherCheckBox = new CheckBox();
            otherCheckBox.Width = 20;
            otherCheckBox.Height = 20;
            affectedCheckboxes.Add(otherCheckBox);

            // Create Select All Checkbox title panel
            StackPanel selTitlePanel = new StackPanel() { Orientation = Orientation.Vertical };
            TextBlock select = new TextBlock();
            select.Text = "Select";
            select.Margin = new Thickness(0, 0, 5, 0);
            select.TextAlignment = TextAlignment.Center;
            select.VerticalAlignment = VerticalAlignment.Bottom;
            TextBlock all = new TextBlock();
            all.Text = "All";
            all.Margin = new Thickness(0, 0, 5, 0);
            all.TextAlignment = TextAlignment.Center;
            all.VerticalAlignment = VerticalAlignment.Bottom;
            selTitlePanel.Children.Add(select);
            selTitlePanel.Children.Add(all);

            // Create a Select All CheckBox object
            CheckBox selectCheckBox = new CheckBox();
            selectCheckBox.Width = 20;
            selectCheckBox.Height = 20;

            // Create a submit button
            Button subBtn = new Button();
            subBtn.Content = "Done";
            subBtn.Margin = new Thickness(0, 0, 10, 5);
            subBtn.Width = 75;
            subBtn.HorizontalAlignment = HorizontalAlignment.Right;

            //Set element locations in grid and add to grid
            // 01
            Grid.SetColumn(solutionList, 0);
            Grid.SetRow(solutionList, 1);
            grid.Children.Add(solutionList);
            // 02
            Grid.SetColumn(addRemPanel, 0);
            Grid.SetRow(addRemPanel, 2);
            grid.Children.Add(addRemPanel);
            // 03
            Grid.SetColumn(hint, 0);
            Grid.SetRow(hint, 3);
            grid.Children.Add(hint);
            // 04
            Grid.SetColumn(hintList, 0);
            Grid.SetRow(hintList, 4);
            grid.Children.Add(hintList);
            // 11
            Grid.SetColumn(justList, 1);
            Grid.SetRow(justList, 1);
            grid.Children.Add(justList);
            // 12
            Grid.SetColumn(justAddPanel, 1);
            Grid.SetRow(justAddPanel, 2);
            grid.Children.Add(justAddPanel);
            // 13

            // 16
            Grid.SetColumn(subBtn, 1);
            Grid.SetRow(subBtn, 6);
            grid.Children.Add(subBtn);
            // 21
            Grid.SetColumn(solutionCheckBox, 2);
            Grid.SetRow(solutionCheckBox, 1);
            grid.Children.Add(solutionCheckBox);
            // 23

            // 24
            Grid.SetColumn(otherCheckBox, 2);
            Grid.SetRow(otherCheckBox, 4);
            grid.Children.Add(otherCheckBox);
            // 25
            Grid.SetColumn(selTitlePanel, 2);
            Grid.SetRow(selTitlePanel, 5);
            grid.Children.Add(selTitlePanel);
            // 26
            Grid.SetColumn(selectCheckBox, 2);
            Grid.SetRow(selectCheckBox, 6);
            grid.Children.Add(selectCheckBox);

            //Set the content of the window to be the newly designed layout
            this.Content = grid;
        }


        /// <summary>
        /// Create the add given windows and set up the list of givens.
        /// </summary>
        private void MakeSolutions()
        {
            givenWindows = new Dictionary<string, AddGivenWindow>();
            givenWindows.Add("Congruent Segments", new AddCongruentSegments());
            // CTA: Need Midpoint
            givenWindows.Add("Congruent Angles", new AddCongruentAngles());
            givenWindows.Add("Segment Bisector", new AddSegmentBisector());
            givenWindows.Add("Angle Bisector", new AddAngleBisector());
            givenWindows.Add("Right Angle", new AddRightAngle());
            givenWindows.Add("Parallel Lines", new AddParallelLines());
            givenWindows.Add("Isosceles Triangle", new AddIsoscelesTriangle());
            givenWindows.Add("Equilateral Triangle", new AddEquilateralTriangle());
            givenWindows.Add("Congruent Triangles", new AddCongruentTriangles());
            givenWindows.Add("Similar Triangles", new AddSimilarTriangles());

            foreach (AddGivenWindow w in givenWindows.Values)
            {
                w.Closed += new EventHandler(AddGivenWindow_Close);
            }
        }


        /// <summary>
        /// This event executes when the "+" button is clicked.
        /// Will pop up a window so that the user can add the currently selected given in the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSolutionBtn_Click(object sender, RoutedEventArgs e)
        {
            givenWindows[addSolution.SelectedValue as string].Show(new LiveGeometry.TutorParser.DrawingParserMain(drawingHost.CurrentDrawing), new List<GroundedClause>(currentGivens.Values));
        }

        /// <summary>
        /// This event executes when the "-" button is clicked.
        /// Will remove all selected items in the current givens list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSolutionBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (string selected in solutionList.SelectedItems)
            {
                currentGivens.Remove(selected);
            }

            //Graphically refresh the solution list.
            solutionList.ItemsSource = null;
            solutionList.ItemsSource = currentGivens.Keys;
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
                if (clause != null)
                {
                    currentGivens.Add(clause.ToString(), clause);
                }
            }

            //Graphically refresh the givens list.
            solutionList.ItemsSource = null;
            solutionList.ItemsSource = currentGivens.Keys;
        }


        //private void SelectAll_Checkbox(object sender, EventArgs e)
          //{
           //foreach (var row in grid.Rows)
          //{
            //    var checkBox = (CheckBox)row.FindControl("selectCheckBox");
              // checkBox.Checked = cbSelectAll.Checked;
          //}
         //}

         //public void CheckBoxChanged(Object sender, EventArgs e)
        //{
          //var isSelectAll = true;

        //foreach (var row in grid.Rows)
        //{
          //var checkBox = (CheckBox)row.FindControl("selectCheckBox");

        //if (!checkBox.Checked)
         //{
          //isSelectAll = false;
         //break;
         //}
        //}

        //cbSelectAll.Checked = isSelectAll;
        //}

    }
}
