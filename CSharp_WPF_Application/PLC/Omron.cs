using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using Omron.Ether;
using LOGRECORDER;
using Config_sharp;
using System.Text;

namespace PLC
{
    public class OmronPLC
    {
        public delegate void TriggerHandler(int K1_TriggerOnOFF, int K1_TriggerCount, int K1_ObjectNumber, int K2_TriggerOnOFF, int K2_ObjectNumber, int K3_TriggerOnOFF, int K3_ObjectNumber, string K_Barcode);
        public event TriggerHandler StationTriggerEvent;

        // Variables for PLC
        // ---------------------------------------------------------------
        public readonly object ReadLocked = new object();
        public bool AutoFlag = false;
        public bool ReadFlag = false;
        public bool[] ReturnFlag = new bool[3] { false, false, false };
        public string Barcode = "";
        public int[] SerialNumber = new int[3] { 0, 0, 0 };
        public FinsTcpCS MajorPLC;
        public Thread PLCTimerThread;
        // ---------------------------------------------------------------

        // Variables for LogRecorder 
        // ---------------------------------------------------------------
        public CustomConfig_PLCParameters PLC_Config;
        // ---------------------------------------------------------------

        // Variables for LogRecorder 
        // ---------------------------------------------------------------
        public InfoMgr LogWritter;
        public bool IsLogInitSuccess = false;
        private string m_LogFileRecipeDirectionPath = System.Environment.CurrentDirectory + "/Appendix/Log/";
        private string m_LogFileNameHeader = "PLC";
        // ---------------------------------------------------------------

        // Variables for Debug 
        // ---------------------------------------------------------------
        public bool[] DebugFlag = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        public bool[] PrintFlag = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        // ---------------------------------------------------------------

        public OmronPLC()
        {
            MajorPLC = new FinsTcpCS();
            PLC_Config = new CustomConfig_PLCParameters();

            LogFile_Init();
        }

        ~OmronPLC()
        {
            
        }

        public void LogFile_Init()
        {
            string path = m_LogFileRecipeDirectionPath + m_LogFileNameHeader;

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
                Console.WriteLine("Initialized Error. Message: " + ex.Message); //throw;
            }

            if (LogWritter != null) IsLogInitSuccess = true;
        }

        public void Open(System.String PLC_IP, int PLC_Port, string StstionName)
        {
            try
            {
                MajorPLC.Open(PLC_IP, PLC_Port);

                if (MajorPLC.IsOpen)
                {
                    PLC_Config.parameters.IsOpen = true;
                    PLC_Config.parameters.StstionIP = PLC_IP;
                    PLC_Config.parameters.StstionPort = PLC_Port;
                    PLC_Config.parameters.StstionName = StstionName;

                    PLCTimerThread = new Thread(PLCTimer_Thread);
                    PLCTimerThread.Start();

                    LogWritter.MsgGenLog("PLC Opened Successfully.");
                }
                else
                {
                    PLC_Config.parameters.IsOpen = false;
                    PLC_Config.parameters.StstionIP = "";
                    PLC_Config.parameters.StstionPort = 0;
                    PLC_Config.parameters.StstionName = "";

                    LogWritter.MsgGenLog("PLC Opened Unsuccessfully.");
                }
            }
            catch (Exception Ex)
            {
                LogWritter.MsgGenLog("Exception Occured When PLC Opened. Message: " + Ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                PLC_Config.parameters.IsOpen = false; 
                PLC_Config.parameters.StstionIP = "";
                PLC_Config.parameters.StstionPort = 0;
                PLC_Config.parameters.StstionName = "";

                if (PLC_Config.parameters.IsOpen)
                    PLCTimerThread.Abort();
                if (MajorPLC.IsOpen)
                    MajorPLC.Close();

                LogWritter.MsgGenLog("PLC Closed Successfully.");
            }
            catch (Exception Ex)
            {
                LogWritter.MsgGenLog("Exception Occured When PLC Closed. Message: " + Ex.Message);
            }
        }

        public void PLCTimer_Thread()
        {
            #region "Edition 01"
            if (false)
            {
                //The "Edition 01" Had Been Removed On 2020/10/23. 
                //If You Want To See The Code Of "Edition 01", You Have To Go To Odd Edition Program To Find.
            }
            #endregion

            #region "Edition 02"
            if (false)
            {
                //The "Edition 01" Had Been Removed On 2020/10/26. 
                //If You Want To See The Code Of "Edition 02", You Have To Go To Odd Edition Program To Find.
            }
            #endregion

            #region "Edition 03"
            if (true)
            {
                while (PLC_Config.parameters.IsOpen)
                {
                    try
                    {
                        if (AutoFlag)
                        {
                            if (MajorPLC.IsOpen)
                            {
                                if (ReadFlag)
                                {
                                    if (MajorPLC.GetBit(eDeviceType.DM, 30013, 0) == 0)
                                    {
                                        MajorPLC.SetBit(eDeviceType.DM, 30013, 0, 1);
                                    }

                                    if (MajorPLC.GetBit(eDeviceType.DM, 30010, 0) == 1)
                                    {
                                        MajorPLC.WriteWord(eDeviceType.DM, 30014, 0);
                                    }

                                    switch (Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30012)))
                                    {
                                        case 0:
                                            break;

                                        case 1:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(Assign(0), Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30022)), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30023)), 0, 0, 0, 0, Barcode);
                                            
                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (1).ToString());
                                            break;

                                        case 2:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(0, 0, 0, Assign(1), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30033)), 0, 0, Barcode);

                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (2).ToString());
                                            break;

                                        case 3:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(Assign(0), Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30022)), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30023)), Assign(1), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30033)), 0, 0, Barcode);
                                            
                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (3).ToString());
                                            break;

                                        case 4:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(0, 0, 0, 0, 0, Assign(2), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30043)), Barcode);

                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (4).ToString());
                                            break;

                                        case 5:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(Assign(0), Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30022)), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30023)), 0, 0, Assign(2), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30043)), Barcode);
                                            
                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (5).ToString());
                                            break;

                                        case 6:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(0, 0, 0, Assign(1), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30033)), Assign(2), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30043)), Barcode);
                                            
                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (6).ToString());
                                            break;

                                        case 7:
                                            Barcode = GetBarcode();
                                            StationTriggerEvent.Invoke(Assign(0), Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30022)), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30023)), Assign(1), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30033)), Assign(2), Convert.ToInt32(MajorPLC.ReadFloat(eDeviceType.DM, 30043)), Barcode);
                                            
                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 1);
                                            ReadFlag = false;

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (7).ToString());
                                            break;

                                        case 100:
                                            MajorPLC.WriteWord(eDeviceType.DM, 30025, 0);
                                            MajorPLC.WriteFloat(eDeviceType.DM, 30027, 0);
                                            MajorPLC.WriteWord(eDeviceType.DM, 30035, 0);
                                            MajorPLC.WriteWord(eDeviceType.DM, 30045, 0);
                                            MajorPLC.WriteWord(eDeviceType.DM, 30014, 0);

                                            LogWritter.MsgGenLog("Barcode: " + Barcode + ", PLC Status: " + (100).ToString());
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                LogWritter.MsgGenLog("PLC Did Not Open When Thread Was Going.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWritter.MsgGenLog("Exception Occurred When Thread Was Going. Message: " + ex.Message);
                    }
                    finally
                    {
                        Thread.Sleep(5);
                    }
                }
            }
            #endregion
        }

        public bool SetBit(int StartWord, int Bit, ushort Value)
        {
            try
            {
                lock (ReadLocked)
                {
                    MajorPLC.SetBit(eDeviceType.DM, StartWord, Bit, Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception Occurred In OmronPLC.SetBit(" + StartWord + "," + Bit + "," + Value + ") : " + ex.Message);
                return false;
            }
        }

        public ushort GetBit(int StartWord, int Bit)
        {
            try
            {
                lock (ReadLocked)
                {
                    return MajorPLC.GetBit(eDeviceType.DM, StartWord, Bit);
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.GetBit(" + StartWord + "," + Bit + ") : " + ex.Message);
                throw ex;
            }
        }

        public bool SetDword(int StartWord, uint Value)
        {
            try
            {
                lock (ReadLocked)
                {
                    MajorPLC.WriteDword(eDeviceType.DM, StartWord, Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.SetDword(" + StartWord + "," + Value + ") : " + ex.Message);
                return false;
            }
        }

        public uint GetDword(int StartWord)
        {
            try
            {
                lock (ReadLocked)
                {
                    return MajorPLC.ReadDword(eDeviceType.DM, StartWord);
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.GetDword(" + StartWord + ") : " + ex.Message);
                throw ex;
            }
        }

        public bool SetWord(int StartWord, ushort Value)
        {
            try
            {
                lock (ReadLocked)
                {
                    MajorPLC.WriteWord(eDeviceType.DM, StartWord, Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.SetWord(" + StartWord + "," + Value + ") : " + ex.Message);
                return false;
            }
        }

        public ushort GetWord(int StartWord)
        {
            try
            {
                lock (ReadLocked)
                {
                    return MajorPLC.ReadWord(eDeviceType.DM, StartWord);
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.GetWord(" + StartWord + ") : " + ex.Message);
                throw ex;
            }
        }

        public bool SetFloat(int StartWord, float Value)
        {
            try
            {
                lock (ReadLocked)
                {
                    MajorPLC.WriteFloat(eDeviceType.DM, StartWord, Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.SetFloat(" + StartWord + "," + Value + ") : " + ex.Message);
                return false;
            }
        }

        public float GetFloat(int StartWord)
        {
            try
            {
                lock (ReadLocked)
                {
                    return MajorPLC.ReadFloat(eDeviceType.DM, StartWord);
                }
            }
            catch (Exception ex)
            {
                LogWritter.MsgGenLog("Exception In OmronPLC.GetFloat(" + StartWord + ") : " + ex.Message);
                throw ex;
            }
        }

        public string GetBarcode()
        {
            string t_RecieveString = "";
            string t_ReturnString = "";

            for (int i = 0; i < 10; i++)
            {
                t_RecieveString = ReverseByArray(ConvertHex(MajorPLC.ReadWord(eDeviceType.DM, (30000 + i)).ToString("X4")));

                for (int j = 0; j < t_RecieveString.Length; j++)
                {
                    switch (t_RecieveString[j])
                    {
                        case '0':
                            t_ReturnString += "0";
                            break;

                        case '1':
                            t_ReturnString += "1";
                            break;

                        case '2':
                            t_ReturnString += "2";
                            break;

                        case '3':
                            t_ReturnString += "3";
                            break;

                        case '4':
                            t_ReturnString += "4";
                            break;

                        case '5':
                            t_ReturnString += "5";
                            break;

                        case '6':
                            t_ReturnString += "6";
                            break;

                        case '7':
                            t_ReturnString += "7";
                            break;

                        case '8':
                            t_ReturnString += "8";
                            break;

                        case '9':
                            t_ReturnString += "9";
                            break;

                        case '-':
                            t_ReturnString += "-";
                            break;

                        default:
                            break;
                    }
                }
            }

            return t_ReturnString;
        }

        public int Assign(int Number)
        {
            switch (Number)
            {
                case 0:
                    return Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30020)) % 16;

                case 1:
                    return Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30030)) % 4;

                case 2:
                    return Convert.ToInt32(MajorPLC.ReadWord(eDeviceType.DM, 30040)) % 2;

                default:
                    return 0;
            }
        }

        public static string ConvertHex(string hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    string hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    ulong decval = Convert.ToUInt64(hs, 16);
                    long deccc = Convert.ToInt64(hs, 16);
                    char character = Convert.ToChar(deccc);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

        public string ReverseByArray(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public void UseTime(string title, TimeSpan ts)
        {
            if (PrintFlag[0])
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                Console.WriteLine(title + elapsedTime);
            }
        }
    }
}