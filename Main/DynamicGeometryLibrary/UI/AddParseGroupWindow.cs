using System.Windows;
using System.Windows.Controls;

namespace DynamicGeometry.UI
{
    public class AddParseGroupWindow : ChildWindow
    {
        private TextBox NameInput;

        public string GroupName
        {
            get
            {
                return NameInput.Text;
            }
            set
            {
                NameInput.Text = value;
            }
        }

        /// <summary>
        /// Create the window.
        /// </summary>
        public AddParseGroupWindow()
        {
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Add Assumption Group";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
            this.HasCloseButton = false;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            StackPanel panel = new StackPanel();

            TextBlock label = new TextBlock();
            label.Text = "Name the new group:";
            panel.Children.Add(label);

            NameInput = new TextBox();
            NameInput.MaxWidth = 200;
            NameInput.MinWidth = 200;
            NameInput.Margin = new Thickness(0, 0, 0, 10);
            panel.Children.Add(NameInput);

            Button okBtn = new Button();
            okBtn.Content = "Add";
            okBtn.HorizontalAlignment = HorizontalAlignment.Center;
            okBtn.Width = 75;
            okBtn.Click += new RoutedEventHandler(OKButton_Click);
            panel.Children.Add(okBtn);

            this.Content = panel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
