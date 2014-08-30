using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    public class SynthesizeProblemWindow : ChildWindow
    {
        private static readonly int ITEMS_PER_COL = 8;

        private List<EntryPanel> EntryPanels;

        /// <summary>
        /// Create the window.
        /// </summary>
        public SynthesizeProblemWindow()
        {
            Initialize();
            MakeEntryPanels();
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

            //Add the entry panels to the UI
            var verticalPanel = new StackPanel() { Orientation = Orientation.Vertical };
            var horizontalPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            horizontalPanel.Children.Add(verticalPanel);
            for (int i = 0; i < EntryPanels.Count; i++)
            {
                if (i % ITEMS_PER_COL == 0) //Start a new column
                {
                    verticalPanel = new StackPanel() { Orientation = Orientation.Vertical };
                    horizontalPanel.Children.Add(verticalPanel);
                }

                var panel = EntryPanels[i];
                panel.HorizontalAlignment = HorizontalAlignment.Right;
                panel.Margin = new Thickness(0, 0, 10, 5);
                verticalPanel.Children.Add(panel);
            }

            //Add the submit button
            Button submitBtn = new Button();
            submitBtn.Content = "Submit";
            submitBtn.HorizontalAlignment = HorizontalAlignment.Right;
            submitBtn.Click += new RoutedEventHandler(SubmitBtn_Click);

            //Organize UIElements and add them to the grid
            Grid.SetRow(horizontalPanel, 0);
            Grid.SetColumn(horizontalPanel, 0);
            grid.Children.Add(horizontalPanel);
            Grid.SetRow(submitBtn, 1);
            Grid.SetColumn(submitBtn, 0);
            grid.Children.Add(submitBtn);

            //Set the grid as the content of the window in order to display it.
            this.Content = grid;
        }

        /// <summary>
        /// Create the figure entry panels
        /// </summary>
        private void MakeEntryPanels()
        {
            EntryPanels = new List<EntryPanel>();
            EntryPanels.Add(new EntryPanel(ShapeType.TRIANGLE, "Triangle"));
            EntryPanels.Add(new EntryPanel(ShapeType.ISOSCELES_TRIANGLE, "Isosceles Triangle"));
            EntryPanels.Add(new EntryPanel(ShapeType.RIGHT_TRIANGLE, "Right Triangle"));
            EntryPanels.Add(new EntryPanel(ShapeType.ISO_RIGHT_TRIANGLE, "Isosceles Right Triangle"));
            EntryPanels.Add(new EntryPanel(ShapeType.EQUILATERAL_TRIANGLE, "Equilateral Triangle"));
            EntryPanels.Add(new EntryPanel(ShapeType.QUADRILATERAL, "Quadrilateral"));
            EntryPanels.Add(new EntryPanel(ShapeType.KITE, "Kite"));
            EntryPanels.Add(new EntryPanel(ShapeType.TRAPEZOID, "Trapezoid"));
            EntryPanels.Add(new EntryPanel(ShapeType.ISO_TRAPEZOID, "Isosceles Trapezoid"));
            EntryPanels.Add(new EntryPanel(ShapeType.PARALLELOGRAM, "Parallelogram"));
            EntryPanels.Add(new EntryPanel(ShapeType.RECTANGLE, "Rectangle"));
            EntryPanels.Add(new EntryPanel(ShapeType.RHOMBUS, "Rhombus"));
            EntryPanels.Add(new EntryPanel(ShapeType.SQUARE, "Square"));
            EntryPanels.Add(new EntryPanel(ShapeType.CIRCLE, "Circle"));
            EntryPanels.Add(new EntryPanel(ShapeType.SECTOR, "Sector"));
        }

        /// <summary>
        /// This event is executed when the submit button is clicked.
        /// This method will organize the information into a dictionary and pass it to the back end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            var figureCountMap = new Dictionary<ShapeType, int>();
            
            //Populate the dictionary
            foreach (var panel in EntryPanels)
            {
                int count = panel.EntryBox.Text.Equals("") ? 0 : int.Parse(panel.EntryBox.Text);
                figureCountMap.Add(panel.ShapeType, count);
            }

            //Complexity check
            int complexity = 0;
            figureCountMap.Values.ForEach(i => complexity += i);
            if (complexity > 3)
            {
                MessageBox.Show("Please only request 3 or less figures.",
                    "Too Many Figures!",
                    MessageBoxButton.OK);
                return;
            }

            GeometryTutorLib.FigureSynthesizerMain.SynthesizerMain(figureCountMap);
        }

        /// <summary>
        /// Allows the user to input how many of a certain figure they want.
        /// </summary>
        private class EntryPanel : StackPanel
        {
            public ShapeType ShapeType { get; private set; }
            public TextBlock Description { get; private set; }
            public TextBox EntryBox { get; private set; }

            public EntryPanel(ShapeType shapeType, string description)
                : base()
            {
                //StackPanel settings
                Orientation = Orientation.Horizontal;
                HorizontalAlignment = HorizontalAlignment.Right;

                //Save input parameter
                ShapeType = shapeType;

                //Make the description box
                Description = new TextBlock();
                Description.Text = description;
                Description.Margin = new Thickness(0, 0, 5, 0);
                Description.VerticalAlignment = VerticalAlignment.Center;
                Children.Add(Description);

                //Make the numeric entry box
                EntryBox = new NumericTextBox();
                EntryBox.Width = 30;
                Children.Add(EntryBox);
            }
        }
    }
}
