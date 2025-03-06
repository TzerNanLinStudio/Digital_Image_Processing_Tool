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
        public Square()
        {
            InitializeComponent();
        }

        // 失去焦點時，將 TextBox 內容更新到 TextBlock，並隱藏 TextBox
        private void TextBox_Number_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateNumber();
        }

        // 按下 Enter 鍵確認輸入，按下 Esc 鍵取消修改
        private void TextBox_Number_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateNumber();
            }
            else if (e.Key == Key.Escape)
            {
                TextBox_Number.Visibility = Visibility.Collapsed;
                Text_Number.Visibility = Visibility.Visible;
            }
        }

        // 更新數值並隱藏 TextBox
        private void UpdateNumber()
        {
            if (int.TryParse(TextBox_Number.Text, out int newValue))
            {
                Text_Number.Text = newValue.ToString();
            }
            TextBox_Number.Visibility = Visibility.Collapsed;
            Text_Number.Visibility = Visibility.Visible;
        }

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
