//#define Edition_001
#undef Edition_001
#define Edition_002
//#undef Edition_002

//System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// WPF Xaml Form
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//Stench Camera
using Sentech.GenApiDotNET;
using Sentech.StApiDotNET;
// EmguCV
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
//Config
using Config_sharp;
//Log
using LOGRECORDER;

namespace CameraDevice
{
    public class StCamera
    {
        #region "Variable And Event"
        // Variable And Event For Continuing Grab.
        // ------------------------------
        public delegate void ContinueGrabHandler(uint deviceNumber);
        public event ContinueGrabHandler ContinueGrabEvent;

        public readonly object ContinueGrabLocked = new object();

        public bool[] IsRecording;
        public bool[] IsConverting;
        public bool[] IsFinished;
        // ------------------------------

        // Camera device
        // ------------------------------
        public int nCountOfCameraDevice = 3;

        public const int nCountOfImagesToGrab = 1;

        private int StationNumber = 0;

        public bool IsOpened = false;
        // ------------------------------

        // StApi Variables
        // ------------------------------
        public CStApiAutoInit api;
        public CStSystem system;
        public CStDeviceArray deviceList;
        public CStDataStream dataStream;
        public CStDataStreamArray dataStreamList;
        // ------------------------------

        // Variables for LogRecorder 
        // ------------------------------
        public InfoMgr LogWritter;
        bool IsLogInitSuccess = false;
        private string m_LogFileRecipeDirectionPath = System.Environment.CurrentDirectory + "/Appendix/Log/";
        private string m_LogFileNameHeader = "Camera";
        // ------------------------------

        //Other
        // ------------------------------
        public Component[] components;

        public List<string> deviceSerialNumberList = new List<string>();
        // ------------------------------
        #endregion

        public StCamera()
        {
            try
            {
                IsRecording = new bool[nCountOfCameraDevice];
                for (int index = 0; index < nCountOfCameraDevice; index++)
                    IsRecording[index] = false;

                IsConverting = new bool[nCountOfCameraDevice];
                for (int index = 0; index < nCountOfCameraDevice; index++)
                    IsConverting[index] = false;

                IsFinished = new bool[nCountOfCameraDevice];
                for (int index = 0; index < nCountOfCameraDevice; index++)
                    IsFinished[index] = true;

                LogFile_Init();

                StAPI_Init();

                PropertyInitization();

                CameraDeviceScan();

                IsOpened = true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Initialized Error. Message: " + Ex.Message);
                return;
            }
        }

        public StCamera(int stationNuber, int countOfCameraDevice)
        {
            try
            {
                IsRecording = new bool[nCountOfCameraDevice];
                for (int index = 0; index < nCountOfCameraDevice; index++)
                    IsRecording[index] = false;

                IsConverting = new bool[nCountOfCameraDevice];
                for (int index = 0; index < nCountOfCameraDevice; index++)
                    IsConverting[index] = false;

                IsFinished = new bool[nCountOfCameraDevice];
                for (int index = 0; index < nCountOfCameraDevice; index++)
                    IsFinished[index] = true;

                StationNumber = stationNuber;

                nCountOfCameraDevice = countOfCameraDevice;

                LogFile_Init();

                StAPI_Init();

                PropertyInitization();

                CameraDeviceScan();

                IsOpened = true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Camera Initialized Error. Message: " + Ex.Message);
                return;
            }
        }

        ~StCamera()
        {
            //CameraDeviceClose();

            api = null;
            system = null;
            deviceList = null;

            IsOpened = false;
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
            catch (Exception Ex)
            {
                Console.WriteLine("Log Of Camera Initialized Error. Message: " + Ex.Message);
                throw;
            }

            if (LogWritter != null) IsLogInitSuccess = true;
        }

        private void StAPI_Init()
        {
            api = new CStApiAutoInit();
            system = new CStSystem();
            deviceList = new CStDeviceArray();
        }

        private void PropertyInitization()
        {
            deviceSerialNumberList = new List<string>();

            switch (StationNumber)
            {
                case 0:
                    deviceSerialNumberList.Add("18K7488");
                    deviceSerialNumberList.Add("18K7493");
                    //deviceSerialNumberList.Add("18K7500");
                    deviceSerialNumberList.Add("18K7504");
                    break;
                case 1:
                    deviceSerialNumberList.Add("18K7511");
                    break;
                default:
                    LogWritter.MsgWarning("Input Of Switch Was Not Suitable In StCamera.PropertyInitization().");
                    break;
            }
        }

        public void CameraDeviceScan()
        {
            components = new Component[nCountOfCameraDevice];
            for (int i = 0; i < nCountOfCameraDevice; i++)
            {
                components[i] = new Component();
            }

#if Edition_001
            // Create a DataStream list object to store all the data stream object related to the cameras.           
            using (dataStreamList = new CStDataStreamArray())
            {
                CStDevice device = null;

                for (UInt16 i = 0; ; i++)
                {
                    try
                    {
                        // Create a camera device object and connect to first detected device.
                        device = system.CreateFirstStDevice();
                    }
                    catch
                    {
                        if (dataStreamList.GetSize() == 0)
                        {
                            Console.WriteLine("Device not found.");
                            return;
                        }

                        break;
                    }

                    // Add the camera into device object list for later usage.
                    deviceList.Register(device);

                    // Displays the DisplayName of the device.
                    Console.WriteLine("Device" + deviceList.GetSize() + "=" + device.GetIStDeviceInfo().DisplayName);

                    // Create a DataStream object for handling image stream data then add into DataStream list for later usage.
                    dataStreamList.Register(device.CreateStDataStream(0));

                    IStDeviceInfo deviceInfo = device.GetIStDeviceInfo();

                    for (int index = 0; index < deviceSerialNumberList.Count; index++)
                    {
                        if (deviceInfo.SerialNumber == deviceSerialNumberList[index])
                        {
                            components[index].DataStreamIndex = i;
                            components[index].FullName = deviceInfo.DisplayName;
                            components[index].Name = deviceInfo.SerialNumber;
                            components[index].Mac = deviceInfo.ID;
                            components[index].DeviceInfo = deviceInfo;

                            components[index].CameraConfigInitial();
                            components[index].CameraConfigLoad();

                            uint uindex = Convert.ToUInt32(index);
                            Camera_SetParameter(uindex, "AcquisitionFrameRate", components[index].FPS);
                            Camera_SetParameter(uindex, "Gain", components[index].Gain);
                            Camera_SetParameter(uindex, "ExposureTime", components[index].Exposure);

                            break;
                        }
                    }
                }
            }
#endif

#if Edition_002
            // Create a DataStream list object to store all the data stream object related to the cameras.           
            dataStreamList = new CStDataStreamArray();

            CStDevice device = null;

            for (UInt16 i = 0; ; i++)
            {
                try
                {
                    // Create a camera device object and connect to first detected device.
                    device = system.CreateFirstStDevice();
                }
                catch
                {
                    if (dataStreamList.GetSize() == 0)
                    {
                        Console.WriteLine("Device not found.");
                        return;
                    }

                    break;
                }

                // Add the camera into device object list for later usage.
                deviceList.Register(device);

                // Displays the DisplayName of the device.
                Console.WriteLine("Device" + deviceList.GetSize() + "=" + device.GetIStDeviceInfo().DisplayName);

                // Create a DataStream object for handling image stream data then add into DataStream list for later usage.
                dataStreamList.Register(device.CreateStDataStream(0));

                IStDeviceInfo deviceInfo = device.GetIStDeviceInfo();

                for (int index = 0; index < deviceSerialNumberList.Count; index++)
                {
                    if (deviceInfo.SerialNumber == deviceSerialNumberList[index])
                    {
                        components[index].DataStreamIndex = i;
                        components[index].FullName = deviceInfo.DisplayName;
                        components[index].Name = deviceInfo.SerialNumber;
                        components[index].Mac = deviceInfo.ID;
                        components[index].DeviceInfo = deviceInfo;

                        components[index].CameraConfigInitial();
                        components[index].CameraConfigLoad();

                        uint uindex = Convert.ToUInt32(index);
                        Camera_SetParameter(uindex, "AcquisitionFrameRate", components[index].FPS);
                        Camera_SetParameter(uindex, "Gain", components[index].Gain);
                        Camera_SetParameter(uindex, "ExposureTime", components[index].Exposure);

                        break;
                    }
                }
            }
#endif
        }

        public void CameraDeviceClose()
        {
            for (int i = 0; i < nCountOfCameraDevice; i++)
            {
                CameraStop(i);
            }
        }

        public void CameraStop(int deviceNumber)
        {
            if (components[deviceNumber].DataStreamIndex < 0)
            {
                Console.WriteLine("Camera #" + deviceNumber + "device not found.");
                return;
            }

            uint deviceIndex = (uint)components[deviceNumber].DataStreamIndex;

            using (dataStream = deviceList[deviceIndex].CreateStDataStream(0))
            {
                // Stop the image acquisition of the camera side.
                deviceList[deviceIndex].AcquisitionStop();

                // Stop the image acquisition of the host side.
                dataStream.StopAcquisition();
            }
        }

        public void Camera_Grab(uint deviceNumber, UInt64 grab_count)
        {
            if (components[deviceNumber].DataStreamIndex < 0)
            {
                Console.WriteLine("Camera " + deviceNumber + " device not found.");
                return;
            }

            uint deviceIndex = (uint)components[deviceNumber].DataStreamIndex;

            //if (deviceIndex > deviceList.GetSize()) return;

            using (dataStream = deviceList[deviceIndex].CreateStDataStream(0))
            {
                // Displays the DisplayName of the device.
                Console.Write("Grab Information : Device = " + deviceList[deviceIndex].GetIStDeviceInfo().DisplayName + " , ");

                // Start the image acquisition of the host (local machine) side.
                dataStream.StartAcquisition(grab_count);

                // Start the image acquisition of the camera side.
                deviceList[deviceIndex].AcquisitionStart();

                // A while loop for acquiring data and checking status. 
                // Here, the acquisition runs until it reaches the assigned numbers of frames.
                while (dataStream.IsGrabbing)
                {
                    // Retrieve the buffer of image data with a timeout of 5000ms.
                    // Use the 'using' statement for automatically managing the buffer re-queue action when it's no longer needed.
                    using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(5000))
                    {
                        // Check if the acquired data contains image data.
                        if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                        {
                            // If yes, we create a IStImage object for further image handling.
                            IStImage stImage = streamBuffer.GetIStImage();

                            // Display the information of the acquired image data.
                            byte[] imageData = stImage.GetByteArray();
                            Type imgType = stImage.GetType();

                            Console.Write("Block ID = " + streamBuffer.GetIStStreamBufferInfo().FrameID + " , ");
                            Console.Write("Size = " + stImage.ImageWidth + " x " + stImage.ImageHeight + " , ");
                            Console.Write("First Byte = " + imageData[0] + Environment.NewLine);

                            //components[deviceNumber].GrabImage.Bytes = imageData;

                            CImageData ConvertToBitmap = new CImageData();
                            ConvertToBitmap.CreateBitmap(stImage);
                            components[deviceNumber].OneBitmap = ConvertToBitmap.m_Bitmap;
                            components[deviceNumber].GrabImage = new Image<Bgr, Byte>(components[deviceNumber].OneBitmap);
                        }
                        else
                        {
                            // If the acquired data contains no image data.
                            Console.WriteLine("Image Data Does Not Exist.");
                        }
                    }
                }

                // Stop the image acquisition of the camera side.
                deviceList[deviceIndex].AcquisitionStop();

                // Stop the image acquisition of the host side.
                dataStream.StopAcquisition();
            }
        }

        public void Camera_ContinueGrab()
        {
#if Edition_001
            Parallel.For
            (0, nCountOfCameraDevice, i =>//nCountOfCameraDevice
            {
                try
                {
                    uint deviceNumber = Convert.ToUInt32(i);
                    int receivedFrame = 0;

                    //lock (ContinueGrabLocked)
                    {
                        if (components[deviceNumber].DataStreamIndex < 0)
                        {
                            Console.WriteLine("Camera " + deviceNumber + "device not found.");
                            return;
                        }

                        uint deviceIndex = (uint)components[deviceNumber].DataStreamIndex;

                        using (dataStream = deviceList[deviceIndex].CreateStDataStream(0))
                        {
                            // Start the image acquisition of the host (local machine) side.
                            dataStream.StartAcquisition(0xFFFFFFFFFFFFFFFFUL);

                            // Start the image acquisition of the camera side.
                            deviceList[deviceIndex].AcquisitionStart();

                            // A while loop for acquiring data and checking status. 
                            // Here, the acquisition runs until it reaches the assigned numbers of frames.
                            while (dataStream.IsGrabbing)
                            {
                                Console.WriteLine(i);

                                if (IsConverting[i])
                                {
                                    Console.WriteLine("A" + i);

                                    // Retrieve the buffer of image data with a timeout of 5000ms.
                                    // Use the 'using' statement for automatically managing the buffer re-queue action when it's no longer needed.
                                    using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(5000))
                                    {
                                        Console.WriteLine("B");

                                        // Check if the acquired data contains image data.
                                        if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                                        {
                                            //Count Frame.
                                            receivedFrame++;

                                            Console.WriteLine("C");

                                            // If yes, we create a IStImage object for further image handling.
                                            IStImage stImage = streamBuffer.GetIStImage();

                                            // Display the information of the acquired image data.
                                            byte[] imageData = stImage.GetByteArray();
                                            Type imgType = stImage.GetType();

                                            if (IsConverting[i])
                                            {
                                                //Set Falg,
                                                IsFinished[i] = false;

                                                Console.WriteLine("D");

                                                //Convert Image Data.
                                                CImageData ConvertToBitmap = new CImageData();
                                                ConvertToBitmap.CreateBitmap(stImage);
                                                components[deviceNumber].OneBitmap = ConvertToBitmap.m_Bitmap;
                                                components[deviceNumber].GrabImage = new Image<Bgr, Byte>(components[deviceNumber].OneBitmap);

                                                //Delegate.
                                                ContinueGrabEvent.Invoke(deviceNumber);

                                                //Print Out.
                                                Console.Write("Device=" + deviceList[deviceIndex].GetIStDeviceInfo().DisplayName);
                                                Console.Write(", Frame=" + receivedFrame);
                                                Console.Write(", BlockId=" + streamBuffer.GetIStStreamBufferInfo().FrameID);
                                                Console.Write(", Size=" + stImage.ImageWidth + " x " + stImage.ImageHeight);
                                                Console.Write(", First byte=" + imageData[0] + Environment.NewLine);

                                                //Set Falg.
                                                IsConverting[i] = false;
                                                IsFinished[i] = true;
                                            }
                                        }
                                        else
                                        {
                                            // If the acquired data contains no image data.
                                            Console.WriteLine("Image data does not exist.");
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Error Occured In StCamer.Camera_ContinueGrab(). Message : " + Ex.Message);
                }
            }
            );          
#endif

#if Edition_002
            try
            {
                dataStreamList.StartAcquisition(0xFFFFFFFFFFFFFFFFUL);
                deviceList.AcquisitionStart();

                while (dataStreamList.IsGrabbingAny)
                {
                    lock (ContinueGrabLocked)
                    {
                        using (CStStreamBuffer streamBuffer = dataStreamList.RetrieveBuffer(5000))
                        {
                            if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                            {
                                IStImage stImage = streamBuffer.GetIStImage();

                                byte[] imageData = stImage.GetByteArray();
                                Type imgType = stImage.GetType();

                                for (uint index = 0; index < deviceSerialNumberList.Count; index++)
                                {
                                    if (IsConverting[index] || IsRecording[index])
                                    {
                                        if (components[index].FullName.Equals(streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName))
                                        {
                                            //Set Falg,
                                            IsFinished[index] = false;

                                            //Convert Image Data.
                                            CImageData ConvertToBitmap = new CImageData();
                                            ConvertToBitmap.CreateBitmap(stImage);
                                            components[index].OneBitmap = ConvertToBitmap.m_Bitmap;
                                            components[index].GrabImage = new Image<Bgr, Byte>(components[index].OneBitmap);

                                            //Delegate.
                                            ContinueGrabEvent.Invoke(index);
 
                                            //Print Out.
                                            Console.Write("Device=" + deviceList[index].GetIStDeviceInfo().DisplayName);
                                            Console.Write(", BlockId=" + streamBuffer.GetIStStreamBufferInfo().FrameID);
                                            Console.Write(", Size=" + stImage.ImageWidth + "x" + stImage.ImageHeight);
                                            Console.Write(", First byte=" + imageData[index] + Environment.NewLine);

                                            //Set Falg.
                                            IsConverting[index] = false;
                                            IsFinished[index] = true;
                                        }
                                        else
                                        {
                                            //Set Falg.
                                            IsConverting[index] = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // If the acquired data contains no image data.
                                LogWritter.MsgGenLog("Image Data From Camera(" + streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName + ") Does Not Exist."); 
                                Console.WriteLine("Image Data From Camera(" + streamBuffer.GetIStDataStream().GetIStDevice().GetIStDeviceInfo().DisplayName + ") Does Not Exist.");
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                LogWritter.MsgError("Error Occured In StCamer.Camera_ContinueGrab(). Message : " + Ex.Message);
            }
#endif
        }

        public double Camera_GetParameter(uint DeviceNumber, string ParameterName)
        {
            //ParameterName = "Gain", "AcquisitionFrameRate", "ExposureTime".

            try
            {
                uint DeviceIndex = (uint)components[DeviceNumber].DataStreamIndex;

                // Get the INodeMap interface for the camera settings
                INodeMap nodeMap = deviceList[DeviceIndex].GetRemoteIStPort().GetINodeMap();

                // Get the FloatNode interface.
                FloatNode floatNode = nodeMap.GetNode<FloatNode>(ParameterName);

                return floatNode.Value;
            }
            catch
            {
                Console.WriteLine("Error Occured When Got Parameter of " + ParameterName.ToString());
            }

            return 0;
        }

        public void Camera_SetParameter(uint DeviceNumber, string ParameterName, double ParameterValue)
        {
            //ParameterName = "Gain", "AcquisitionFrameRate", "ExposureTime".

            try
            {
                uint DeviceIndex = (uint)components[DeviceNumber].DataStreamIndex;

                // Get the INodeMap interface for the camera settings
                INodeMap nodeMap = deviceList[DeviceIndex].GetRemoteIStPort().GetINodeMap();

                // Get the FloatNode interface.
                FloatNode floatNode = nodeMap.GetNode<FloatNode>(ParameterName);

                // Update the settings using the FloatNode interface.
                floatNode.Value = ParameterValue;

                if (ParameterName.Equals("Gain"))
                    components[DeviceNumber].Gain = Convert.ToInt32(ParameterValue);
                if (ParameterName.Equals("AcquisitionFrameRate"))
                    components[DeviceNumber].FPS = Convert.ToInt32(ParameterValue);
                if (ParameterName.Equals("ExposureTime"))
                    components[DeviceNumber].Exposure = Convert.ToInt32(ParameterValue);

                Console.WriteLine(ParameterName.ToString() + " Set As " + ParameterValue.ToString() + " Successfully.");
            }
            catch
            {
                Console.WriteLine(ParameterName.ToString() + " Set As " + ParameterValue.ToString() + " UnSuccessfully.");
            }
        }

        public void Camera_SetParameter(uint DeviceNumber, string ParameterName, string ParameterValue)
        {
            //ParameterName = "TriggerMode"; ParameterValue = "On" or "Off".
            //ParameterName = "TriggerSource"; ParameterValue = "Software" or "Line0".

            try
            {
                uint DeviceIndex = (uint)components[DeviceNumber].DataStreamIndex;

                // Get the INodeMap interface for the camera settings.
                INodeMap nodeMap = deviceList[DeviceIndex].GetRemoteIStPort().GetINodeMap();

                // Get the IEnum interface.
                IEnum enumNode = nodeMap.GetNode<IEnum>(ParameterName);

                // Update the settings using the IEnum interface.
                enumNode.StringValue = ParameterValue;

                Console.WriteLine(ParameterName.ToString() + " Set As " + ParameterValue.ToString() + " Successfully.");
            }
            catch
            {
                Console.WriteLine(ParameterName.ToString() + " Set As " + ParameterValue.ToString() + " UnSuccessfully.");
            }
        }

        private static ImageSource CVImageToImageSource(Image<Gray, byte> image)
        {
            System.Drawing.Bitmap source = image.Bitmap;

            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                ptr = IntPtr.Zero;
                //DeleteObject(ptr);
                return bs;
            }
        }
    }

    public class CImageData : IDisposable
    {
        public Bitmap m_Bitmap = null;
        public CStPixelFormatConverter m_Converter = null;

        public Bitmap CreateBitmap(IStImage stImage)
        {
            if (m_Converter == null)
            {
                m_Converter = new CStPixelFormatConverter();
            }

            bool isColor = CStApiDotNet.GetIStPixelFormatInfo(stImage.ImagePixelFormat).IsColor;

            if (isColor)
            {
                // Convert the image data to BGR8 format.
                m_Converter.DestinationPixelFormat = eStPixelFormatNamingConvention.BGR8;
            }
            else
            {
                // Convert the image data to Mono8 format.
                m_Converter.DestinationPixelFormat = eStPixelFormatNamingConvention.Mono8;
            }

            if (m_Bitmap != null)
            {
                if ((m_Bitmap.Width != (int)stImage.ImageWidth) || (m_Bitmap.Height != (int)stImage.ImageHeight))
                {
                    m_Bitmap.Dispose();
                    m_Bitmap = null;
                }
            }

            if (m_Bitmap == null)
            {
                if (isColor)
                {
                    m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                }
                else
                {
                    m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                    ColorPalette palette = m_Bitmap.Palette;
                    for (int i = 0; i < 256; ++i) palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                    m_Bitmap.Palette = palette;
                }
            }

            using (CStImageBuffer imageBuffer = CStApiDotNet.CreateStImageBuffer())
            {
                m_Converter.Convert(stImage, imageBuffer);

                // Lock the bits of the bitmap.
                BitmapData bmpData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.WriteOnly, m_Bitmap.PixelFormat);

                // Place the pointer to the buffer of the bitmap.
                IntPtr ptrBmp = bmpData.Scan0;
                byte[] imageData = imageBuffer.GetIStImage().GetByteArray();
                Marshal.Copy(imageData, 0, ptrBmp, imageData.Length);
                m_Bitmap.UnlockBits(bmpData);
            }

            return m_Bitmap;
        }

        public void Dispose()
        {
            if (m_Bitmap != null)
            {
                m_Bitmap.Dispose();
                m_Bitmap = null;
            }

            if (m_Converter != null)
            {
                m_Converter.Dispose();
                m_Converter = null;
            }
        }
    }
}
