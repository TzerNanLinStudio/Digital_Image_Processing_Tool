using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Windows.Media.Media3D;
using System.Security.Cryptography;
using Microsoft.Win32;
using ImageProcessing;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Omron.Ether;

namespace AOI_UI
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        #region "Parameter"
        // Commend Window Parameter
        // --------------------------------------------------
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        IntPtr Hwnd = GetConsoleWindow();
        // --------------------------------------------------

        //UI Parameter
        // --------------------------------------------------
        public BitmapImage[] StatusSign = new BitmapImage[2];
        public string[] StatusText = new string[6] { "Pass", "Fail", "Error", "ByPass", "", "00.00" };
        public bool[] ControlIsEnableFlag = new bool[2] { true, true };
        public bool[] SampleConfigFlag = new bool[3] { false, false, false };
        public bool IsInitializationFlag = false;
        public bool AOIRunFlag = false;
        public bool VideoRunFlag = false;
        public bool ROIChooseFlag = false;
        public int[] ImageSource = new int[3] { -1, -1, -1 };
        public int ROINumber = 0;
        public int CenterNumber = 0;
        // --------------------------------------------------

        // Used For Picture Box Selection
        // --------------------------------------------------
        public bool IsShowPixelValue = true;
        public bool IsMouseDown = false;

        public int FirstPictureBox_ROILength = 0;
        public int ThirdPictureBox_ROIWidth = 0;

        public bool Selecting = false;
        public System.Drawing.Rectangle rectPictureBox = System.Drawing.Rectangle.Empty;
        public System.Drawing.Point StartPoint_PictureBox;
        public System.Drawing.Point EndPoint_PictureBox;
        public System.Drawing.Point CenterPoint_PictureBox;

        public bool SelectROI = false;
        public System.Drawing.Rectangle rectImageROI;
        public System.Drawing.Point ImageROI_StartPoint;
        public System.Drawing.Point ImageROI_EndPoint;

        public bool[] SelectCenter = new bool[3] { false, false, false };
        public System.Drawing.Point Image_CenterPoint;
        // --------------------------------------------------

        // Camera Parameter
        // --------------------------------------------------
        private static int CameraImageWidth = 2592;
        private static int CameraImageHeight = 1944;
        // --------------------------------------------------

        // IP Parameter
        // --------------------------------------------------
        public D_FirstStation FirstStationData;
        private static Thread IP_1st;
        public int[] ToIP = new int[3] { 0, 0, 0 };
        public int[] ToPLC = new int[3] { 0, 0, 0 };
        // --------------------------------------------------

        // Other Parameter
        // --------------------------------------------------
        public System.String CurrentDirectory = System.Environment.CurrentDirectory;
        private static readonly System.Globalization.CultureInfo CI = System.Globalization.CultureInfo.InvariantCulture;
        // --------------------------------------------------

        // Debug Parameter
        // --------------------------------------------------
        public Stopwatch MyWatch01 = new Stopwatch();
        public Stopwatch MyWatch02 = new Stopwatch();
        public Stopwatch MyWatch03 = new Stopwatch();
        private static Thread DebugThread;
        public bool[] DebugFlag = new bool[5] { true, true, false, false, false };
        public bool[] PrintFlag = new bool[5] { false, true, false, false, false };
        // --------------------------------------------------
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeAOI();
        }

        #region "Event"
        private void ImageRecord_ParameterEvent(bool t_CloseWindow, bool t_SaveConfig, bool t_ClearData, bool t_ChooseOrigin, bool t_ChooseResult, int t_Limit, int t_Rate)
        {
            try
            {

            }
            catch (Exception Ex)
            {
                WriteLogOnUI("Error", "設定影像留存時發生異常!");
            }
        }

        private void F_ResultReturn()
        {
            //Return 1st Result To UI.
            {
                //Display Image In ImageBox.
                {
                    //if (AOIRunFlag)
                        //Dispatcher.Invoke(() => MyCameraView[0].ImageBoxDisplay.Image = FirstStationData.GetImage(0));

                    ImageSource[0] = 1;
                    Dispatcher.Invoke(() => ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]));
                }

                //Return Result Of Inspection Item "三芯線轉線角度判別" To UI.
                switch (FirstStationData.InspectionResultFlag[0])
                {
                    //Result Of Inspection Item "三芯線轉線角度判別" Is Error.
                    case -1:
                        Dispatcher.Invoke(() => Border_F_Item01_PartResult.Background = Brushes.Red);
                        Dispatcher.Invoke(() => Label_F_Item01_PartResult.Content = "Degree : " + StatusText[2]);
                        break;

                    //Result Of Inspection Item "三芯線轉線角度判別" Is Null.
                    case 0:
                        Dispatcher.Invoke(() => Border_F_Item01_PartResult.Background = Brushes.Green);
                        Dispatcher.Invoke(() => Label_F_Item01_PartResult.Content = "Degree : " + StatusText[4]);
                        break;

                    //Result Of Inspection Item "三芯線轉線角度判別" Is NG.
                    case 1:
                        Dispatcher.Invoke(() => Border_F_Item01_PartResult.Background = Brushes.Red);
                        Dispatcher.Invoke(() => Label_F_Item01_PartResult.Content = "Degree : " + FirstStationData.Item01_ProcessAngle.ToString() + "°");
                        break;

                    //Result Of Inspection Item "三芯線轉線角度判別" Is OK.
                    case 2:
                        Dispatcher.Invoke(() => Border_F_Item01_PartResult.Background = Brushes.Green);
                        Dispatcher.Invoke(() => Label_F_Item01_PartResult.Content = "Degree : " + FirstStationData.Item01_ProcessAngle.ToString() + "°");
                        break;

                    default:
                        Dispatcher.Invoke(() => Border_F_Item01_PartResult.Background = Brushes.Red);
                        Dispatcher.Invoke(() => Label_F_Item01_PartResult.Content = "Degree : " + StatusText[2]);
                        WriteLogOnUI("Error", "K1影像處理於<三芯線轉線角度判別>時發生異常!");
                        break;
                }

                //Return Result Of Inspection Item "三芯線色差判別" To UI.
                switch (FirstStationData.InspectionResultFlag[1])
                {
                    //Result Of Inspection Item "三芯線色差判別" Is Error.
                    case -1:
                        Dispatcher.Invoke(() => Border_F_Item02_PartResult.Background = Brushes.Red);
                        Dispatcher.Invoke(() => Label_F_Item02_PartResult.Content = "Area : " + StatusText[2]);
                        break;

                    //Result Of Inspection Item "三芯線色差判別" Is Null.
                    case 0:
                        Dispatcher.Invoke(() => Border_F_Item02_PartResult.Background = Brushes.Green);
                        Dispatcher.Invoke(() => Label_F_Item02_PartResult.Content = "Area : " + StatusText[4]);
                        break;

                    //Result Of Inspection Item "三芯線色差判別" Is NG.
                    case 1:
                        Dispatcher.Invoke(() => Border_F_Item02_PartResult.Background = Brushes.Red);
                        Dispatcher.Invoke(() => Label_F_Item02_PartResult.Content = "Area : " + FirstStationData.Item02_AreaRate.ToString("F2") + "%");
                        break;

                    //Result Of Inspection Item "三芯線色差判別" Is OK.
                    case 2:
                        Dispatcher.Invoke(() => Border_F_Item02_PartResult.Background = Brushes.Green);
                        Dispatcher.Invoke(() => Label_F_Item02_PartResult.Content = "Area : " + FirstStationData.Item02_AreaRate.ToString("F2") + "%");
                        break;

                    default:
                        Dispatcher.Invoke(() => Border_F_Item02_PartResult.Background = Brushes.Red);
                        Dispatcher.Invoke(() => Label_F_Item02_PartResult.Content = "Area : " + StatusText[2]);
                        WriteLogOnUI("Error", "K1影像處理於<三芯線色差判別>時發生異常!");
                        break;
                }
            }

            //Print Out 1st Result For Check.
            if (PrintFlag[1])
            {
                Console.WriteLine("-------------------------第1站檢測結果(如下)-------------------------");
                Console.WriteLine("日期時間 : " + FirstStationData.PresentDateTime);
                Console.WriteLine("數值說明 : [true]為有檢測，[false]為不檢測");
                Console.WriteLine("數值說明 : -1為例外狀況，0為初始值，1為NG，2為OK");
                Console.WriteLine("[" + FirstStationData.InspectionItemFlag[0] + "]" + "三芯線轉線角度判別 : " + FirstStationData.InspectionResultFlag[0]);
                Console.WriteLine("[" + FirstStationData.InspectionItemFlag[1] + "]" + "三芯線色差判別 : " + FirstStationData.InspectionResultFlag[1]);
                Console.WriteLine("[" + FirstStationData.InspectionItemFlag[2] + "]" + "三芯線夾取位置判別 : " + FirstStationData.InspectionResultFlag[2]);
                Console.WriteLine("[" + FirstStationData.InspectionItemFlag[3] + "]" + "剝外被邊緣品質判別 : " + FirstStationData.InspectionResultFlag[3]);
                Console.WriteLine("-------------------------第1站檢測結果(如上)-------------------------" + "\n");
            }

            //Watch Stop.
            //TestWatch.Stop();
            //UseTime("RunTime In ResultReturn : ", TestWatch.Elapsed); PrintFlag[0] = false;
        }
        #endregion

        #region "Function"
        private void InitializeAOI()
        {
            try
            {
                if (true)
                {
                    IsInitializationFlag = true;
                    Console.WriteLine("====================================================================================================");
                    Console.WriteLine("====================================================================================================");
                    Console.WriteLine("====================================================================================================");
                }

                // System Initialization Or Setting
                // --------------------------------------------------
                ShowWindow(Hwnd, SW_HIDE);
                // --------------------------------------------------

                // IP Initialization Or Setting
                // --------------------------------------------------
                FirstStationData = new D_FirstStation();
                FirstStationData.ResultHandlerEvent += F_ResultReturn;
                FirstStationData.LogHandlerEvent += WriteLogOnUI;

                IP_1st = new Thread(FirstStationData.DoInspection);
                IP_1st.Start();
                // --------------------------------------------------

                // UI Initialization Or Setting
                // --------------------------------------------------
                StatusSign[0] = new BitmapImage();
                StatusSign[0].BeginInit();
                StatusSign[0].UriSource = new Uri(CurrentDirectory + @"\Appendix\Icon\GreenSign.ico", UriKind.RelativeOrAbsolute);
                StatusSign[0].EndInit();
                StatusSign[1] = new BitmapImage();
                StatusSign[1].BeginInit();
                StatusSign[1].UriSource = new Uri(CurrentDirectory + @"\Appendix\Icon\RedSign.ico", UriKind.RelativeOrAbsolute);
                StatusSign[1].EndInit();

                Grid_FirstStation.IsEnabled = true;

                IPDataUpdate();
                // --------------------------------------------------

                // Debug Code Initialization Or Setting
                // --------------------------------------------------

                // --------------------------------------------------

                if (true)
                {
                    IsInitializationFlag = false;
                    Console.WriteLine("====================================================================================================");
                    Console.WriteLine("====================================================================================================");
                    Console.WriteLine("====================================================================================================" + "\n");
                }
            }
            catch (Exception EX)
            {
                Console.WriteLine(EX.Message);
            }
        }

        private void Restart()
        {
            Close();

            System.Threading.Thread thtmp = new System.Threading.Thread(new
            System.Threading.ParameterizedThreadStart(ReRun));

            object appName = System.Windows.Forms.Application.ExecutablePath;
            System.Threading.Thread.Sleep(2000);
            thtmp.Start(appName);
        }

        private void ReRun(Object obj)
        {
            System.Diagnostics.Process ps = new System.Diagnostics.Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
        }

        public void UseTime(string title, TimeSpan ts)
        {
            if (PrintFlag[0])
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                Console.WriteLine(title + elapsedTime);
            }
        }

        private string TimeFormat(int mode, DateTime datetime)
        {
            string date;
            string time;
            string str;

            switch (mode)
            {
                case 1:
                    date = datetime.ToShortDateString();
                    time = datetime.ToString("hh:mm:ss.FFF", CI).PadRight(12, '0');
                    str = " [ " + date + " " + time + " ] : ";
                    return str;

                case 2:
                    str = datetime.Year.ToString() + "." + datetime.Month.ToString() + "." + datetime.Day.ToString() + "," + datetime.Hour.ToString() + "." + datetime.Minute.ToString() + "." + datetime.Second.ToString();
                    return str;

                case 3:
                    date = datetime.ToShortDateString();
                    time = datetime.ToString("hh:mm:ss.FFF", System.Globalization.CultureInfo.InvariantCulture).PadRight(12, '0');
                    str = date + " " + time;
                    return str;

                default:
                    date = datetime.ToShortDateString();
                    time = datetime.ToString("hh:mm:ss.FFF", System.Globalization.CultureInfo.InvariantCulture).PadRight(12, '0');
                    str = " [ " + date + " " + time + " ] : ";
                    return str;
            }
        }

        public void WriteLogOnUI(string m_Mode, string m_Word)
        {
            string m_DateTime = TimeFormat(1, DateTime.Now);

            switch (m_Mode)
            {
                case "General":
                    Dispatcher.Invoke(() => RichTextBox_GeneralLog.Document.Blocks.Add(new Paragraph(new Run(m_DateTime + m_Word))));
                    Dispatcher.Invoke(() => RichTextBox_GeneralLog.ScrollToEnd());
                    break;

                case "Warning":
                    Dispatcher.Invoke(() => RichTextBox_WarningLog.Document.Blocks.Add(new Paragraph(new Run(m_DateTime + m_Word))));
                    Dispatcher.Invoke(() => RichTextBox_WarningLog.ScrollToEnd());
                    break;

                case "Error":
                    Dispatcher.Invoke(() => RichTextBox_ErrorLog.Document.Blocks.Add(new Paragraph(new Run(m_DateTime + m_Word))));
                    Dispatcher.Invoke(() => RichTextBox_ErrorLog.ScrollToEnd());
                    break;

                default:
                    Dispatcher.Invoke(() => RichTextBox_WarningLog.Document.Blocks.Add(new Paragraph(new Run(m_DateTime + "Input Value Was Not Suitable When Log Was Writing."))));
                    Dispatcher.Invoke(() => RichTextBox_WarningLog.ScrollToEnd());
                    break;
            }
        }

        private void RadioButtonBeFalse(int number)
        {
            switch (number)
            {
                case 1:
                    RadioButton_F_Item01_ROI01.IsChecked = false;
                    RadioButton_F_Item01_Result01.IsChecked = false;
                    RadioButton_F_Item02_ROI.IsChecked = false;
                    RadioButton_F_Item02_Result.IsChecked = false;
                    break;
                default:
                    break;
            }
        }

        private void ImageBoxPaintFlagBeFalse()
        {
            ROIChooseFlag = false;
            Selecting = false;
            SelectROI = false;
            for (int i = 0; i < SelectCenter.Length; i++)
                SelectCenter[i] = false;
        }

        private void IPDataUpdate()
        {
            //First Station In UI. 
            // --------------------------------------------------
            /*NumericUpDown_F_Item01_ROILength.Value = Convert.ToDecimal(FirstStationData.IP_Config.parameters.Parameter01[11]) / Convert.ToDecimal(1000);//Convert "um" To "mm".
            NumericUpDown_F_Item01_PassAngle01.Value = FirstStationData.IP_Config.parameters.Parameter01[9];
            NumericUpDown_F_Item01_PassAngle02.Value = FirstStationData.IP_Config.parameters.Parameter01[10];
            NumericUpDown_F_Item02_Parameter0101.Value = FirstStationData.IP_Config.parameters.Parameter01[0];
            NumericUpDown_F_Item02_Parameter0102.Value = FirstStationData.IP_Config.parameters.Parameter01[1];
            NumericUpDown_F_Item02_Parameter0103.Value = FirstStationData.IP_Config.parameters.Parameter01[2];
            NumericUpDown_F_Item02_Parameter0201.Value = FirstStationData.IP_Config.parameters.Parameter01[3];
            NumericUpDown_F_Item02_Parameter0202.Value = FirstStationData.IP_Config.parameters.Parameter01[4];
            NumericUpDown_F_Item02_Parameter0203.Value = FirstStationData.IP_Config.parameters.Parameter01[5];
            NumericUpDown_F_Item02_Parameter0301.Value = FirstStationData.IP_Config.parameters.Parameter01[6];
            NumericUpDown_F_Item02_Parameter0302.Value = FirstStationData.IP_Config.parameters.Parameter01[7];
            NumericUpDown_F_Item02_Parameter0303.Value = FirstStationData.IP_Config.parameters.Parameter01[8];*/
          
            // --------------------------------------------------
        }
        #endregion

        #region "UI"
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IP_1st.Abort();

            ShowWindow(Hwnd, SW_HIDE);
        }

        private void TabControl_Station_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitializationFlag)
            {
                
            }
        }

        private void ImageBox_FirstStation_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (ImageBox_FirstStation.Image == null)
                {
                    return;
                }

                if (Selecting && IsMouseDown)
                {
                    if (Selecting && IsMouseDown && SelectROI)
                    {
                        int x = Math.Min(StartPoint_PictureBox.X, e.X);
                        int y = Math.Min(StartPoint_PictureBox.Y, e.Y);
                        int width = Math.Max(StartPoint_PictureBox.X, e.X) - Math.Min(StartPoint_PictureBox.X, e.X);
                        int height = Math.Max(StartPoint_PictureBox.Y, e.Y) - Math.Min(StartPoint_PictureBox.Y, e.Y);

                        rectPictureBox = new System.Drawing.Rectangle(x, y, width, height);
                        ImageBox_FirstStation.Refresh();
                    }

                    if (Selecting && IsMouseDown && SelectCenter[0])
                    {
                        CenterPoint_PictureBox = new System.Drawing.Point(e.X, e.Y);
                        ImageBox_FirstStation.Refresh();
                    }

                    if (Selecting && IsMouseDown && SelectCenter[1])
                    {
                        CenterPoint_PictureBox = new System.Drawing.Point(e.X, e.Y);
                        ImageBox_FirstStation.Refresh();
                    }
                }

                if (IsShowPixelValue)
                {
                    ImageBox imageBox = sender as ImageBox;

                    System.Drawing.Size imgSize = (imageBox.Image as Image<Bgr, byte>).Size;

                    // Calulate Camera Image Coordanite
                    int imageCoorX = (int)((e.Location.X) * (float)imgSize.Width / (imageBox.Width - 1));
                    int imageCoorY = (int)((e.Location.Y) * (float)imgSize.Height / (imageBox.Height - 1));
                    int imageSize = (int)(imgSize.Width * imgSize.Height);

                    // When EmguCV Imagebox scrool bar is active
                    //imageCoorX += displayImage.HorizontalScrollBar.Visible ? (int)displayImage.HorizontalScrollBar.Value : 0;
                    //imageCoorY += displayImage.VerticalScrollBar.Visible ? (int)displayImage.VerticalScrollBar.Value : 0;

                    int index = imageCoorY * imgSize.Width + imageCoorX;
                    index = index < 0 ? 0 : index > imageSize - 1 ? imageSize - 1 : index;

                    byte pixel = ((Image<Bgr, byte>)(ImageBox_FirstStation.Image)).Bytes[index];

                    //State.Text = "Position: (" + Convert.ToString(imageCoorX) + ", " + Convert.ToString(imageCoorY) + ")\tDepth: " + pixel.ToString();
                    string state = "Position: (" + Convert.ToString(imageCoorX) + ", " + Convert.ToString(imageCoorY) + ")\tDepth: " + pixel.ToString();

                    //PictureBoxMouseMoveEvent.Invoke(state);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ImageBox_FirstStation_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selecting == true)
            {
                IsMouseDown = true;

                StartPoint_PictureBox = e.Location;
            }

            if (SelectROI)
            {
                IsMouseDown = true;

                ImageROI_StartPoint.X = (int)((e.Location.X) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                ImageROI_StartPoint.Y = (int)((e.Location.Y) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));
            }

            if (SelectCenter[0])
            {
                IsMouseDown = true;

                //FirstPictureBox_ROILength = Convert.ToInt32((Convert.ToDouble(FirstStationData.IP_Config.parameters.Parameter01[11]) / FirstStationData.Resolution) * Convert.ToDouble((sender as ImageBox).Width - 1) / Convert.ToDouble(CameraImageWidth));

                CenterPoint_PictureBox = new System.Drawing.Point(e.X, e.Y);

                ImageBox_FirstStation.Refresh();
            }

            if (SelectCenter[1])
            {
                IsMouseDown = true;

                CenterPoint_PictureBox = new System.Drawing.Point(e.X, e.Y);

                ImageBox_FirstStation.Refresh();
            }
        }

        private void ImageBox_FirstStation_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selecting)
            {
                Selecting = false;
                IsMouseDown = false;
                EndPoint_PictureBox = e.Location;
            }

            if (SelectROI)
            {
                SelectROI = false;
                IsMouseDown = false;

                // Calulate Camera Image Coordanite
                ImageROI_EndPoint.X = (int)((e.Location.X) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                ImageROI_EndPoint.Y = (int)((e.Location.Y) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));

                int x = Math.Min(ImageROI_StartPoint.X, ImageROI_EndPoint.X);
                int y = Math.Min(ImageROI_StartPoint.Y, ImageROI_EndPoint.Y);
                int width = Math.Abs(ImageROI_StartPoint.X - ImageROI_EndPoint.X);
                int height = Math.Abs(ImageROI_StartPoint.Y - ImageROI_EndPoint.Y);

                rectImageROI = new System.Drawing.Rectangle(x, y, width, height);

                if (rectImageROI.X >= 0 && rectImageROI.Y >= 0 && (rectImageROI.X + rectImageROI.Width) <= CameraImageWidth && (rectImageROI.Y + rectImageROI.Height) <= CameraImageHeight)
                {
                    switch (ROINumber)
                    {
                        case 100:
                            //FirstStationData.IP_Config.parameters.ROI01 = rectImageROI;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    WriteLogOnUI("Warning", "K1檢測站的ROI設定失敗!");
                }
            }

            if (SelectCenter[0])
            {
                SelectCenter[0] = false;
                IsMouseDown = false;

                Image_CenterPoint.X = (int)((e.Location.X) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                Image_CenterPoint.Y = (int)((e.Location.Y) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));

                int x = (int)((e.Location.X - 6) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                int y = (int)((e.Location.Y - 120) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));
                int width = (int)((12) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                int height = (int)((240) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));

                rectImageROI = new System.Drawing.Rectangle(x, y, width, height);

                if (rectImageROI.X >= 0 && rectImageROI.Y >= 0 && (rectImageROI.X + rectImageROI.Width) <= CameraImageWidth && (rectImageROI.Y + rectImageROI.Height) <= CameraImageHeight)
                {
                    switch (CenterNumber)
                    {
                        case 100:
                            //FirstStationData.IP_Config.parameters.ROI01 = rectImageROI;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    WriteLogOnUI("Warning", "K1檢測站的ROI設定失敗!");
                }
            }

            if (SelectCenter[1])
            {
                SelectCenter[1] = false;
                IsMouseDown = false;

                Image_CenterPoint.X = (int)((e.Location.X) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                Image_CenterPoint.Y = (int)((e.Location.Y) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));

                int x = (int)((e.Location.X - 60) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                int y = (int)((e.Location.Y - 60) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));
                int width = (int)((120) * (float)CameraImageWidth / ((sender as ImageBox).Width - 1));
                int height = (int)((120) * (float)CameraImageHeight / ((sender as ImageBox).Height - 1));

                rectImageROI = new System.Drawing.Rectangle(x, y, width, height);

                if (rectImageROI.X >= 0 && rectImageROI.Y >= 0 && (rectImageROI.X + rectImageROI.Width) <= CameraImageWidth && (rectImageROI.Y + rectImageROI.Height) <= CameraImageHeight)
                {
                    switch (CenterNumber)
                    {
                        case 190:
                            FirstStationData.RectAdjust = rectImageROI;
                            ROIChooseFlag = false;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    WriteLogOnUI("Warning", "K1檢測站的ROI設定失敗!");
                }
            }
        }

        private void ImageBox_FirstStation_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (IsMouseDown)
            {
                if (SelectROI)
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, rectPictureBox);
                    }
                }

                if (SelectCenter[0])
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(CenterPoint_PictureBox.X - 6, CenterPoint_PictureBox.Y - 120, 12, 240));
                        e.Graphics.DrawLine(pen, CenterPoint_PictureBox.X + 6 + FirstPictureBox_ROILength, CenterPoint_PictureBox.Y - 120, CenterPoint_PictureBox.X + 6 + FirstPictureBox_ROILength, CenterPoint_PictureBox.Y + 120);
                    }
                }

                if (SelectCenter[1])
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(CenterPoint_PictureBox.X - 60, CenterPoint_PictureBox.Y - 60, 120, 120));
                    }
                }
            }
        }

        private void Btn_F_Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                RadioButtonBeFalse(1);

                ImageSource[0] = 0;

                FirstStationData.SetImage(0, new Image<Bgr, byte>(openFileDialog.FileName));
                FirstStationData.SetImage(1, FirstStationData.GetImage(2));

                ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);

                WriteLogOnUI("General", "完成圖檔開啟!");
            }
            else
            {
                WriteLogOnUI("General", "未完成圖檔開啟!");
            }
        }

        private void Btn_F_Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog.Filter = ".bmp|*.bmp";

            if (saveFileDialog.ShowDialog() == true && ImageSource[0] >= 0)
            {
                FirstStationData.GetImage(ImageSource[0]).Save(saveFileDialog.FileName);

                WriteLogOnUI("General", "完成圖檔儲存!");
            }
            else
            {
                WriteLogOnUI("General", "未完成圖檔儲存!");
            }
        }

        private void Btn_F_Grab_Click(object sender, RoutedEventArgs e)
        {
            #region "Edition 01"
            if (false)
            {
                ImageSource[0] = 0;

                //MyCamera.Camera_Grab(0, 1);
                //FirstStationData.SetImage(0, MyCamera.components[0].GrabImage);

                //MyCameraView[0].ImageBoxDisplay.Image = FirstStationData.GetImage(ImageSource[0]);
                //ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
            }
            #endregion

            #region "Edition 02"
            if (true)
            {
                RadioButtonBeFalse(1);

                //MyWatch01.Reset();
                //MyWatch01.Start();

                /*if (MyCamera.IsFinished[0])
                {
                    MyCamera.IsConverting[0] = true;
                }
                else
                {
                    WriteLogOnUI("General", "K1檢測站的攝影機線執行緒正在使用中!");
                }*/
            }
            #endregion
        }

        private void Btn_F_Origin_Click(object sender, RoutedEventArgs e)
        {
            ImageSource[0] = 0;

            ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);

            RadioButtonBeFalse(1);
        }

        private void Btn_F_ROI_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonBeFalse(1);
            ImageBoxPaintFlagBeFalse();

            switch ((sender as System.Windows.Controls.Button).Name)
            {
                case "Btn_F_Item01_ROI":
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(0);

                    CenterNumber = 100;
                    SelectCenter[0] = true;
                    Selecting = true;

                    break;

                case "Btn_F_Adjustment":
                    ROIChooseFlag = true;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(0);

                    CenterNumber = 190;
                    SelectCenter[1] = true;
                    Selecting = true;

                    break;

                default:
                    WriteLogOnUI("Error", "選取ROI時發生異常!");
                    break;
            }
        }

        private void Btn_F_IP_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as System.Windows.Controls.Button).Name)
            {
                //IP Of Item "1".
                case "Btn_F_Item01_IP":
                    FirstStationData.InspectionItemFlag[0] = true;
                    FirstStationData.InspectionItemFlag[1] = false;
                    FirstStationData.InspectionItemFlag[2] = false;
                    FirstStationData.InspectionItemFlag[3] = false;

                    RadioButtonBeFalse(1);

                    //Do Image Processing
                    if (FirstStationData.DoInspectionFlag)
                    {
                        WriteLogOnUI("Warning", "K1影像處理線執行緒正在使用中!");
                    }
                    else
                    {
                        FirstStationData.Barcode = "Manual";
                        FirstStationData.TriggerCount = 1;
                        FirstStationData.SerialNumber = 1;
                        FirstStationData.DoInspectionFlag = true;
                    }

                    break;

                //IP Of Item "2".
                case "Btn_F_Item02_IP":
                    FirstStationData.InspectionItemFlag[0] = false;
                    FirstStationData.InspectionItemFlag[1] = true;
                    FirstStationData.InspectionItemFlag[2] = false;
                    FirstStationData.InspectionItemFlag[3] = false;

                    RadioButtonBeFalse(1);

                    //Do Image Processing
                    if (FirstStationData.DoInspectionFlag)
                    {
                        WriteLogOnUI("Warning", "K1影像處理線執行緒正在使用中!");
                    }
                    else
                    {
                        FirstStationData.Barcode = "Manual";
                        FirstStationData.TriggerCount = 1;
                        FirstStationData.SerialNumber = 1;
                        FirstStationData.DoInspectionFlag = true;
                    }

                    break;

                //IP Of Item "2".
                case "Btn_F_Item03_IP":
                    FirstStationData.InspectionItemFlag[0] = false;
                    FirstStationData.InspectionItemFlag[1] = false;
                    FirstStationData.InspectionItemFlag[2] = true;
                    FirstStationData.InspectionItemFlag[3] = false;

                    RadioButtonBeFalse(1);

                    //Do Image Processing
                    if (FirstStationData.DoInspectionFlag)
                    {
                        WriteLogOnUI("Warning", "K1影像處理線執行緒正在使用中!");
                    }
                    else
                    {
                        FirstStationData.Barcode = "Manual";
                        FirstStationData.TriggerCount = 1;
                        FirstStationData.SerialNumber = 1;
                        FirstStationData.DoInspectionFlag = true;
                    }

                    break;

                default:
                    break;
            }
        }


        private void RadioButton_FirstStation_Checked(object sender, RoutedEventArgs e)
        {
            switch ((sender as System.Windows.Controls.RadioButton).Name)
            {
                case "RadioButton_F_Item01_ROI01":
                    ImageSource[0] = 10;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item01_ROI02":
                    ImageSource[0] = 10;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item01_Result01":
                    ImageSource[0] = 1;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item01_Result02":
                    ImageSource[0] = 1;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item02_ROI":
                    ImageSource[0] = 10;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item02_Result":
                    ImageSource[0] = 1;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item03_ROI":
                    ImageSource[0] = 10;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                case "RadioButton_F_Item03_Result":
                    ImageSource[0] = 1;
                    ImageBox_FirstStation.Image = FirstStationData.GetImage(ImageSource[0]);
                    break;

                default:
                    break;
            }
        }

        private void NumericUpDown_F_Parameter_ValueChanged(object sender, EventArgs e)
        {

        }

        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ShowWindow(Hwnd, SW_SHOW);
            }

            if (e.Key == Key.H && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ShowWindow(Hwnd, SW_HIDE);
            }
        }
        #endregion
    }
}
