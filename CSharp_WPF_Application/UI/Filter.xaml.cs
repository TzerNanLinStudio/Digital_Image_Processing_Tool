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
    /// Filter.xaml 的互動邏輯
    /// </summary>
    public partial class Filter : UserControl
    {
       // public delegate void FilterHandler(float[,] kernel, bool temp);
        public delegate void FilterHandler(float[][] kernel, bool temp);
        public event FilterHandler FilterHandleEvent;

        public Filter()
        {
            InitializeComponent();

            for (int x = 0; x < StackPanel_NineSquare.Children.Count; x++)
            {
                for (int y = 0; y < (StackPanel_NineSquare.Children[x] as StackPanel).Children.Count; y++)
                {
                    ((StackPanel_NineSquare.Children[x] as StackPanel).Children[y] as Square).NumberHandleEvent += UpFilter;
                }
            }
        }

        // Handle right mouse button click on StackPanel_NineSquare
        private void StackPanel_NineSquare_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpFilter(false);
        }

        private void UpFilter(bool temp)
        {
            // Initialize a 3x3 jagged float array, assuming StackPanel is a 3x3 grid
            float[][] kernel = new float[3][];
            for (int x = 0; x < 3; x++)
            {
                kernel[x] = new float[3]; // Initialize each row
            }

            for (int x = 0; x < StackPanel_NineSquare.Children.Count; x++)
            {
                for (int y = 0; y < (StackPanel_NineSquare.Children[x] as StackPanel).Children.Count; y++)
                {
                    string numberText = ((StackPanel_NineSquare.Children[x] as StackPanel).Children[y] as Square).Text_Number.Text;
                    if (float.TryParse(numberText, out float value))  // Convert string to float and store in kernel array
                    {
                        kernel[x][y] = value;
                    }
                    else  // Default to 0 if conversion fails
                    {
                        kernel[x][y] = 0f;
                    }
                }
            }

            // Trigger the event with the kernel
            FilterHandleEvent(kernel, temp);
        }
    }
}
