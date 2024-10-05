using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO.Ports;
using Microsoft.VisualBasic.CompilerServices;
using System.Threading;
using LOGRECORDER;
using Config_sharp;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Lighting_WindowsControlLibrary_Wpf
{
    public class TimeStamp
    {
        public DateTime RecordTime;
        //public long Timeout;
        public bool IsTriggered;
        public bool IsTimeout { get { Interval = DateTime.Now.Ticks - RecordTime.Ticks;  return Interval > Timeout.Ticks; } }
        public long Interval;
        public TimeSpan Timeout;

        public TimeStamp()
        {
            RecordTime = new DateTime();
            IsTriggered = false;
            Timeout = TimeSpan.FromMilliseconds(1000);
            Interval = 0;
        }

        ~TimeStamp()
        {
        }

    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Lighting_WindowsControl_Wpf : UserControl
    {
        public delegate void StateEventHandler(bool IsOpen);
        public event StateEventHandler StateEvent;

        #region Variables

        // Variables for LogRecorder 
        // ---------------------------------------------------------------
        public InfoMgr LogWritter;
        bool IsLogInitSuccess = false;
        private string m_LogFileRecipeDirectionPath = System.Environment.CurrentDirectory + "/Appendix/Log/";
        private string m_LogFileNameHeader = "LightingControl";
        // ---------------------------------------------------------------

        // Variables for Config
        // ---------------------------------------------------------------
        private CustomConfig_Lighting m_Config;
        private string m_RecipeDirectoryPath = System.Environment.CurrentDirectory + "/Appendix/Config/";
        private string m_RecipeFilename = "LightingControl";
        private string m_RecipeSubtitle = ".dat";
        private string m_RecipeFullPath;
        // ---------------------------------------------------------------

        public List<TimeStamp> channelTriggerTimeList = new List<TimeStamp>();

        //private TimeStamp holdTime = new TimeStamp();
        private DateTime m_FlashTimeout = new DateTime(5000);

        private DispatcherTimer timer = new DispatcherTimer();

        private bool m_IsInitialized = false;

        private SerialPort _m_Rs232;
        private Stopwatch m_SpendTime;
        private string[] m_PortNames;
        private static int m_ButtonOffsets = 1;
        private static int m_MaxValue = 100;
        private static int m_MinValue = 0;       
        private static int m_BaseValue = 0; 
        private static int m_IntervalValue = 4096 - m_BaseValue;

        // =========接收字元的POOL===============
        public delegate void DataReceivedEventHandler();
        public event DataReceivedEventHandler DataReceived;

        private string m_bufferStr;
        private string m_finalStr;

        private int m_SelectedIndex;
        private string m_SelectedPort;

        private SerialPort m_Rs232
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _m_Rs232;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_m_Rs232 != null)
                {
                    _m_Rs232.DataReceived -= RS232_DataReceived;
                }

                _m_Rs232 = value;
                if (_m_Rs232 != null)
                {
                    _m_Rs232.DataReceived += RS232_DataReceived;
                }
            }
        }

        public string[] GetPortNames
        {
            get
            {
                m_PortNames = SerialPort.GetPortNames();
                return m_PortNames;
            }
        }

        public string BufferStr
        {
            get
            {
                string BufferStrRet = default;
                BufferStrRet = m_bufferStr;
                return BufferStrRet;
            }
        }

        public string FinalStr
        {
            get
            {
                string FinalStrRet = default;
                FinalStrRet = m_finalStr;
                return FinalStrRet;
            }
        }

        public int m_CH1_Value;
        public int m_CH2_Value;
        public int m_CH3_Value;
        public int m_CH4_Value;

        public int CH1_Value
        {
            set
            {
                // Check and set CH value is between 0 ~ 100
                m_CH1_Value = (value < m_MinValue) ? 0 : (value > m_MaxValue ? m_MaxValue : value);
                
                this.SliderCH1.Value = m_CH1_Value;
                this.TextBoxValue_CH1.Text = m_CH1_Value.ToString();

                /*
                if (isHoldOnAWhile == false && m_Rs232 is object)
                {
                    holdTime.IsTriggered = true;
                    holdTime.RecordTime = DateTime.Now;

                    SWR(1, m_CH1_Value);
                }
                */
                SWR(1, m_CH1_Value);
            }

            get
            {
                return m_CH1_Value;
            }
        }

        public int CH2_Value
        {
            set
            {
                // Check and set CH value is between 0 ~ 100
                m_CH2_Value = (value < m_MinValue) ? 0 : (value > m_MaxValue ? m_MaxValue : value);
                SliderCH2.Value = m_CH2_Value;
                this.TextBoxValue_CH2.Text = m_CH2_Value.ToString();

                /*
                if (isHoldOnAWhile == false && m_Rs232 is object)
                {
                    holdTime.IsTriggered = true;
                    holdTime.RecordTime = DateTime.Now;

                    SWR(2, m_CH2_Value);
                }
                */
                SWR(2, m_CH2_Value);
            }

            get
            {
                return m_CH2_Value;
            }
        }

        public int CH3_Value
        {
            set
            {
                // Check and set CH value is between 0 ~ 100
                m_CH3_Value = (value < m_MinValue) ? 0 : (value > m_MaxValue ? m_MaxValue : value);
                this.SliderCH3.Value = m_CH3_Value;
                this.TextBoxValue_CH3.Text = m_CH3_Value.ToString();

                /*
                if (isHoldOnAWhile == false && m_Rs232 is object)
                {
                    holdTime.IsTriggered = true;
                    holdTime.RecordTime = DateTime.Now;

                    SWR(3, m_CH3_Value);
                }
                */
                SWR(3, m_CH3_Value);
            }

            get
            {
                return m_CH3_Value;
            }
        }

        public int CH4_Value
        {
            set
            {
                // Check and set CH value is between 0 ~ 100
                m_CH4_Value = (value < m_MinValue) ? 0 : (value > m_MaxValue ? m_MaxValue : value);
                this.SliderCH4.Value = m_CH4_Value;
                this.TextBoxValue_CH4.Text = m_CH4_Value.ToString();

                /*
                if (isHoldOnAWhile == false && m_Rs232 is object)
                {
                    holdTime.IsTriggered = true;
                    holdTime.RecordTime = DateTime.Now;

                    SWR(3, m_CH3_Value);
                }
                */
                SWR(4, m_CH4_Value);
            }

            get
            {
                return m_CH4_Value;
            }
        }

        private void GetAllSerialDevice()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            // Display each port name to the console.
            foreach (string port in ports)
            {
                ComboBoxItem item = new ComboBoxItem();

                item.Content = port.ToString();

                ComPortList.Items.Add(item);
            }
        }

        public int SelectedIndex { get => m_SelectedIndex; }

        public string SelectedPort { get => m_SelectedPort; }
        public static int ButtonOffsets { get => m_ButtonOffsets; set => m_ButtonOffsets = value; }
        public bool IsInitialized1 { get => m_IsInitialized; set => m_IsInitialized = value; }
        #endregion

        #region Basic Functions

        public Lighting_WindowsControl_Wpf()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += Timer_Tick;
                        
            channelTriggerTimeList.Add(new TimeStamp());
            channelTriggerTimeList.Add(new TimeStamp());
            channelTriggerTimeList.Add(new TimeStamp());
            channelTriggerTimeList.Add(new TimeStamp());
        }

        public void Init(string serialPort, string deviceName)
        {
            try
            { 
                SetPort(serialPort);

                SetDeviceName(deviceName);

                m_CH1_Value = 0;
                m_CH2_Value = 0;
                m_CH3_Value = 0;
                m_CH4_Value = 0;

                Config_Init();

                LogFile_Initialization();
            }
            catch (Exception ex)
            {
                if (IsLogInitSuccess) LogWritter.MsgError("Initialized Error. Message: " + ex.Message);
                else Console.WriteLine("Initialized Error. Message: " + ex.Message);
                throw;
            }

            if(IsLogInitSuccess) LogWritter.MsgGenLog("Initialized Successfully.");
        }

        public void Config_Init()
        {
            // Append "RecipeFilename" to "DeviceName"
            m_RecipeFilename += DeviceName.Content;

            m_RecipeFullPath = m_RecipeDirectoryPath + m_RecipeFilename + m_RecipeSubtitle;

            m_Config = new CustomConfig_Lighting(m_RecipeFullPath);

            ButtonLoad_Click(new object(), new RoutedEventArgs());
        }

        public void LogFile_Initialization()
        {
            string path = m_LogFileRecipeDirectionPath + m_LogFileNameHeader + DeviceName.Content;

            LogFile_Initialization(path);
        }

        public void LogFile_Initialization(string path)
        {
            string GenLog, WarningLog, ErrLog, DebugLog;

            try
            {
                GenLog = path + "./GeneralLog";
                WarningLog = path + "./WarningLog";
                ErrLog = path + "./ErrorLog";
                DebugLog = path + "./DebugLog";

                LogWritter = new InfoMgr(GenLog, WarningLog, ErrLog, DebugLog);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Log檔無法記錄資訊! Message:" + ex.Message);
            }

            if (LogWritter != null) IsLogInitSuccess = true;
        }

        public bool Open()
        {
            if (Open(SelectedPort))
            {
                timer.IsEnabled = true;
                timer.Start();

                return true;
            }
            return false;
        }

        public bool Open(string port)
        {
            try
            {
                m_Rs232 = new SerialPort();
                m_SpendTime = new Stopwatch();
                m_bufferStr = "";
                this.StatusIcon.Fill = Brushes.Red;

                if (m_Rs232.IsOpen)
                {
                    m_Rs232.Close();
                }

                // Me.TextBox_ComPort.Text = port

                m_Rs232.PortName = port;
                m_Rs232.BaudRate = 57600;
                m_Rs232.Parity = Parity.None;
                m_Rs232.DataBits = 8;
                m_Rs232.StopBits = StopBits.One;
                m_Rs232.RtsEnable = false;
                m_Rs232.DtrEnable = false;
                m_Rs232.Open();

                this.StatusIcon.Fill = Brushes.Green;
                this.ValueSettingGroup.IsEnabled = true;

                this.m_IsInitialized = true;
                StateEvent.Invoke(m_IsInitialized);

                return true;
            }
            catch (Exception ex) when (m_Rs232.IsOpen == false)
            {
                StateEvent.Invoke(false);
                StateEvent.Invoke(m_IsInitialized);

                this.StatusIcon.Fill = Brushes.Red;
                if (IsLogInitSuccess) LogWritter.MsgError("Lighting Opening Error. Message: " + ex.Message);
                return false;
            }

            return false;
        }

        public bool Close()
        {
            if (m_Rs232 is object == false) return false;

            try
            {
                timer.Stop();
                timer.IsEnabled = false;

                if (m_Rs232.IsOpen)
                {
                    this.StatusIcon.Fill = Brushes.Red;
                    this.ValueSettingGroup.IsEnabled = false;
                    m_Rs232.Close();
                }

                StateEvent.Invoke(false);

                return true;
            }
            catch (Exception ex)
            {
                StateEvent.Invoke(false);

                if (IsLogInitSuccess) LogWritter.MsgError("Lighting Closing Error. Message: " + ex.Message);
                
                return false;
            }
        }

        public void TurnOffLights()
        {
            SWR(1, 0);
            SWR(2, 0);
            SWR(3, 0);
            SWR(4, 0);
        }

        #endregion

        #region Functions

        void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                /*
                if (holdTime.IsTriggered == true && holdTime.IsTimeout)
                {
                    holdTime.IsTriggered = false;

                    if (m_Rs232 is object)
                    {
                        SWR(1, m_CH1_Value);
                        SWR(2, m_CH2_Value);
                        SWR(3, m_CH3_Value);
                        SWR(4, m_CH4_Value);
                    }
                }
                */

                for (int i = 0; i < 3; i++)
                {
                    if (channelTriggerTimeList[i].IsTriggered && channelTriggerTimeList[i].IsTimeout)
                    {
                        long interval = channelTriggerTimeList[i].Interval;
                        channelTriggerTimeList[i].IsTriggered = false;

                        SWR(i + 1, 0);
                        Console.WriteLine("Ticks per millisecond: " + (interval / 10000).ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                if (IsLogInitSuccess) LogWritter.MsgError("Timer tick error\n" + ex.ToString());
            }
        }

        public void SetPort(string port)
        {
            try
            {
                if (m_Rs232 != null)
                {
                    if (m_Rs232.IsOpen)
                    {
                        Close();
                    }
                }

                m_SelectedPort = port;
                Label_ComPort.Content = port;

                Open();
            }
            catch (Exception ex)
            {
                if (IsLogInitSuccess) LogWritter.MsgError("SetPort Error. Message: " + ex.Message);
            }
        }

        public void SetDeviceName(string name)
        {
            try
            {
                this.DeviceName.Content = name;
            }
            catch (Exception ex)
            {
                if (IsLogInitSuccess) LogWritter.MsgError("SetDeviceName Error. Message: " + ex.Message);
            }
        }

        private void SWR(int ch, int light_val)
        {
            if (m_IsInitialized == false) return;
            int val = -1;

            light_val = light_val < 0 ? 0 : light_val > 100 ? 100 : light_val;

            try
            {
                // Convert percent to value
                light_val = Convert.ToInt32(m_BaseValue + m_IntervalValue * light_val * 0.01 - 1);

                light_val = light_val < 0 ? 0 : light_val > 4095 ? 4095 : light_val;

                // ch: 1~4 , light_val 0~4095
                
                val = 10000 * ch + light_val;
                RS232_DataTransmit_ByString("SWR" + val.ToString() + Constants.vbCrLf);
            }
            catch (Exception ex)
            {
                MessageBox.Show("光源設定異常!");
                if (!IsLogInitSuccess) return;

                LogWritter.MsgError("Lighting Set Value Error. Message: " + ex.Message);

                LogWritter.MsgDebug("---------------------------------------------");
                LogWritter.MsgDebug("Lighting Set Value Variables /n");
                LogWritter.MsgDebug("---------------------------------------------");
                LogWritter.MsgDebug("Channel: " + ch.ToString() + "/n");
                LogWritter.MsgDebug("Value: " + light_val.ToString() + "/n");
                LogWritter.MsgDebug("Value Sent: " + val.ToString() + "/n");
                LogWritter.MsgDebug("---------------------------------------------");
            }
        }

        private void RS232_DataTransmit_ByString(string str)
        {
            if (m_Rs232 is object)
            {
                m_Rs232.DiscardInBuffer(); // Clear input buffer
                m_Rs232.Write(str); // & Chr(13)) ' 送出Request
            }
        }

        private void RS232_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
                return;

            string inDataStr = "";
            try
            {
                inDataStr = m_Rs232.ReadExisting();
            }
            catch (Exception)
            {
                Console.WriteLine("Lighting Read Rs232 Error!");
            }

            m_bufferStr += inDataStr;

            // ==============等換行符號=== Chr(13) VbCr
            if (inDataStr.Contains(Conversions.ToString((char)13)))
            {
                var a = m_bufferStr.Split((char)13);
                m_finalStr = a[0];
                m_bufferStr = "";  // 清空
                DataReceived?.Invoke();
            }
        }

        public void Trigger(int channel)
        {
            if (channel == 1)
                Trigger(channel, m_FlashTimeout.Ticks, m_CH1_Value);
            else if (channel == 2)
                Trigger(channel, m_FlashTimeout.Ticks, m_CH2_Value);
            else if (channel == 3)
                Trigger(channel, m_FlashTimeout.Ticks, m_CH3_Value);
            else if (channel == 4)
                Trigger(channel, m_FlashTimeout.Ticks, m_CH4_Value);
        }

        public void Trigger(int channel, long timeout)
        {
            if (channel == 1)
                Trigger(channel, timeout, m_CH1_Value);
            else if (channel == 2)
                Trigger(channel, timeout, m_CH2_Value);
            else if (channel == 3)
                Trigger(channel, timeout, m_CH3_Value);
            else if (channel == 4)
                Trigger(channel, timeout, m_CH4_Value);
        }

        public void Trigger(int channel, long timeout, int intensity)
        {
            if (channel >= 1 && channel <= 3)
            {
                int index = channel - 1;

                if (channelTriggerTimeList[index].IsTriggered == true) return;

                channelTriggerTimeList[index].IsTriggered = true;
                channelTriggerTimeList[index].Timeout = TimeSpan.FromMilliseconds(timeout);

                channelTriggerTimeList[index].RecordTime = DateTime.Now;

                SWR(channel, intensity);
            }
        }

        private void WriteLogMessage(string msg)
        {
        }

        #endregion

        #region Controller Event
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;

            switch (btn.Content)
            {
                case "Save":
                    ButtonSave_Click(sender, e);
                    break;
                case "Load":
                    ButtonLoad_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void SliderCH1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //CH1_Value = Convert.ToInt32(SliderCH1.Value);
            CH1_Value = Convert.ToInt32((sender as Slider).Value);

        }

        private void SliderCH2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //CH2_Value = Convert.ToInt32(SliderCH2.Value);
            CH2_Value = Convert.ToInt32((sender as Slider).Value);

        }

        private void SliderCH3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //CH3_Value = Convert.ToInt32(SliderCH3.Value);
            CH3_Value = Convert.ToInt32((sender as Slider).Value);

        }

        private void SliderCH4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //CH3_Value = Convert.ToInt32(SliderCH3.Value);
            CH4_Value = Convert.ToInt32((sender as Slider).Value);

        }

        private void TextBoxValue_CH1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CH1_Value = Convert.ToInt32(TextBoxValue_CH1.Text);
            CH1_Value = Convert.ToInt32((sender as TextBox).Text);

        }

        private void TextBoxValue_CH2_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CH2_Value = Convert.ToInt32(TextBoxValue_CH2.Text);
            CH2_Value = Convert.ToInt32((sender as TextBox).Text);

        }

        private void TextBoxValue_CH3_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CH3_Value = Convert.ToInt32(TextBoxValue_CH3.Text);
            CH3_Value = Convert.ToInt32((sender as TextBox).Text);

        }

        private void TextBoxValue_CH4_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CH3_Value = Convert.ToInt32(TextBoxValue_CH3.Text);
            CH4_Value = Convert.ToInt32((sender as TextBox).Text);

        }

        private void ButtonUp_CH1_Click(object sender, RoutedEventArgs e)
        {
            CH1_Value += ButtonOffsets;
        }

        private void ButtonDown_CH1_Click(object sender, RoutedEventArgs e)
        {
            CH1_Value -= ButtonOffsets;
        }

        private void ButtonUp_CH2_Click(object sender, RoutedEventArgs e)
        {
            CH2_Value += ButtonOffsets;
        }

        private void ButtonDown_CH2_Click(object sender, RoutedEventArgs e)
        {
            CH2_Value -= ButtonOffsets;
        }

        private void ButtonUp_CH3_Click(object sender, RoutedEventArgs e)
        {
            CH3_Value += ButtonOffsets;
        }

        private void ButtonDown_CH3_Click(object sender, RoutedEventArgs e)
        {
            CH3_Value -= ButtonOffsets;
        }

        private void ButtonUp_CH4_Click(object sender, RoutedEventArgs e)
        {
            CH4_Value += ButtonOffsets;
        }

        private void ButtonDown_CH4_Click(object sender, RoutedEventArgs e)
        {
            CH4_Value -= ButtonOffsets;
        }

        private void ComPortList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Close();

            m_SelectedIndex = ComPortList.SelectedIndex;

            m_SelectedPort = (ComPortList.SelectedItem.ToString().Split(' '))[1];

            Open();
        }

        public void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            if (m_Config.Load() == false) return;

            CH1_Value = Convert.ToInt32(m_Config.parameters.CH1_Value);
            CH2_Value = Convert.ToInt32(m_Config.parameters.CH2_Value);
            CH3_Value = Convert.ToInt32(m_Config.parameters.CH3_Value);
            CH4_Value = Convert.ToInt32(m_Config.parameters.CH4_Value);

            //TurnOffLights();
        }

        public void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            m_Config.parameters.CH1_Value = CH1_Value;
            m_Config.parameters.CH2_Value = CH2_Value;
            m_Config.parameters.CH3_Value = CH3_Value;
            m_Config.parameters.CH4_Value = CH4_Value;

            m_Config.Save();

            //TurnOffLights();
        }

        #endregion

        #region VB Comment from Gary
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
        // Public Sub DataTransmit_ByByte(ByVal str As String)
        // Dim transmitString As String
        // transmitString = str '& Chr(13)

        // Dim aa(transmitString.Length) As Byte
        // aa = StrToByteArray(transmitString)

        // Me.m_Rs232.DiscardInBuffer()                        ' Clear input buffer
        // Me.m_Rs232.Write(aa, 0, aa.GetLength(0)) ' 送出Request
        // End Sub

        // Public Sub DataTransmit_HexStrByByte(ByVal str As String)
        // Dim transmitString As String
        // transmitString = str '& Chr(13)

        // Dim aa(CInt(transmitString.Length / 2) - 1) As Byte
        // aa = HexStr2ByteArray(transmitString)

        // Me.m_Rs232.DiscardInBuffer()                        ' Clear input buffer
        // Me.m_Rs232.Write(aa, 0, aa.GetLength(0)) ' 送出Request
        // End Sub

        // ' 將字串轉成byte陣列
        // Private Function StrToByteArray(ByVal str As String) As Byte()
        // Dim encoding As New System.Text.ASCIIEncoding()
        // Return encoding.GetBytes(str)
        // End Function
        // Private Function FormatTrans(ByVal input As Double) As String
        // Dim TransString As String
        // TransString = String.Format("{0:000.000}", input)
        // Return TransString
        // End Function
        // ' 將Hex字串轉成byte陣列
        // Public Function HexStr2ByteArray(ByVal str As String) As Byte()
        // Dim sendData As Byte() = New Byte(CInt(str.Length / 2) - 1) {}
        // For i As Integer = 0 To (sendData.Length - 1)
        // sendData(i) = CByte(Convert.ToInt32(str.Substring(i * 2, 2), 16))
        // Next i
        // Return sendData
        // End Function
        #endregion
    }
}

