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
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using ImageProcessing;
using Config;
using LOGRECORDER;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        #region "Parameter"
        // Initialization Parameter
        // --------------------------------------------------
        public bool InitializationFlag = false;
        // --------------------------------------------------

        // Picture Box Selection Parameter
        // --------------------------------------------------
        public bool IsMouseDown = false;

        public bool Selecting = false;
        public System.Drawing.Rectangle rectPictureBox = System.Drawing.Rectangle.Empty;
        public System.Drawing.Point StartPoint_PictureBox;
        public System.Drawing.Point EndPoint_PictureBox;
        public System.Drawing.Point CenterPoint_PictureBox;

        public bool SelectFlexibleROI = false;
        public bool SelectFixedROI = false;
        public System.Drawing.Rectangle rectImageROI;
        public System.Drawing.Point ImageROI_StartPoint;
        public System.Drawing.Point ImageROI_EndPoint;
        public System.Drawing.Point ImageROI_CenterPoint;
        // --------------------------------------------------

        // Other Parameter
        // --------------------------------------------------
        public System.String CurrentDirectory = System.Environment.CurrentDirectory;
        private static readonly System.Globalization.CultureInfo CI = System.Globalization.CultureInfo.InvariantCulture;
        // --------------------------------------------------

        // Debug Parameter
        // --------------------------------------------------
        private static Thread DebugThread;
        public bool[] DebugFlag = new bool[5] { true, true, false, false, false };
        public Stopwatch[] DebugWatch = new Stopwatch[5] { new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch() };
        // --------------------------------------------------
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region "Event"
        private void Filter_Event(float[,] kernel, bool temp)
        {
            try
            {
                if (temp)
                {
                    ImageBox_Main.Image = BasicAlgorithm.ToBGR(BasicAlgorithm.ApplyCustomFilter(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)), kernel));
                }
                else
                {
                    Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Filter Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ApplyCustomFilter(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)), kernel)));
                    DoLog("完成濾波!", true, true, "General"); // todo
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message); // todo
            }
        }

        private void Buffer_Event(int index)
        {
            try
            {
                ImageBox_Main.Image = Buffer_x12.GetImage(index);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message); // todo
            }
        }
        #endregion

        #region "Function"
        private void InitializeUI()
        {
            try
            {
                // 暫時紀錄代辦任務，完成後即可刪除
                Console.WriteLine("使用config紀錄UI參數-如3*3filter設定，像是影像處裡或是UI");
                Console.WriteLine("使用log，但是gitignore得忽視存下的檔案");
                Console.WriteLine("美化英文UI");
                Console.WriteLine("完成基本英文註解");
                Console.WriteLine("完成todo");
                Console.WriteLine("清除不必要檔案");

                EnsureDirectoriesExist();

                Slide_Binarization.Value = 100;
                Filter_3x3.FilterHandleEvent += Filter_Event;
                Buffer_x12.BufferHandleEvent += Buffer_Event;

                InitializationFlag = true;
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

        /// <summary>
        /// 檢查並創建必要的目錄結構
        /// </summary>
        public void EnsureDirectoriesExist()
        {
            string currentDirectory = CurrentDirectory;
            string appendixPath = System.IO.Path.Combine(currentDirectory, "Appendix");
            string configPath = System.IO.Path.Combine(appendixPath, "Config");
            string logPath = System.IO.Path.Combine(appendixPath, "Log");

            // 檢查並創建 Appendix 資料夾
            if (!System.IO.Directory.Exists(appendixPath))
            {
                System.IO.Directory.CreateDirectory(appendixPath);
                System.Console.WriteLine("已創建 Appendix 資料夾");
            }

            // 檢查並創建 Config 資料夾
            if (!System.IO.Directory.Exists(configPath))
            {
                System.IO.Directory.CreateDirectory(configPath);
                System.Console.WriteLine("已創建 Config 資料夾");
            }

            // 檢查並創建 Log 資料夾
            if (!System.IO.Directory.Exists(logPath))
            {
                System.IO.Directory.CreateDirectory(logPath);
                System.Console.WriteLine("已創建 Log 資料夾");
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

        public void DoLog(string message, bool isPrintOnRichTextBox, bool isPrintOnConsole, string mode = "General" )
        {
            // todo

            string m_DateTime = TimeFormat(1, DateTime.Now);

            message = TimeFormat(1, DateTime.Now) + message;

            if (isPrintOnRichTextBox)
            {
                Dispatcher.Invoke(() => RichTextBox_GeneralLog.Document.Blocks.Add(new Paragraph(new Run(message))));
                Dispatcher.Invoke(() => RichTextBox_GeneralLog.ScrollToEnd());
            }

            if (isPrintOnConsole)
            {
                Console.WriteLine(message);
            }

                switch (mode)
            {
                case "General":
                    break;

                case "Warning":
                    break;

                case "Error":
                    break;

                default:
                    break;
            }

            
        }
#endregion

                #region "UI"
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void ImageBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (ImageBox_Main.Image == null)
                {
                    return;
                }

                if (Selecting && IsMouseDown)
                {
                    if (Selecting && IsMouseDown && SelectFlexibleROI)
                    {
                        int x = Math.Min(StartPoint_PictureBox.X, e.X);
                        int y = Math.Min(StartPoint_PictureBox.Y, e.Y);
                        int width = Math.Max(StartPoint_PictureBox.X, e.X) - Math.Min(StartPoint_PictureBox.X, e.X);
                        int height = Math.Max(StartPoint_PictureBox.Y, e.Y) - Math.Min(StartPoint_PictureBox.Y, e.Y);

                        rectPictureBox = new System.Drawing.Rectangle(x, y, width, height);
                        ImageBox_Main.Refresh();
                    }

                    if (Selecting && IsMouseDown && SelectFixedROI)
                    {
                        CenterPoint_PictureBox = new System.Drawing.Point(e.X, e.Y);
                        ImageBox_Main.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ImageBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selecting == true)
            {
                IsMouseDown = true;

                StartPoint_PictureBox = e.Location;
            }

            if (SelectFlexibleROI)
            {
                ImageROI_StartPoint.X = (int)((e.Location.X) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                ImageROI_StartPoint.Y = (int)((e.Location.Y) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));
            }

            if (SelectFixedROI)
            {
                CenterPoint_PictureBox = new System.Drawing.Point(e.X, e.Y);

                ImageBox_Main.Refresh();
            }
        }

        private void ImageBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Selecting)
            {
                Selecting = false;
                IsMouseDown = false;
                EndPoint_PictureBox = e.Location;
            }

            if (SelectFlexibleROI)
            {
                SelectFlexibleROI = false;
                IsMouseDown = false;

                ImageROI_EndPoint.X = (int)((e.Location.X) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                ImageROI_EndPoint.Y = (int)((e.Location.Y) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));

                int x = Math.Min(ImageROI_StartPoint.X, ImageROI_EndPoint.X);
                int y = Math.Min(ImageROI_StartPoint.Y, ImageROI_EndPoint.Y);
                int width = Math.Abs(ImageROI_StartPoint.X - ImageROI_EndPoint.X);
                int height = Math.Abs(ImageROI_StartPoint.Y - ImageROI_EndPoint.Y);

                rectImageROI = new System.Drawing.Rectangle(x, y, width, height);
                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "ROI Image", BasicAlgorithm.GetROI(Buffer_x12.GetImage(Buffer_x12.CurrentIndex), rectImageROI));
            }

            if (SelectFixedROI)
            {
                SelectFixedROI = false;
                IsMouseDown = false;

                ImageROI_CenterPoint.X = (int)((e.Location.X) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                ImageROI_CenterPoint.Y = (int)((e.Location.Y) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));

                int x = (int)((e.Location.X - 60) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                int y = (int)((e.Location.Y - 60) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));
                int width = (int)((120) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                int height = (int)((120) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));

                rectImageROI = new System.Drawing.Rectangle(x, y, width, height);
                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "ROI Image", BasicAlgorithm.GetROI(Buffer_x12.GetImage(Buffer_x12.CurrentIndex), rectImageROI));
            }
        }

        private void ImageBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (IsMouseDown)
            {
                if (SelectFlexibleROI)
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, rectPictureBox);
                    }
                }

                if (SelectFixedROI)
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(CenterPoint_PictureBox.X - 60, CenterPoint_PictureBox.Y - 60, 120, 120));
                    }
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InitializationFlag)
                {
                    switch ((sender as System.Windows.Controls.Button).Name)
                    {
                        case "Btn_Open":
                            {
                                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

                                openFileDialog.Multiselect = false;
                                openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*";

                                if (openFileDialog.ShowDialog() == true)
                                {
                                    if (Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Source Image", new Image<Bgr, byte>(openFileDialog.FileName)))
                                    {
                                        DoLog("完成圖檔開啟!", true, true, "General"); // todo
                                    }
                                    else
                                    {
                                        DoLog("Buffer已滿", true, true, "General"); // todo
                                    }
                                }
                                else
                                {
                                    DoLog("未完成圖檔開啟!", true, true, "General"); // todo
                                }
                            }
                            break;

                        case "Btn_Save":
                            {
                                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                                saveFileDialog.Filter = ".bmp|*.bmp";

                                if (saveFileDialog.ShowDialog() == true && Buffer_x12.CurrentIndex >= 0 && Buffer_x12.CurrentIndex < Buffer_x12.VisibleCount)
                                {
                                    Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Save(saveFileDialog.FileName);
                                    DoLog("完成圖檔儲存!", true, true, "General"); // todo
                                }
                                else
                                {
                                    DoLog("未完成圖檔儲存!", true, true, "General"); // todo
                                }
                            }
                            break;

                        case "Btn_Copy":
                            {
                                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Copy Image", Buffer_x12.GetImage(Buffer_x12.CurrentIndex));
                                DoLog("完成灰階!", true, true, "General"); // todo
                            }
                            break;

                        case "Btn_Delete":
                            {
                                Buffer_x12.RemoveImage(Buffer_x12.CurrentIndex);
                                Buffer_x12.ResetAllRadioButtons();
                                ImageBox_Main.Image = Buffer_x12.GetImage(Buffer_x12.CurrentIndex);
                                DoLog("Delete", true, true, "General"); // todo
                            }
                            break;

                        case "Btn_Gray":
                            {
                                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Grayscale Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex))));
                                DoLog("完成灰階!", true, true, "General"); // todo
                            }
                            break;

                        case "Btn_Dilation":
                            {
                                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Dilated Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToDilation(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)))));
                                DoLog("完成膨脹!", true, true, "General"); // todo
                            }
                            break;

                        case "Btn_Erosion":
                            {
                                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Eroded Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToErosion(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)))));
                                DoLog("完成侵蝕!", true, true, "General"); // todo
                            }
                            break;

                        case "Btn_ROI":
                            {
                                if (0 == 0)
                                    SelectFlexibleROI = true; // ROI Option 1
                                else
                                    SelectFixedROI = true; // ROI Option 2
                                Selecting = true;
                            }
                            break;

                        case "Btn_Reset":
                            {
                                Restart();
                            }
                            break;

                        case "test1":
                            {
                                // todo
                            }
                            break;

                        case "test2":
                            {
                                // todo
                            }
                            break;

                        default:
                            {

                            }
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception Occurred When Button Clicked. Message:" + Ex.Message);
            }
        }

        private void Slide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (InitializationFlag)
                {
                    switch ((sender as System.Windows.Controls.Slider).Name)
                    {
                        case "Slide_Binarization":
                            {
                                ImageBox_Main.Image = BasicAlgorithm.ToBGR(BasicAlgorithm.ToBinarization(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)), new Gray(Convert.ToInt32(e.NewValue))));
                            }
                            break;

                        default:
                            {

                            }
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {

            }
        }

        private void Slide_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (InitializationFlag)
                {
                    switch ((sender as System.Windows.Controls.Slider).Name)
                    {
                        case "Slide_Binarization":
                            {
                                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Binary Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToBinarization(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)), new Gray(Convert.ToInt32((sender as System.Windows.Controls.Slider).Value)))));
                                DoLog("完成二質化!", true, true, "General"); // todo
                            }
                            break;

                        default:
                            {

                            }
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {

            }
        }

        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {

            }

            if (e.Key == Key.H && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {

            }
        }
                #endregion
    }
}
