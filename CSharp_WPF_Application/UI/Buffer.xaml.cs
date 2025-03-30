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

        /// <summary>
        /// Constructor initializes the buffer with default values and images.
        /// </summary>
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

        /// <summary>
        /// Retrieves a clone of the image at the specified index.
        /// </summary>
        /// <param name="index">Index of the image to retrieve.</param>
        /// <returns>A cloned Image if index is valid; otherwise, a default empty image.</returns>
        public Image<Bgr, byte> GetImage(int index)
        {
            if (index >= 0 && index < VisibleCount)
            {
                return ImgSource[index].Clone();
            }

            return ImgNull.Clone();
        }

        /// <summary>
        /// Adds an image to the buffer at the specified index, updating UI accordingly.
        /// </summary>
        /// <param name="index">Index at which to insert the new image.</param>
        /// <param name="text">Display text for the associated RadioButton.</param>
        /// <param name="image">The image to add to the buffer.</param>
        /// <returns>True if addition is successful; otherwise, false.</returns>
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

        /// <summary>
        /// Removes the image at the specified index and updates visibility.
        /// </summary>
        /// <param name="index">Index of the image to remove.</param>
        /// <returns>True if removal is successful; otherwise, false.</returns>
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

        /// <summary>
        /// Updates visibility of RadioButtons based on the current count of images.
        /// </summary>
        /// <param name="count">Number of RadioButtons to set as visible.</param>
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
        /// Resets all RadioButtons in the StackPanel to an unchecked state.
        /// </summary>
        public void ResetAllRadioButtons()
        {
            foreach (var child in StackPanel_TwelveRadioButton.Children)
            {
                if (child is RadioButton radioButton)
                {
                    radioButton.IsChecked = false;
                }
            }

            CurrentIndex = -1;
        }

        /// <summary>
        /// Event handler triggered when a RadioButton is checked. Updates current selection and triggers event.
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CurrentIndex = StackPanel_TwelveRadioButton.Children.IndexOf(sender as RadioButton);
            BufferHandleEvent(CurrentIndex);
        }
    }
}
