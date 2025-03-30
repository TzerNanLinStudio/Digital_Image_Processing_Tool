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
        #region "Enum"
        public enum LogLevel
        {
            General,
            Warning,
            Error,
            Debug,
            None,
        }
        #endregion

        #region "Parameter"
        // Common Parameter
        // --------------------------------------------------
        private const int RoiMode = 1;
        private readonly System.String _currentDirectory = System.Environment.CurrentDirectory;
        private bool _isInitialized = false;
        // --------------------------------------------------

        // PictureBox Selection Parameter
        // --------------------------------------------------
        private bool _isMouseDown = false;
        private bool _isSelecting = false;
        private bool _isFlexibleRoiSelected = false;
        private bool _isFixedRoiSelected = false;

        private System.Drawing.Rectangle _pictureBoxRectangle = System.Drawing.Rectangle.Empty;
        private System.Drawing.Point _pictureBoxStartPoin;
        private System.Drawing.Point _pictureBoxEndPoint;
        private System.Drawing.Point _pictureBoxCenterPoint;

        private System.Drawing.Rectangle _imageRoiRectangle;
        private System.Drawing.Point _imageRoiStartPoint;
        private System.Drawing.Point _imageRoiEndPoint;
        private System.Drawing.Point _imageRoiCenterPoint;
        // --------------------------------------------------

        // Config Parameter
        // --------------------------------------------------
        public CustomConfig_UI UiConfig = new CustomConfig_UI();
        private string _recipeDirectoryPath = System.Environment.CurrentDirectory + "/Appendix/Config/";
        private string _recipeFileName = "UI";
        private string _recipeFileExtension = ".dat";
        private string _recipeFullPath = "";
        // --------------------------------------------------

        // LogRecorder Parameter
        // --------------------------------------------------
        public InfoMgr LogWritter;
        private string _logFileDirectoryPath = System.Environment.CurrentDirectory + "/Appendix/Log/";
        private string _logFileNamePrefix = "UI";
        // --------------------------------------------------

        // Debug Parameter
        // --------------------------------------------------
        public static Thread DebugThread;
        public bool[] DebugFlags = new bool[5] { false, false, false, false, false };
        public Stopwatch[] DebugStopwatches = new Stopwatch[5] { new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch(), new Stopwatch() };
        // --------------------------------------------------
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            SetupUI();
        }

        #region "Event"
        private void Filter_Event(float[][] kernel, bool temp)
        {
            try
            {
                UiConfig.parameters.Kernel = kernel; // Update kernel parameters (shallow copy)

                var filteredImage = BasicAlgorithm.ApplyCustomFilter(
                                        BasicAlgorithm.ToGray(
                                            Buffer_x12.GetImage(
                                                Buffer_x12.CurrentIndex)), 
                                        UiConfig.parameters.Kernel);

                if (temp)
                {
                    ImageBox_Main.Image = BasicAlgorithm.ToBGR(filteredImage);
                    HandleMessage("Temporary filter applied and displayed on main image.", false, false, true, LogLevel.General);
                }
                else
                {
                    Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Filtered Image", BasicAlgorithm.ToBGR(filteredImage));
                    HandleMessage("Filtered image successfully added to buffer.", false, true, true, LogLevel.General);
                }
            }
            catch (Exception ex)
            {
                HandleMessage("Error occurred while applying filter: " + ex.Message, false, true, true, LogLevel.Error);
            }
        }

        private void Buffer_Event(int index)
        {
            try
            {
                ImageBox_Main.Image = Buffer_x12.GetImage(index);
                HandleMessage($"Image at index {index} successfully loaded.", false, false, true, LogLevel.General);
            }
            catch (Exception ex)
            {
                HandleMessage($"Failed to load image from buffer: {ex.Message}", false, true, true, LogLevel.Error);
            }
        }
        #endregion

        #region "Function"
        /// <summary>
        /// Initializes the UI components, sets initial parameter values,
        /// binds event handlers, and prepares necessary resources such as log files and directories.
        /// </summary>
        private void SetupUI()
        {
            try
            {
                EnsureDirectoriesExist();
                InitializeConfig(true);
                InitializeLogFiles(_logFileDirectoryPath + _logFileNamePrefix);

                Slide_Binarization.Value = UiConfig.parameters.Threshold;
                for (int x = 0; x < Filter_3x3.StackPanel_NineSquare.Children.Count; x++)
                {
                    for (int y = 0; y < (Filter_3x3.StackPanel_NineSquare.Children[x] as StackPanel).Children.Count; y++)
                    {
                        ((Filter_3x3.StackPanel_NineSquare.Children[x] as StackPanel).Children[y] as Square).Text_Number.Text = UiConfig.parameters.Kernel[x][y].ToString();
                    }
                }

                Filter_3x3.FilterHandleEvent += Filter_Event;
                Buffer_x12.BufferHandleEvent += Buffer_Event;

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Restarts the application by closing the current instance and launching a new one.
        /// </summary>
        private void RestartApplication()
        {
            Close();

            System.Threading.Thread tmp = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(RunApplication));
            System.Threading.Thread.Sleep(2000);
            tmp.Start(System.Windows.Forms.Application.ExecutablePath);
        }

        /// <summary>
        /// Launches a new process of the application.
        /// </summary>
        /// <param name="obj">The executable path of the application.</param>
        private void RunApplication(Object obj)
        {
            System.Diagnostics.Process ps = new System.Diagnostics.Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
        }

        /// <summary>
        /// Ensures that necessary directories (Appendix, Config, Log) exist. If not, creates them.
        /// </summary>
        public void EnsureDirectoriesExist()
        {
            string currentDirectory = _currentDirectory;
            string appendixPath = System.IO.Path.Combine(currentDirectory, "Appendix");
            string configPath = System.IO.Path.Combine(appendixPath, "Config");
            string logPath = System.IO.Path.Combine(appendixPath, "Log");

            if (!System.IO.Directory.Exists(appendixPath))
            {
                System.IO.Directory.CreateDirectory(appendixPath);
            }

            if (!System.IO.Directory.Exists(configPath))
            {
                System.IO.Directory.CreateDirectory(configPath);
            }

            if (!System.IO.Directory.Exists(logPath))
            {
                System.IO.Directory.CreateDirectory(logPath);
            }
        }

        /// <summary>
        /// Initializes configuration data from file. Optionally loads values into the UI.
        /// </summary>
        /// <param name="isLoaded">If true, loads configuration into the UI after initialization.</param>
        public void InitializeConfig(bool isLoaded)
        {
            _recipeFullPath = _recipeDirectoryPath + _recipeFileName + _recipeFileExtension;

            UiConfig = new CustomConfig_UI(_recipeFullPath);

            if (isLoaded) LoadConfig();
        }

        /// <summary>
        /// Loads configuration values into the UI components.
        /// </summary>
        public void LoadConfig()
        {
            if (UiConfig.Load() == false) return;
        }
       
        /// <summary>
        /// Saves the current UI configuration to file.
        /// </summary>
        public void SaveConfig()
        {
            UiConfig.Save();
        }

        /// <summary>
        /// Initializes the log writer with paths for different types of logs.
        /// </summary>
        /// <param name="path">Base path for log files.</param>
        public void InitializeLogFiles(string path)
        {
            LogWritter = new InfoMgr(path + "./GeneralLog", path + "./WarningLog", path + "./ErrorLog", path + "./DebugLog");
        }

        /// <summary>
        /// Handles log messages with options to print to message box, rich text box, or console.
        /// </summary>
        /// <param name="message">Message content.</param>
        /// <param name="isShowedOnMessageBox">Whether to display on Windows message box.</param>
        /// <param name="isPrintOnRichTextBox">Whether to display on UI rich text box.</param>
        /// <param name="isPrintOnConsole">Whether to print to console.</param>
        /// <param name="mode">Log level: General, Warning, Error, or Debug.</param>
        public void HandleMessage(string message, bool isShowedOnMessageBox, bool isPrintOnRichTextBox, bool isPrintOnConsole, LogLevel mode = LogLevel.None)
        {
            switch (mode)
            {
                case LogLevel.General:
                    LogWritter.MsgGenLog(message);
                    break;

                case LogLevel.Warning:
                    LogWritter.MsgWarning(message);
                    break;

                case LogLevel.Error:
                    LogWritter.MsgError(message);
                    break;

                case LogLevel.Debug:
                    LogWritter.MsgDebug(message);
                    break;

                default:
                    break;
            }

            if (isShowedOnMessageBox)
            {
                System.Windows.MessageBox.Show(message);
            }

            message = $"[ {DateTime.Now:yyyy/M/d HH:mm:ss} ] " + message;

            if (isPrintOnRichTextBox)
            {
                Dispatcher.Invoke(() => RichTextBox_GeneralLog.Document.Blocks.Add(new Paragraph(new Run(message))));
                Dispatcher.Invoke(() => RichTextBox_GeneralLog.ScrollToEnd());
            }

            if (isPrintOnConsole)
            {
                Console.WriteLine(message);
            }
        }
        #endregion

        #region "UI"
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfig();
        }

        private void ImageBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (ImageBox_Main.Image == null)
                {
                    return;
                }

                if (_isSelecting && _isMouseDown)
                {
                    if (_isSelecting && _isMouseDown && _isFlexibleRoiSelected)
                    {
                        int x = Math.Min(_pictureBoxStartPoin.X, e.X);
                        int y = Math.Min(_pictureBoxStartPoin.Y, e.Y);
                        int width = Math.Max(_pictureBoxStartPoin.X, e.X) - Math.Min(_pictureBoxStartPoin.X, e.X);
                        int height = Math.Max(_pictureBoxStartPoin.Y, e.Y) - Math.Min(_pictureBoxStartPoin.Y, e.Y);

                        _pictureBoxRectangle = new System.Drawing.Rectangle(x, y, width, height);
                        ImageBox_Main.Refresh();
                    }

                    if (_isSelecting && _isMouseDown && _isFixedRoiSelected)
                    {
                        _pictureBoxCenterPoint = new System.Drawing.Point(e.X, e.Y);
                        ImageBox_Main.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleMessage($"Mouse move error: {ex.Message}", false, true, true, LogLevel.Error);
            }
        }

        private void ImageBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isSelecting == true)
            {
                _isMouseDown = true;

                _pictureBoxStartPoin = e.Location;
            }

            if (_isFlexibleRoiSelected)
            {
                _imageRoiStartPoint.X = (int)((e.Location.X) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                _imageRoiStartPoint.Y = (int)((e.Location.Y) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));
            }

            if (_isFixedRoiSelected)
            {
                _pictureBoxCenterPoint = new System.Drawing.Point(e.X, e.Y);

                ImageBox_Main.Refresh();
            }
        }

        private void ImageBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                _isMouseDown = false;
                _pictureBoxEndPoint = e.Location;
            }

            if (_isFlexibleRoiSelected)
            {
                _isFlexibleRoiSelected = false;
                _isMouseDown = false;

                _imageRoiEndPoint.X = (int)((e.Location.X) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                _imageRoiEndPoint.Y = (int)((e.Location.Y) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));

                int x = Math.Min(_imageRoiStartPoint.X, _imageRoiEndPoint.X);
                int y = Math.Min(_imageRoiStartPoint.Y, _imageRoiEndPoint.Y);
                int width = Math.Abs(_imageRoiStartPoint.X - _imageRoiEndPoint.X);
                int height = Math.Abs(_imageRoiStartPoint.Y - _imageRoiEndPoint.Y);

                _imageRoiRectangle = new System.Drawing.Rectangle(x, y, width, height);
                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "ROI Image", BasicAlgorithm.GetROI(Buffer_x12.GetImage(Buffer_x12.CurrentIndex), _imageRoiRectangle));
                HandleMessage("ROI image successfully added to buffer.", false, true, true, LogLevel.General);
            }

            if (_isFixedRoiSelected)
            {
                _isFixedRoiSelected = false;
                _isMouseDown = false;

                _imageRoiCenterPoint.X = (int)((e.Location.X) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                _imageRoiCenterPoint.Y = (int)((e.Location.Y) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));

                int x = (int)((e.Location.X - 60) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                int y = (int)((e.Location.Y - 60) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));
                int width = (int)((120) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Width) / ((sender as ImageBox).Width - 1));
                int height = (int)((120) * (float)(Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Height) / ((sender as ImageBox).Height - 1));

                _imageRoiRectangle = new System.Drawing.Rectangle(x, y, width, height);
                Buffer_x12.AddImage(Buffer_x12.VisibleCount, "ROI Image", BasicAlgorithm.GetROI(Buffer_x12.GetImage(Buffer_x12.CurrentIndex), _imageRoiRectangle));
                HandleMessage("ROI image successfully added to buffer.", false, true, true, LogLevel.General);
            }
        }

        private void ImageBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (_isMouseDown)
            {
                if (_isFlexibleRoiSelected)
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, _pictureBoxRectangle);
                    }
                }

                if (_isFixedRoiSelected)
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(_pictureBoxCenterPoint.X - 60, _pictureBoxCenterPoint.Y - 60, 120, 120));
                    }
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isInitialized)
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
                                        HandleMessage("Image opened successfully.", false, true, true, LogLevel.General);   
                                    }
                                    else
                                    {
                                        HandleMessage("Buffer is full.", false, true, true, LogLevel.Warning); 
                                    }
                                }
                                else
                                {
                                    HandleMessage("Image opening canceled.", false, true, true, LogLevel.Warning); 
                                }
                            }
                            break;

                        case "Btn_Save":
                            {
                                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                                saveFileDialog.Filter = ".bmp|*.bmp";

                                if (saveFileDialog.ShowDialog() == true)
                                {
                                    if (Buffer_x12.CurrentIndex >= 0 && Buffer_x12.CurrentIndex < Buffer_x12.VisibleCount)
                                    {
                                        Buffer_x12.GetImage(Buffer_x12.CurrentIndex).Save(saveFileDialog.FileName);
                                        HandleMessage("Image saved successfully.", false, true, true, LogLevel.General);
                                    }
                                    else
                                    {
                                        HandleMessage("Buffer is empty.", false, true, true, LogLevel.Warning);
                                    }
                                }
                                else
                                {
                                    HandleMessage("Image saving canceled.", false, true, true, LogLevel.Warning);
                                }
                            }
                            break;

                        case "Btn_Copy":
                            {
                                if (Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Copy Image", Buffer_x12.GetImage(Buffer_x12.CurrentIndex)))
                                {
                                    HandleMessage("Copy image successfully added to buffer.", false, true, true, LogLevel.General);
                                }
                                else
                                {
                                    HandleMessage("Buffer is full.", false, true, true, LogLevel.Warning);
                                }
                            }
                            break;

                        case "Btn_Delete":
                            {
                                if (Buffer_x12.RemoveImage(Buffer_x12.CurrentIndex))
                                {
                                    Buffer_x12.ResetAllRadioButtons();
                                    ImageBox_Main.Image = Buffer_x12.GetImage(Buffer_x12.CurrentIndex);
                                    HandleMessage("Succeeded to delete image.", false, true, true, LogLevel.General);
                                }
                                else
                                {
                                    HandleMessage("Failed to delete image.", false, true, true, LogLevel.General);
                                }
                            }
                            break;

                        case "Btn_Gray":
                            {
                                if (Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Grayscale Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)))))
                                {
                                    HandleMessage("Grayscale image successfully added to buffer.", false, true, true, LogLevel.General);
                                }
                                else
                                {
                                    HandleMessage("Buffer is full.", false, true, true, LogLevel.Warning);
                                }
                            }
                            break;

                        case "Btn_Dilation":
                            {
                                if (Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Dilated Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToDilation(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex))))))
                                {
                                    HandleMessage("Dilated image successfully added to buffer.", false, true, true, LogLevel.General);
                                }
                                else
                                {
                                    HandleMessage("Buffer is full.", false, true, true, LogLevel.Warning);
                                }
                            }
                            break;

                        case "Btn_Erosion":
                            {
                                if (Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Eroded Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToErosion(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex))))))
                                {
                                    HandleMessage("Eroded image successfully added to buffer.", false, true, true, LogLevel.General);
                                }
                                else
                                {
                                    HandleMessage("Buffer is full.", false, true, true, LogLevel.Warning);
                                }
                            }
                            break;

                        case "Btn_ROI":
                            {
                                if (RoiMode == 1)
                                    _isFlexibleRoiSelected = true;
                                else if (RoiMode == 2)
                                    _isFixedRoiSelected = true;
                                _isSelecting = true;
                            }
                            break;

                        case "Btn_Reset":
                            {
                                RestartApplication();
                            }
                            break;

                        default:
                            {
                                HandleMessage("Unknown button control activated in Btn_Clickn. Please check event handlers.", false, true, true, LogLevel.Error);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleMessage($"Unexpected error occurred: {ex.Message}", false, true, true, LogLevel.Error);
            }
        }

        private void Slide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (_isInitialized)
                {
                    switch ((sender as System.Windows.Controls.Slider).Name)
                    {
                        case "Slide_Binarization":
                            {
                                UiConfig.parameters.Threshold = Convert.ToInt32((sender as System.Windows.Controls.Slider).Value);
                                ImageBox_Main.Image = BasicAlgorithm.ToBGR(BasicAlgorithm.ToBinarization(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)), new Gray(UiConfig.parameters.Threshold)));
                                HandleMessage("Temporary binarization applied and displayed on main image.", false, false, true, LogLevel.General);
                            }
                            break;

                        default:
                            {
                                HandleMessage("Unknown slider control activated in Slide_ValueChanged. Please check event handlers.", false, true, true, LogLevel.Error); 
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleMessage($"Unexpected error occurred: {ex.Message}", false, true, true, LogLevel.Error);
            }
        }

        private void Slide_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_isInitialized)
                {
                    switch ((sender as System.Windows.Controls.Slider).Name)
                    {
                        case "Slide_Binarization":
                            {
                                UiConfig.parameters.Threshold = Convert.ToInt32((sender as System.Windows.Controls.Slider).Value);
                                if (Buffer_x12.AddImage(Buffer_x12.VisibleCount, "Binary Image", BasicAlgorithm.ToBGR(BasicAlgorithm.ToBinarization(BasicAlgorithm.ToGray(Buffer_x12.GetImage(Buffer_x12.CurrentIndex)), new Gray(UiConfig.parameters.Threshold)))))
                                {
                                    HandleMessage("Binarized image successfully added to buffer.", false, true, true, LogLevel.General);
                                }
                                else
                                {
                                    HandleMessage("Buffer is full.", false, true, true, LogLevel.Warning);
                                }
                            }
                            break;

                        default:
                            {
                                HandleMessage("Unknown slider control activated in Slide_MouseRightButtonDown. Please check event handlers.", false, true, true, LogLevel.Error); 
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleMessage($"Unexpected error occurred: {ex.Message}", false, true, true, LogLevel.Error);
            }
        }

        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                HandleMessage("Entered debug mode.", false, false, true, LogLevel.Debug);
            }

            if (e.Key == Key.H && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                HandleMessage("Left debug mode.", false, false, true, LogLevel.Debug);
            }
        }
       #endregion
    }
}
