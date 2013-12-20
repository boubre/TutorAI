using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to configure which axioms, defintions, and theorems the back end is permitted to use.
    /// </summary>
    public class ParseOptionsWindow : ChildWindow
    {
        private ListBox visibibleAssumptions;
        private List<CheckBox> assumptionCheckboxes;

        /// <summary>
        /// Create the window.
        /// </summary>
        public ParseOptionsWindow()
        {
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Parse Options Configuration";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            visibibleAssumptions = new ListBox();
            visibibleAssumptions.MaxWidth = 400;
            visibibleAssumptions.MaxHeight = 800;
            visibibleAssumptions.MinWidth = 400;
            visibibleAssumptions.MinHeight = 200;
            MakeAssumptionCheckBoxes();
            visibibleAssumptions.ItemsSource = assumptionCheckboxes;

            Grid.SetColumn(visibibleAssumptions, 0);
            Grid.SetRow(visibibleAssumptions, 0);
            grid.Children.Add(visibibleAssumptions);

            this.Content = grid;
        }

        /// <summary>
        /// Make a check box for each assumption. These checkboxes will automatically update the "Enabled" field of their corresponding assumption.
        /// </summary>
        private void MakeAssumptionCheckBoxes()
        {
            assumptionCheckboxes = new List<CheckBox>();
            foreach (Assumption assumption in Assumption.GetAssumptions())
            {
                CheckBox cb = new CheckBox();
                cb.IsChecked = assumption.Enabled;
                cb.Checked += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e) { assumption.Enabled = true; });
                cb.Unchecked += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e) { assumption.Enabled = false; });
                string assumptionType;
                switch (assumption.Type)
                {
                    case Assumption.AssumptionType.Axiom:
                        assumptionType = "Axiom";
                        break;
                    case Assumption.AssumptionType.Definition:
                        assumptionType = "Def";
                        break;
                    case Assumption.AssumptionType.Theorem:
                        assumptionType = "Thm";
                        break;
                    default:
                        assumptionType = "";
                        break;
                }
                cb.Content = assumptionType + ": " + assumption.Name;
                assumptionCheckboxes.Add(cb);
            }
        }
    }
}
