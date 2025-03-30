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
using Emgu.CV;
using Emgu.CV.Flann;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;

namespace UI
{
    /// <summary>
    /// Buffer.xaml 的互動邏輯
    /// </summary>
    public partial class Buffer : UserControl
    {
        public int VisibleCount;
        public int CurrentIndex;
        public bool TempFlag;
        public Image<Bgr, byte> ImgNull;
        public Image<Bgr, byte> ImgTemp;
        public Image<Bgr, byte>[] ImgSource;

        public delegate void BufferHandler(int index = -1);
        public event BufferHandler BufferHandleEvent;

        public Buffer()
        {
            InitializeComponent();

            VisibleCount = 0;
            CurrentIndex = -1;
            TempFlag = false;
            ImgNull = new Image<Bgr, byte>(64, 64);
            ImgTemp = ImgNull.Clone();
            ImgSource = new Image<Bgr, byte>[StackPanel_TwelveRadioButton.Children.Count];
            for (int i = 0; i < StackPanel_TwelveRadioButton.Children.Count; i++) ImgSource[i] = ImgNull.Clone();
        }

        public Image<Bgr, byte> GetImage(int index)
        {
            if (index >= 0 && index < VisibleCount)
            {
                return ImgSource[index].Clone();
            }

            return ImgNull.Clone();
        }

        public bool AddImage(int index, string text, Image<Bgr, byte> image)
        {
            if (VisibleCount < StackPanel_TwelveRadioButton.Children.Count)
            {
                if (index >= 0 && index < VisibleCount)
                {
                    for (int i = VisibleCount; i > index; i--)
                    {
                        ImgSource[i] = ImgSource[i - 1].Clone();
                    }
                }
                else if (index > VisibleCount)
                {
                    return false;
                }

                UpdateBufferVisibility(++VisibleCount);

                ImgSource[index] = image.Clone();

                (StackPanel_TwelveRadioButton.Children[index] as RadioButton).Content = text;
                (StackPanel_TwelveRadioButton.Children[index] as RadioButton).IsChecked = true;

                return true;
            }

            return false;
        }

        public bool RemoveImage(int index)
        {
            if (index >= 0 && index < VisibleCount)
            {
                for (int i = index; i < VisibleCount - 1; i++)
                {
                    Console.WriteLine(i);
                    ImgSource[i] = ImgSource[i + 1].Clone();
                }

                UpdateBufferVisibility(--VisibleCount);

                return true;
            }

            return false; 
        }

        public void UpdateBufferVisibility(int count)
        {
            if (count > 0 && count <= StackPanel_TwelveRadioButton.Children.Count)
            {
                for (int i = 0; i < StackPanel_TwelveRadioButton.Children.Count; i++)
                {
                    StackPanel_TwelveRadioButton.Children[i].Visibility = i < count ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// 重置StackPanel中所有RadioButton为未选中状态
        /// </summary>
        public void ResetAllRadioButtons()
        {
            // 遍历StackPanel_TwelveRadioButton中的所有子元素
            foreach (var child in StackPanel_TwelveRadioButton.Children)
            {
                // 检查子元素是否为RadioButton
                if (child is RadioButton radioButton)
                {
                    // 将RadioButton设置为未选中状态
                    radioButton.IsChecked = false;
                }
            }

            CurrentIndex = -1;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CurrentIndex = StackPanel_TwelveRadioButton.Children.IndexOf(sender as RadioButton);
            BufferHandleEvent(CurrentIndex);
        }
    }
}
