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

namespace AOI_UI
{
    /// <summary>
    /// CameraSetting.xaml 的互動邏輯
    /// </summary>
    public partial class OffsetSetting : UserControl
    {
        public delegate void TextBoxHandler(uint DeviceNunber, String ParameterName, int ParameterValue);
        public event TextBoxHandler TextBoxHandlerEvent;

        public delegate void ButtonHandler(uint DeviceNunber);
        public event ButtonHandler ButtonHandlerEvent;

        public uint CameraNumber;
        public String CameraName;

        public OffsetSetting()
        {
            InitializeComponent();
        }

        public void Inittialization(uint DeviceNunber, String DeviceName, double ExposureTime, double Gain, double AcquisitionFrameRate)
        {

        }

        private void Btn_CameraConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NumericUpDown_CameraParameter_ValueChanged(object sender, EventArgs e)
        {
            switch ((sender as System.Windows.Forms.NumericUpDown).AccessibleName)
            {

                           
                default:
                    Console.WriteLine("Input Value Was Not Suitable When Camera Value Was Setting.");
                    break;
            }
        }
    }
}