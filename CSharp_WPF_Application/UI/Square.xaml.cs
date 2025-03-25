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

        public Square()
        {
            InitializeComponent();
        }

        // When the TextBox loses focus, update the TextBlock content and hide the TextBox
        private void TextBox_Number_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateNumber(); 
        }

        // Handle Enter and Escape keys: Enter confirms input, Escape cancels the edit
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

        // Update the value and hide the TextBox
        private void UpdateNumber()
        {
            if (int.TryParse(TextBox_Number.Text, out int newValue))
            {
                Text_Number.Text = newValue.ToString(); 
            }
            TextBox_Number.Visibility = Visibility.Collapsed;
            Text_Number.Visibility = Visibility.Visible; 
        }

        // When the TextBlock is clicked, switch to the TextBox for editing
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
