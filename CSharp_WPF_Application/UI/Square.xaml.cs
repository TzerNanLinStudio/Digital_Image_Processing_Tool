using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Square.xaml 的互動邏輯
    /// </summary>
    public partial class Square : UserControl
    {
        public delegate void NumberHandler(bool temp);
        public event NumberHandler NumberHandleEvent;

        /// <summary>
        /// Constructor for the Square UserControl. Initializes UI components.
        /// </summary>
        public Square()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for when the TextBox loses keyboard focus.
        /// Commits the entered number and updates the UI accordingly.
        /// </summary>
        /// <param name="sender">Source of the event (TextBox).</param>
        /// <param name="e">RoutedEventArgs containing event data.</param>
        private void TextBox_Number_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateNumber(); 
        }

        /// <summary>
        /// Event handler for key presses within the TextBox.
        /// Enter key commits the value, while Escape key cancels the edit.
        /// </summary>
        /// <param name="sender">Source of the event (TextBox).</param>
        /// <param name="e">KeyEventArgs containing details about the key pressed.</param>
        private void TextBox_Number_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateNumber();
                NumberHandleEvent(true);
            }
            else if (e.Key == Key.Escape)
            {
                TextBox_Number.Visibility = Visibility.Collapsed;
                Text_Number.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Updates the numeric value displayed, hides the TextBox, and shows the TextBlock.
        /// Parses the TextBox input and updates the TextBlock accordingly.
        /// </summary>
        private void UpdateNumber()
        {
            if (int.TryParse(TextBox_Number.Text, out int newValue))
            {
                Text_Number.Text = newValue.ToString(); 
            }
            TextBox_Number.Visibility = Visibility.Collapsed;
            Text_Number.Visibility = Visibility.Visible; 
        }

        /// <summary>
        /// Event handler for mouse clicks on the TextBlock.
        /// Activates the TextBox for editing the displayed number.
        /// </summary>
        /// <param name="sender">Source of the event (TextBlock).</param>
        /// <param name="e">MouseButtonEventArgs containing mouse click data.</param>
        private void Text_Number_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox_Number.Text = Text_Number.Text;
            TextBox_Number.Visibility = Visibility.Visible;
            Text_Number.Visibility = Visibility.Collapsed;
            TextBox_Number.Focus();
            TextBox_Number.SelectAll();
        }
    }
}
