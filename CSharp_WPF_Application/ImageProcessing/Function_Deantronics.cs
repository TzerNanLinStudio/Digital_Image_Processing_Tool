using System;
using System.Numerics;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV.Flann;
using Emgu.CV.OCR;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Config_sharp;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace ImageProcessing
{
    public enum MinorStation
    {
        Fisrt = 1,
        Second = 2,
        Third = 3,
        Fourth = 4
    }

    public enum SizeFormat
    {
        Bytes,
        KiloBytes,
        MegaBytes,
        GigaBytes
    }

    public class D_Image
    {
        public static int Width;
        public static int Height;

        public D_Image()
        {
            Width = 2592;
            Height = 1944;
        }
    }

    public class D_FirstStation
    {
        #region "Declare And Delegate"
        public delegate void ResultHandler();
        public event ResultHandler ResultHandlerEvent;

        public delegate void LogHandler(string m_Mode, string m_Word);
        public event LogHandler LogHandlerEvent;

        // Variables for Config
        // ---------------------------------------------------------------
        public CustomConfig_IP IP_Config;
        private string m_RecipeDirectoryPath = System.Environment.CurrentDirectory + @"\Appendix\Config\";
        private string m_RecipeFilename = "ImageProcessing_K1";
        private string m_RecipeSubtitle = ".dat";
        private string m_RecipeFullPath;
        // ---------------------------------------------------------------

        // Variables for Image Record
        // ---------------------------------------------------------------
        DirectoryInfo FilePath;
        public bool ImageFile_OriginFlag = true;
        public bool ImageFile_ResultFlag = true;
        public int ImageFile_Limit = 50;
        public int ImageFile_Rate = 50;
        public int[] ImageFile_CurrentCount = new int[2] { 0, 0 };
        public int[] ImageFile_LimitCount = new int[2] { 100, 100 };
        private string ImageFile_FullPath_Origin = "";
        private string ImageFile_FullPath_Result = "";
        private string ImageFile_FullPath_All = System.Environment.CurrentDirectory + @"\Appendix\Image\Record\K1\";
        private string ImageFile_RootPath_Origin = System.Environment.CurrentDirectory + @"\Appendix\Image\Record\K1\Origin\";
        private string ImageFile_RootPath_Result = System.Environment.CurrentDirectory + @"\Appendix\Image\Record\K1\Result\";
        // ---------------------------------------------------------------

        // Variables for ImageRecorder 
        // ---------------------------------------------------------------
        private string CSV_Result_RootPath = System.Environment.CurrentDirectory + @"\Appendix\CSV\K1\Result";
        private string CSV_Parameter_RootPath = System.Environment.CurrentDirectory + @"\Appendix\CSV\K1\Parameter";
        private string CSV_Result_FullPath = "";
        private string CSV_Parameter_FullPath = "";
        // ---------------------------------------------------------------

        // Variables for Date And Time.
        // --------------------------------------------------------------- 
        public string PresentDateTime;
        public string PresentDate;
        public string PresentTime;
        // --------------------------------------------------------------- 

        // Variables for Image Processing
        // ---------------------------------------------------------------
        public readonly object ParameterLocked = new object();
        public bool[] AdjustmentFlag;
        public bool[] InspectionItemFlag; // true : Do Inspection ; false : Don't Do Inspection
        public int[] InspectionResultFlag; // -1 : Initial ; 0 : OK ; 1 : NG
        public int[] InspectionResultValue;
        public int[] Item01_PreviousValue;
        public const int InspectionItem = 4;
        public double Resolution = 2.75;

        public string Barcode = "";
        public int ModelNumber = 0; // Blue Model Is Preinstall.
        public int TriggerCount = 0;
        public int SerialNumber = 0;
        public int Item01_ProcessAngle = 0;
        public int Item01_OffsetAngle = 0;
        public int Item01_NewAngle = 0;
        public int Item01_AllAngle = 0;
        public const int Item02_LineHeight = 660;
        public float Item02_AreaRate = 0;
        
        public Rectangle RectAdjust;
        public double CurrentSharpValue = 0;
        public double CurrentLightValue = 0;
        public double IdealSharpValue = double.MaxValue;
        public double IdealLightValue = 150.0;
        // ---------------------------------------------------------------

        // Variables for Image
        // ---------------------------------------------------------------
        private static int CameraImageWidth = 2592;
        private static int CameraImageHeight = 1944;

        private Image<Bgr, byte> ImgSource;
        private Image<Bgr, byte> ImgROI;
        private Image<Gray, byte> ImgThreshold;
        private Image<Bgr, byte> ImgCanvas;
        private Image<Bgr, byte> ImgRecord;
        private Image<Bgr, byte> ImgResult;
        private Image<Bgr, byte> ImgAdjust;
        private Image<Bgr, byte> ImgNull;
        // ---------------------------------------------------------------

        // Variables for Thread
        // ---------------------------------------------------------------
        public bool DoInspectionFlag = false;
        // ---------------------------------------------------------------

        // Variables for Debug
        // ---------------------------------------------------------------
        public bool DebugFlag = false;
        public bool[] PrintFlag = new bool[5] { false, false, false, false, false };
        // ---------------------------------------------------------------

        // Variables for Test
        // ---------------------------------------------------------------

        // ---------------------------------------------------------------
        #endregion

        public D_FirstStation()
        {
            AdjustmentFlag = new bool[InspectionItem];
            for (int index = 0; index < InspectionItem; index++)
                AdjustmentFlag[index] = false;

            InspectionItemFlag = new bool[InspectionItem];
            for (int index = 0; index < InspectionItem; index++)
                InspectionItemFlag[index] = false;

            InspectionResultFlag = new int[InspectionItem];
            for (int index = 0; index < InspectionItem; index++)
                InspectionResultFlag[index] = 0;

            InspectionResultValue = new int[InspectionItem];
            for (int index = 0; index < InspectionItem; index++)
                InspectionResultValue[index] = 0;

            Item01_PreviousValue = new int[100];
            for (int index = 0; index < 100; index++)
                Item01_PreviousValue[index] = 0;

            RectAdjust = new System.Drawing.Rectangle(0, 0, 100, 100);

            ImgSource = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
            ImgROI = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
            ImgThreshold = new Image<Gray, byte>(CameraImageWidth, CameraImageHeight);
            ImgCanvas = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
            ImgRecord = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
            ImgResult = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
            ImgAdjust = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
            ImgNull = new Image<Bgr, byte>(CameraImageWidth, CameraImageHeight);
        }

        ~D_FirstStation()
        {

        }

        public void ImageFile_Init()
        {
            #region "Edition 02"
            if (true)
            {
                //Paramete.
                // ---------------------------------------------------------------
                int t_FileNumber = 0;
                PresentDate = TimeFormat(3, DateTime.Now);
                //ImageFile_Limit = IP_Config[ModelNumber].parameters.NumberOfImageInFile;
                // ---------------------------------------------------------------

                //Origin Image.
                // ---------------------------------------------------------------
                FilePath = new DirectoryInfo(ImageFile_RootPath_Origin + PresentDate + "(" + Barcode + ")");
                if (!FilePath.Exists)
                {
                    FilePath.Create();
                }

                t_FileNumber = FilePath.GetDirectories().Length;
                if (t_FileNumber <= 0)
                {
                    ImageFile_FullPath_Origin = ImageFile_RootPath_Origin + PresentDate + "(" + Barcode + ")" + "/" + (1).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                    FilePath = new DirectoryInfo(ImageFile_FullPath_Origin);
                    FilePath.Create();
                }
                else
                {
                    ImageFile_FullPath_Origin = ImageFile_RootPath_Origin + PresentDate + "(" + Barcode + ")" + "/" + (t_FileNumber).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                    FilePath = new DirectoryInfo(ImageFile_FullPath_Origin);
                    if (!FilePath.Exists)
                    {
                        ImageFile_FullPath_Origin = ImageFile_RootPath_Origin + PresentDate + "(" + Barcode + ")" + "/" + (t_FileNumber + 1).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                        FilePath = new DirectoryInfo(ImageFile_FullPath_Origin);
                        FilePath.Create();
                    }
                    else
                    {
                        if (FilePath.GetFiles().ToList().Count() >= ImageFile_Limit)
                        {
                            ImageFile_FullPath_Origin = ImageFile_RootPath_Origin + PresentDate + "(" + Barcode + ")" + "/" + (t_FileNumber + 1).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                            FilePath = new DirectoryInfo(ImageFile_FullPath_Origin);
                            FilePath.Create();
                        }
                    }
                }
                // ---------------------------------------------------------------

                //Result Image.
                // ---------------------------------------------------------------
                FilePath = new DirectoryInfo(ImageFile_RootPath_Result + PresentDate + "(" + Barcode + ")");
                if (!FilePath.Exists)
                {
                    FilePath.Create();
                }

                t_FileNumber = FilePath.GetDirectories().Length;
                if (t_FileNumber <= 0)
                {
                    ImageFile_FullPath_Result = ImageFile_RootPath_Result + PresentDate + "(" + Barcode + ")" + "/" + (1).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                    FilePath = new DirectoryInfo(ImageFile_FullPath_Result);
                    FilePath.Create();
                }
                else
                {
                    ImageFile_FullPath_Result = ImageFile_RootPath_Result + PresentDate + "(" + Barcode + ")" + "/" + (t_FileNumber).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                    FilePath = new DirectoryInfo(ImageFile_FullPath_Result);
                    if (!FilePath.Exists)
                    {
                        ImageFile_FullPath_Result = ImageFile_RootPath_Result + PresentDate + "(" + Barcode + ")" + "/" + (t_FileNumber + 1).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                        FilePath = new DirectoryInfo(ImageFile_FullPath_Result);
                        FilePath.Create();
                    }
                    else
                    {
                        if (FilePath.GetFiles().ToList().Count() >= ImageFile_Limit)
                        {
                            ImageFile_FullPath_Result = ImageFile_RootPath_Result + PresentDate + "(" + Barcode + ")" + "/" + (t_FileNumber + 1).ToString() + "(" + ImageFile_Limit.ToString() + ")";
                            FilePath = new DirectoryInfo(ImageFile_FullPath_Result);
                            FilePath.Create();
                        }
                    }
                }
                // ---------------------------------------------------------------
            }
            #endregion
        }

        public void ImageFile_Save()
        {
            #region "Edition 02"
            if (true)
            {
                ImageFile_Init();

                if (ImageFile_OriginFlag)
                    ImgSource.Save(ImageFile_FullPath_Origin + "/" + PresentTime + "(" + SerialNumber.ToString() + ")" + ".bmp");
                if (ImageFile_ResultFlag)
                    SaveJpeg(ImageFile_FullPath_Result + "/" + PresentTime + "(" + SerialNumber.ToString() + ")" + ".jpg", ImgResult.ToBitmap(), ImageFile_Rate);//ImgRecord.Save(ImageFile_FullPath_Result + "/" + PresentTime + "_" + SerialNumber.ToString() + ".jpg");
            }
            #endregion
        }

        public double ImageFile_Size(SizeFormat t_Format)
        {
            #region "Edition 01"
            if (true)
            {
                try
                {
                    DirectoryInfo t_Directory = new DirectoryInfo(ImageFile_FullPath_All);
                    double t_Size = DirectorySize(t_Directory);

                    switch (t_Format)
                    {
                        case SizeFormat.Bytes:
                            t_Size = t_Size / Math.Pow(1024, 0);
                            break;
                        case SizeFormat.KiloBytes:
                            t_Size = t_Size / Math.Pow(1024, 1);
                            break;
                        case SizeFormat.MegaBytes:
                            t_Size = t_Size / Math.Pow(1024, 2);
                            break;
                        case SizeFormat.GigaBytes:
                            t_Size = t_Size / Math.Pow(1024, 3);
                            break;
                    }

                    return t_Size;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Exception Occured When K1 Inspection Calculated Size Of Image File. Message: " + Ex.Message);

                    throw Ex;
                }
            }
            #endregion
        }

        public void ImageFile_Clear()
        {
            #region "Edition 01"
            if (true)
            {
                try
                {
                    DeleteSrcFolder_E01(ImageFile_RootPath_Origin);
                    DeleteSrcFolder_E01(ImageFile_RootPath_Result);
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Exception Occured When K1 Inspection Cleared Image File. Message: " + Ex.Message);
                }
            }
            #endregion
        }

        public void SetImage(int number, Image<Bgr, byte> image)
        {
            switch (number)
            {
                case 0:
                    ImgSource = image.Clone();
                    break;
                case 1:
                    ImgResult = image.Clone();
                    break;
                case 10:
                    //ImgROI = image.GetSubRect(IP_Config.parameters.ROI01).Clone();
                    break;
                default:
                    Console.WriteLine("Input Of Switch Was Not Suitable In D_FirstStation.SetImage().");
                    break;
            }
        }

        public Image<Bgr, byte> GetImage(int number)
        {
            switch (number)
            {
                case 0:
                    return ImgSource;
                case 1:
                    return ImgResult;
                case 2:
                    return ImgNull;
                case 3:
                    return ImgAdjust;
                case 10:
                    //ImgROI = ImgSource.GetSubRect(IP_Config.parameters.ROI01).Clone();
                    return ImgROI;
                default:
                    Console.WriteLine("Input Of Switch Was Not Suitable In D_FirstStation.GetImage().");
                    return ImgNull;
            }
        }

        //Inspection Loop.
        public void DoInspection()
        {
            try
            {
                while (true)
                {
                    if (DoInspectionFlag)
                    {
                        //Watch Start.
                        Stopwatch TestWatch = new Stopwatch();
                        TestWatch.Start();

                        //Clear Flag And Value.
                        for (int index = 0; index < InspectionItem; index++)
                        {
                            InspectionResultFlag[index] = 0;
                            InspectionResultValue[index] = 0;
                        }

                        //Get Present Date And Time.
                        PresentDateTime = TimeFormat(2, DateTime.Now);
                        PresentDate = TimeFormat(3, DateTime.Now);
                        PresentTime = TimeFormat(4, DateTime.Now);

                        //Record Image.
                        ImgResult = ImgSource.Clone();


                        //Inspection Item "1" : "三芯線轉線角度判別".
                        //Inspection Item "2" : "三芯線色差判別".
                        //Inspection Item "3" : "芯線夾取位置判別".
                        if (InspectionItemFlag[0] || InspectionItemFlag[1] || InspectionItemFlag[2])
                            Algorithm_AngleColorAddressOfLine_Check();

                        //Inspection Item "4" : "剝外被邊緣品質判別".
                        if (InspectionItemFlag[3])
                            Algorithm_AualityOfCLine_Check();

                        //Record Image. 
                        ImageFile_Save();

                        //Return Result.
                        ResultHandlerEvent.Invoke();

                        //Set Flag.
                        DoInspectionFlag = false;

                        //Watch Stop.
                        TestWatch.Stop();
                        UseTime("Runtime In K1 IP : ", TestWatch.Elapsed);
                    }

                    Thread.Sleep(5);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception In K1 Inspection Loop. Message: " + Ex.Message);
            }
        }

        //Inspection Item "1" : "三芯線轉線角度判別".
        //Inspection Item "2" : "三芯線色差判別".
        //Inspection Item "3" : "芯線夾取位置判別".
        public void Algorithm_AngleColorAddressOfLine_Check()
        {
            try
            {
                #region "Edition 02"
                if (true)
                {
                    #region "Parameter"
                    //Parameter
                    //----------------------------------------------------------------------------------------------------
                    string[] m_ErrorString = new string[3] { "", "", "" };
                    bool[] m_ColorFlag = new bool[3] { false, false, false };
                    bool[] m_DefectFlag = new bool[3] { false, false, false };
                    int[] m_DefectValue = new int[3] { 0, 0, 0 };
                    int[] m_PassAngle = new int[2] { IP_Config.parameters.Parameter01[9], IP_Config.parameters.Parameter01[10] };
                    int[,] m_ColorCount = new int[2, 3] { { 0, 0, 0 }, { 0, 0, 0 } }; //[0->1,] : Y-Axis->Area ; [,0->1->2] : Blue->White->Red.            
                    double m_RateOfArea = 0;
                    double[] m_LimitOfRate = new double[2] { 90.0, 110.0 };

                    System.Drawing.Rectangle m_RectROI = new System.Drawing.Rectangle(IP_Config.parameters.ROI01.X + IP_Config.parameters.ROI01.Width / 4, IP_Config.parameters.ROI01.Y, IP_Config.parameters.ROI01.Width / 2, IP_Config.parameters.ROI01.Height);
                    System.Drawing.Rectangle[] m_RectColor = new System.Drawing.Rectangle[3];
                    Image<Gray, byte>[] m_ImgColor = new Image<Gray, byte>[3];
                    Image<Gray, byte>[] m_ImgMorphology = new Image<Gray, byte>[3];
                    Image<Bgr, byte> m_ImgCanvas = new Image<Bgr, byte>(m_RectROI.Width, m_RectROI.Height);

                    ImgROI = ImgSource.GetSubRect(m_RectROI);
                    ImgCanvas = new Image<Bgr, byte>(ImgROI.Width * 2, ImgROI.Height); 
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Image Processing Algorithm"
                    //Step 1 Of Algorithm : Color Pixel.
                    //----------------------------------------------------------------------------------------------------
                    m_ImgColor[0] = new Image<Gray, byte>(ImgROI.Size); //For Blue.
                    m_ImgColor[1] = new Image<Gray, byte>(ImgROI.Size); //For White.
                    m_ImgColor[2] = new Image<Gray, byte>(ImgROI.Size); //For Red.

                    m_ImgMorphology[0] = new Image<Gray, byte>(ImgROI.Size); //For Blue.
                    m_ImgMorphology[1] = new Image<Gray, byte>(ImgROI.Size); //For White.
                    m_ImgMorphology[2] = new Image<Gray, byte>(ImgROI.Size); //For Red.

                    m_RectColor[0] = new System.Drawing.Rectangle(0, 0, 10, 10); //For Blue.
                    m_RectColor[1] = new System.Drawing.Rectangle(0, 0, 10, 10); //For White.
                    m_RectColor[2] = new System.Drawing.Rectangle(0, 0, 10, 10); //For Red.

                    Parallel.For
                    (0, ImgROI.Height, i =>
                    {
                        for (int j = 0; j < ImgROI.Width; j++)
                        {
                            if (ImgROI[i, j].Blue > IP_Config.parameters.Parameter01[3] && ImgROI[i, j].Red > IP_Config.parameters.Parameter01[4] && ImgROI[i, j].Green > IP_Config.parameters.Parameter01[5])
                            {
                                m_ImgColor[1].Data[i, j, 0] = 255;//white
                            }

                            if (ImgROI[i, j].Blue > IP_Config.parameters.Parameter01[0] && ImgROI[i, j].Blue - ImgROI[i, j].Red > IP_Config.parameters.Parameter01[1] && ImgROI[i, j].Red < IP_Config.parameters.Parameter01[2])
                            {
                                m_ImgColor[0].Data[i, j, 0] = 255;//blue
                            }

                            if (ImgROI[i, j].Red > IP_Config.parameters.Parameter01[6] && ImgROI[i, j].Red - ImgROI[i, j].Blue > IP_Config.parameters.Parameter01[7] && ImgROI[i, j].Blue < IP_Config.parameters.Parameter01[8])
                            {
                                m_ImgColor[2].Data[i, j, 0] = 255;//red
                            }
                        }
                    }
                    );
                    //----------------------------------------------------------------------------------------------------

                    //Step 2 Of Algorithm : Color Area.
                    //----------------------------------------------------------------------------------------------------
                    //Find The Biggest Area.
                    for (int i = 0; i < 3; i++)
                    {
                        m_ImgMorphology[i] = m_ImgColor[i].Dilate(1).Erode(1).Clone();

                        using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                        {
                            CvInvoke.FindContours(m_ImgMorphology[i], contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                            System.Drawing.Rectangle BoundingBox = new System.Drawing.Rectangle(0, 0, 10, 10);
                            int m_ContourSize = contours.Size;
                            int m_MaxArea = -1;
                            int m_MaxNumber = -1;

                            for (int j = 0; j < m_ContourSize; j++)
                            {
                                using (VectorOfPoint contour = contours[j])
                                {
                                    if (CvInvoke.ContourArea(contour) > m_MaxArea)
                                    {
                                        BoundingBox = CvInvoke.BoundingRectangle(contour);
                                        m_MaxArea = Convert.ToInt32(CvInvoke.ContourArea(contour));
                                        m_MaxNumber = j;
                                    }
                                }
                            }

                            if (m_MaxNumber >= 0 && m_MaxArea > 0)
                            {
                                if(BoundingBox.Width < ImgROI.Width || BoundingBox.Height < 20)
                                {
                                    m_ColorFlag[i] = false; 
                                }
                                else
                                {
                                    m_ColorFlag[i] = true;
                                    m_RectColor[i] = BoundingBox;
                                }
                            }
                            else
                            {
                                m_ColorFlag[i] = false;
                            }
                        }
                    }

                    //Check Whether Find Blue.
                    {
                        if (m_RectColor[0].Width < ImgROI.Width || m_RectColor[0].Height < 20 ||
                           (m_RectColor[0].Y > m_RectColor[1].Y && (m_RectColor[0].Y + m_RectColor[0].Height) < (m_RectColor[1].Y + m_RectColor[1].Height)) ||
                           (m_RectColor[0].Y > m_RectColor[2].Y && (m_RectColor[0].Y + m_RectColor[0].Height) < (m_RectColor[2].Y + m_RectColor[2].Height)))
                            m_ColorFlag[0] = false;
                        else
                            m_ColorFlag[0] = true;
                    }

                    //Check Whether Find White.
                    {
                        if (m_RectColor[1].Width < ImgROI.Width || m_RectColor[1].Height < 20 ||
                           (m_RectColor[1].Y > m_RectColor[0].Y && (m_RectColor[1].Y + m_RectColor[1].Height) < (m_RectColor[0].Y + m_RectColor[0].Height)) ||
                           (m_RectColor[1].Y > m_RectColor[2].Y && (m_RectColor[1].Y + m_RectColor[1].Height) < (m_RectColor[2].Y + m_RectColor[2].Height)))
                            m_ColorFlag[1] = false;
                        else
                            m_ColorFlag[1] = true;
                    }

                    //Check Whether Find Red.
                    {
                        if (m_RectColor[2].Width < ImgROI.Width || m_RectColor[2].Height < 20 ||
                           (m_RectColor[2].Y > m_RectColor[0].Y && (m_RectColor[2].Y + m_RectColor[2].Height) < (m_RectColor[0].Y + m_RectColor[0].Height)) ||
                           (m_RectColor[2].Y > m_RectColor[1].Y && (m_RectColor[2].Y + m_RectColor[2].Height) < (m_RectColor[1].Y + m_RectColor[1].Height)))
                            m_ColorFlag[2] = false;
                        else
                            m_ColorFlag[2] = true;
                    }
                    //----------------------------------------------------------------------------------------------------

                    //Step 3 Of Algorithm : Angle Calculation.
                    //----------------------------------------------------------------------------------------------------
                    if (m_ColorFlag[0] && !m_ColorFlag[1] && m_ColorFlag[2])//只有紅藍
                    {
                        if(m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2)//只有紅藍，紅上籃下
                        {
                            if (m_RectColor[0].Y == m_RectColor[2].Y + m_RectColor[2].Height)
                            {
                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                            else
                            {
                                int m_TempValue = (m_RectColor[0].Y + m_RectColor[2].Y + m_RectColor[2].Height) / 2;
                                m_RectColor[0].Y = m_TempValue;
                                m_RectColor[2].Height = m_TempValue - m_RectColor[2].Y;

                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                        }
                        else if (m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2)//只有紅藍，紅下籃上
                        {
                            if (m_RectColor[2].Y == m_RectColor[0].Y + m_RectColor[0].Height)
                            {
                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                            else
                            {
                                int m_TempValue = (m_RectColor[2].Y + m_RectColor[0].Y + m_RectColor[0].Height) / 2;
                                m_RectColor[2].Y = m_TempValue;
                                m_RectColor[0].Height = m_TempValue - m_RectColor[0].Y;

                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                        }
                        else//例外狀況
                        {
                            m_ErrorString[0] += "BR_01；";
                        }

                        if (m_ColorCount[0, 0] > m_ColorCount[0, 2] && m_ColorCount[1, 0] > m_ColorCount[1, 2])//只有紅藍，紅上籃下，紅偏小藍偏大，向上旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 2]), Convert.ToDouble(m_ColorCount[1, 0]))) * (1) + (120);
                        }
                        else if (m_ColorCount[0, 0] > m_ColorCount[0, 2] && m_ColorCount[1, 0] < m_ColorCount[1, 2])//只有紅藍，紅上籃下，紅偏大藍偏小，向下旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 0]), Convert.ToDouble(m_ColorCount[1, 2]))) * (-1) + (-120);
                        }
                        else if (m_ColorCount[0, 0] > m_ColorCount[0, 2] && m_ColorCount[1, 0] == m_ColorCount[1, 2])//只有紅藍，紅上籃下，紅藍相等，向上旋180度
                        {
                            m_DefectValue[0] = 180;
                        }
                        else if (m_ColorCount[0, 0] < m_ColorCount[0, 2] && m_ColorCount[1, 0] > m_ColorCount[1, 2])//只有紅藍，紅下籃上，紅偏小藍偏大，向下旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 2]), Convert.ToDouble(m_ColorCount[1, 0]))) * (-1) + (-120);
                        }
                        else if (m_ColorCount[0, 0] < m_ColorCount[0, 2] && m_ColorCount[1, 0] < m_ColorCount[1, 2])//只有紅藍，紅下籃上，紅偏大藍偏小，向上旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 0]), Convert.ToDouble(m_ColorCount[1, 2]))) * (1) + (120);
                        }
                        else if (m_ColorCount[0, 0] < m_ColorCount[0, 2] && m_ColorCount[1, 0] == m_ColorCount[1, 2])//只有紅藍，紅下籃上，紅藍相等，向上旋180度
                        {
                            m_DefectValue[0] = 180;
                        }
                        else//例外狀況
                        {
                            m_DefectValue[0] = 0;
                            m_ErrorString[0] += "BR_02；";
                        }
                    }
                    else if (!m_ColorFlag[0] && m_ColorFlag[1] && m_ColorFlag[2])//只有紅白
                    {
                        if (m_RectColor[1].Y + m_RectColor[1].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2)//只有紅白，紅上白下
                        {
                            if (m_RectColor[1].Y == m_RectColor[2].Y + m_RectColor[2].Height)
                            {
                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                            else
                            {
                                int m_TempValue = (m_RectColor[1].Y + m_RectColor[2].Y + m_RectColor[2].Height) / 2;
                                m_RectColor[1].Y = m_TempValue;
                                m_RectColor[2].Height = m_TempValue - m_RectColor[2].Y;

                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                        }
                        else if (m_RectColor[1].Y + m_RectColor[1].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2)//只有紅白，紅下白上
                        {
                            if (m_RectColor[2].Y == m_RectColor[1].Y + m_RectColor[1].Height)
                            {
                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                            else
                            {
                                int m_TempValue = (m_RectColor[2].Y + m_RectColor[1].Y + m_RectColor[1].Height) / 2;
                                m_RectColor[2].Y = m_TempValue;
                                m_RectColor[1].Height = m_TempValue - m_RectColor[1].Y;

                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 2] = m_RectColor[2].Y + m_RectColor[2].Height / 2;
                                m_ColorCount[1, 2] = m_RectColor[2].Height * m_RectColor[2].Width;
                            }
                        }
                        else//例外狀況
                        {
                            m_ErrorString[0] += "WR_01；";
                        }

                        if (m_ColorCount[0, 1] > m_ColorCount[0, 2] && m_ColorCount[1, 1] > m_ColorCount[1, 2])//只有紅白，紅上白下，紅偏小白偏大，向上旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 2]), Convert.ToDouble(m_ColorCount[1, 1]))) * (1);
                        }
                        else if (m_ColorCount[0, 1] > m_ColorCount[0, 2] && m_ColorCount[1, 1] < m_ColorCount[1, 2])//只有紅白，紅上白下，紅偏大白偏小，向上旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 1]), Convert.ToDouble(m_ColorCount[1, 2]))) * (-1) + (120);
                        }
                        else if (m_ColorCount[0, 1] > m_ColorCount[0, 2] && m_ColorCount[1, 1] == m_ColorCount[1, 2])//只有紅白，紅上白下，紅白相等，向上旋
                        {
                            m_DefectValue[0] = 60;
                        }
                        else if (m_ColorCount[0, 1] < m_ColorCount[0, 2] && m_ColorCount[1, 1] > m_ColorCount[1, 2])//只有紅白，紅下白上，紅偏小白偏大，向下旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 2]), Convert.ToDouble(m_ColorCount[1, 1]))) * (-1);
                        }
                        else if (m_ColorCount[0, 1] < m_ColorCount[0, 2] && m_ColorCount[1, 1] < m_ColorCount[1, 2])//只有紅白，紅下白上，紅偏大白偏小，向下旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 1]), Convert.ToDouble(m_ColorCount[1, 2]))) * (1) + (-120);
                        }
                        else if (m_ColorCount[0, 1] < m_ColorCount[0, 2] && m_ColorCount[1, 1] == m_ColorCount[1, 2])//只有紅白，紅下白上，紅白相等，向下旋
                        {
                            m_DefectValue[0] = -60;
                        }
                        else//例外狀況
                        {
                            m_DefectValue[0] = 0;
                            m_ErrorString[0] += "WR_02；";
                        }
                    }
                    else if (m_ColorFlag[0] && m_ColorFlag[1] && !m_ColorFlag[2])//只有藍白
                    {
                        if (m_RectColor[1].Y + m_RectColor[1].Height / 2 > m_RectColor[0].Y + m_RectColor[0].Height / 2)//只有藍白，藍上白下
                        {
                            if (m_RectColor[1].Y == m_RectColor[0].Y + m_RectColor[0].Height)
                            {
                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                            }
                            else
                            {
                                int m_TempValue = (m_RectColor[1].Y + m_RectColor[0].Y + m_RectColor[0].Height) / 2;
                                m_RectColor[1].Y = m_TempValue;
                                m_RectColor[0].Height = m_TempValue - m_RectColor[0].Y;

                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                            }
                        }
                        else if (m_RectColor[1].Y + m_RectColor[1].Height / 2 < m_RectColor[0].Y + m_RectColor[0].Height / 2)//只有藍白，藍下白上
                        {
                            if (m_RectColor[0].Y == m_RectColor[1].Y + m_RectColor[1].Height)
                            {
                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                            }
                            else
                            {
                                int m_TempValue = (m_RectColor[0].Y + m_RectColor[1].Y + m_RectColor[1].Height) / 2;
                                m_RectColor[0].Y = m_TempValue;
                                m_RectColor[1].Height = m_TempValue - m_RectColor[1].Y;

                                m_ColorCount[0, 1] = m_RectColor[1].Y + m_RectColor[1].Height / 2;
                                m_ColorCount[1, 1] = m_RectColor[1].Height * m_RectColor[1].Width;
                                m_ColorCount[0, 0] = m_RectColor[0].Y + m_RectColor[0].Height / 2;
                                m_ColorCount[1, 0] = m_RectColor[0].Height * m_RectColor[0].Width;
                            }
                        }
                        else//例外狀況
                        {
                            m_ErrorString[0] += "BW_01；";
                        }

                        if (m_ColorCount[0, 0] > m_ColorCount[0, 1] && m_ColorCount[1, 0] > m_ColorCount[1, 1])//只有藍白，藍下白上，白偏小藍偏大，向下旋75度
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 1]), Convert.ToDouble(m_ColorCount[1, 0]))) * (1) - 120;
                        }
                        else if (m_ColorCount[0, 0] > m_ColorCount[0, 1] && m_ColorCount[1, 0] < m_ColorCount[1, 1])//只有藍白，藍下白上，白偏大藍偏小，向下旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 0]), Convert.ToDouble(m_ColorCount[1, 1]))) * (-1);
                        }
                        else if (m_ColorCount[0, 0] > m_ColorCount[0, 1] && m_ColorCount[1, 0] == m_ColorCount[1, 1])//只有藍白，藍下白上，白藍相等，向下旋
                        {
                            m_DefectValue[0] = -60;
                        }
                        else if (m_ColorCount[0, 0] < m_ColorCount[0, 1] && m_ColorCount[1, 0] > m_ColorCount[1, 1])//只有藍白，藍上白下，白偏小藍偏大，向上旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 1]), Convert.ToDouble(m_ColorCount[1, 0]))) * (-1) + (120);
                        }
                        else if (m_ColorCount[0, 0] < m_ColorCount[0, 1] && m_ColorCount[1, 0] < m_ColorCount[1, 1])//只有藍白，藍上白下，白偏大藍偏小，向上旋
                        {
                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("TwoLine", Convert.ToDouble(m_ColorCount[1, 0]), Convert.ToDouble(m_ColorCount[1, 1]))) * (1);
                        }
                        else if (m_ColorCount[0, 0] < m_ColorCount[0, 1] && m_ColorCount[1, 0] == m_ColorCount[1, 1])//只有藍白，藍上白下，白藍相等，向上旋
                        {
                            m_DefectValue[0] = 60;
                        }
                        else//例外狀況
                        {
                            m_DefectValue[0] = 0;
                            m_ErrorString[0] += "BW_02；";
                        }
                    }
                    else if (m_ColorFlag[0] && m_ColorFlag[1] && m_ColorFlag[2])//白紅藍都有
                    {
                        if (m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[1].Y + m_RectColor[1].Height / 2 
                            && m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2
                            && m_RectColor[1].Y + m_RectColor[1].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2)//白紅藍都有，白在中藍在下紅在上
                        {
                            if (m_RectColor[1].Y != m_RectColor[2].Y + m_RectColor[2].Height)
                            {
                                int m_TempValue = (m_RectColor[1].Y + m_RectColor[2].Y + m_RectColor[2].Height) / 2;
                                m_RectColor[1].Y = m_TempValue;
                                m_RectColor[2].Height = m_TempValue - m_RectColor[2].Y;
                            }

                            if (m_RectColor[0].Y != m_RectColor[1].Y + m_RectColor[1].Height)
                            {
                                int m_TempValue = (m_RectColor[0].Y + m_RectColor[1].Y + m_RectColor[1].Height) / 2;
                                m_RectColor[0].Y = m_TempValue;
                                m_RectColor[1].Height = m_TempValue - m_RectColor[1].Y;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                m_ColorCount[0, i] = m_RectColor[i].Y + m_RectColor[i].Height / 2;
                                m_ColorCount[1, i] = m_RectColor[i].Height * m_RectColor[i].Width;
                            }

                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("SmallAngle", Convert.ToDouble(m_ColorCount[1, 0]), Convert.ToDouble(m_ColorCount[1, 2])));
                        }
                        else if (m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[1].Y + m_RectColor[1].Height / 2
                                && m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2
                                && m_RectColor[1].Y + m_RectColor[1].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2)//白紅藍都有，白在中藍在上紅在下
                        {
                            if (m_RectColor[1].Y != m_RectColor[0].Y + m_RectColor[0].Height)
                            {
                                int m_TempValue = (m_RectColor[1].Y + m_RectColor[0].Y + m_RectColor[0].Height) / 2;
                                m_RectColor[1].Y = m_TempValue;
                                m_RectColor[0].Height = m_TempValue - m_RectColor[0].Y;
                            }

                            if (m_RectColor[2].Y != m_RectColor[1].Y + m_RectColor[1].Height)
                            {
                                int m_TempValue = (m_RectColor[2].Y + m_RectColor[1].Y + m_RectColor[1].Height) / 2;
                                m_RectColor[2].Y = m_TempValue;
                                m_RectColor[1].Height = m_TempValue - m_RectColor[1].Y;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                m_ColorCount[0, i] = m_RectColor[i].Y + m_RectColor[i].Height / 2;
                                m_ColorCount[1, i] = m_RectColor[i].Height * m_RectColor[i].Width;
                            }

                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("SmallAngle", Convert.ToDouble(m_ColorCount[1, 2]), Convert.ToDouble(m_ColorCount[1, 0])));
                        }
                        else if (m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[1].Y + m_RectColor[1].Height / 2
                                && m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2
                                && m_RectColor[1].Y + m_RectColor[1].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2)//白紅藍都有，白在上藍在下紅在中
                        {
                            if (m_RectColor[2].Y != m_RectColor[1].Y + m_RectColor[1].Height)
                            {
                                int m_TempValue = (m_RectColor[2].Y + m_RectColor[1].Y + m_RectColor[1].Height) / 2;
                                m_RectColor[2].Y = m_TempValue;
                                m_RectColor[1].Height = m_TempValue - m_RectColor[1].Y;
                            }

                            if (m_RectColor[0].Y != m_RectColor[2].Y + m_RectColor[2].Height)
                            {
                                int m_TempValue = (m_RectColor[0].Y + m_RectColor[2].Y + m_RectColor[2].Height) / 2;
                                m_RectColor[0].Y = m_TempValue;
                                m_RectColor[2].Height = m_TempValue - m_RectColor[2].Y;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                m_ColorCount[0, i] = m_RectColor[i].Y + m_RectColor[i].Height / 2;
                                m_ColorCount[1, i] = m_RectColor[i].Height * m_RectColor[i].Width;
                            }

                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("SmallAngle", Convert.ToDouble(m_ColorCount[1, 0]), Convert.ToDouble(m_ColorCount[1, 1]))) + (-120);
                        }
                        else if (m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[1].Y + m_RectColor[1].Height / 2
                                && m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2
                                && m_RectColor[1].Y + m_RectColor[1].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2)//白紅藍都有，白在上藍在中紅在下
                        {
                            if (m_RectColor[0].Y != m_RectColor[1].Y + m_RectColor[1].Height)
                            {
                                int m_TempValue = (m_RectColor[0].Y + m_RectColor[1].Y + m_RectColor[1].Height) / 2;
                                m_RectColor[0].Y = m_TempValue;
                                m_RectColor[1].Height = m_TempValue - m_RectColor[1].Y;
                            }

                            if (m_RectColor[2].Y != m_RectColor[0].Y + m_RectColor[0].Height)
                            {
                                int m_TempValue = (m_RectColor[2].Y + m_RectColor[0].Y + m_RectColor[0].Height) / 2;
                                m_RectColor[2].Y = m_TempValue;
                                m_RectColor[0].Height = m_TempValue - m_RectColor[0].Y;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                m_ColorCount[0, i] = m_RectColor[i].Y + m_RectColor[i].Height / 2;
                                m_ColorCount[1, i] = m_RectColor[i].Height * m_RectColor[i].Width;
                            }

                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("SmallAngle", Convert.ToDouble(m_ColorCount[1, 2]), Convert.ToDouble(m_ColorCount[1, 1]))) + (-120);
                        }
                        else if (m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[1].Y + m_RectColor[1].Height / 2
                                && m_RectColor[0].Y + m_RectColor[0].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2
                                && m_RectColor[1].Y + m_RectColor[1].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2)//白紅藍都有，白在下藍在中紅在上
                        {
                            if (m_RectColor[0].Y != m_RectColor[2].Y + m_RectColor[2].Height)
                            {
                                int m_TempValue = (m_RectColor[0].Y + m_RectColor[2].Y + m_RectColor[2].Height) / 2;
                                m_RectColor[0].Y = m_TempValue;
                                m_RectColor[2].Height = m_TempValue - m_RectColor[2].Y;
                            }

                            if (m_RectColor[1].Y != m_RectColor[0].Y + m_RectColor[0].Height)
                            {
                                int m_TempValue = (m_RectColor[1].Y + m_RectColor[0].Y + m_RectColor[0].Height) / 2;
                                m_RectColor[1].Y = m_TempValue;
                                m_RectColor[0].Height = m_TempValue - m_RectColor[0].Y;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                m_ColorCount[0, i] = m_RectColor[i].Y + m_RectColor[i].Height / 2;
                                m_ColorCount[1, i] = m_RectColor[i].Height * m_RectColor[i].Width;
                            }

                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("SmallAngle", Convert.ToDouble(m_ColorCount[1, 1]), Convert.ToDouble(m_ColorCount[1, 2]))) + (120);
                        }
                        else if (m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[1].Y + m_RectColor[1].Height / 2
                                && m_RectColor[0].Y + m_RectColor[0].Height / 2 < m_RectColor[2].Y + m_RectColor[2].Height / 2
                                && m_RectColor[1].Y + m_RectColor[1].Height / 2 > m_RectColor[2].Y + m_RectColor[2].Height / 2)//白紅藍都有，白在下藍在上紅在中
                        {
                            if (m_RectColor[2].Y != m_RectColor[0].Y + m_RectColor[0].Height)
                            {
                                int m_TempValue = (m_RectColor[2].Y + m_RectColor[0].Y + m_RectColor[0].Height) / 2;
                                m_RectColor[2].Y = m_TempValue;
                                m_RectColor[0].Height = m_TempValue - m_RectColor[0].Y;
                            }

                            if (m_RectColor[1].Y != m_RectColor[2].Y + m_RectColor[2].Height)
                            {
                                int m_TempValue = (m_RectColor[1].Y + m_RectColor[2].Y + m_RectColor[2].Height) / 2;
                                m_RectColor[1].Y = m_TempValue;
                                m_RectColor[2].Height = m_TempValue - m_RectColor[2].Y;
                            }

                            for (int i = 0; i < 3; i++)
                            {
                                m_ColorCount[0, i] = m_RectColor[i].Y + m_RectColor[i].Height / 2;
                                m_ColorCount[1, i] = m_RectColor[i].Height * m_RectColor[i].Width;
                            }

                            m_DefectValue[0] = Convert.ToInt32(CountSmallAngle("SmallAngle", Convert.ToDouble(m_ColorCount[1, 1]), Convert.ToDouble(m_ColorCount[1, 0]))) + (120);
                        }
                        else//例外狀況
                        {
                            m_DefectValue[0] = 0;
                            m_ErrorString[0] += "BWR_01；";
                        }
                    }
                    else
                    {
                        m_DefectValue[0] = 0;
                        m_ErrorString[0] += "NBNWNR_01；";
                    }
                    //----------------------------------------------------------------------------------------------------

                    //Step 4 Of Algorithm : Logic Judgement.
                    //----------------------------------------------------------------------------------------------------
                    if (m_DefectValue[0] <= m_PassAngle[1] && m_DefectValue[0] >= m_PassAngle[0])
                    {
                        Item01_PreviousValue[TriggerCount - 1] = m_DefectValue[0];
                        m_DefectFlag[0] = false;
                    }
                    else
                    {
                        Item01_PreviousValue[TriggerCount - 1] = m_DefectValue[0];
                        m_DefectFlag[0] = true;
                    }
                    //----------------------------------------------------------------------------------------------------

                    //Step 5 Of Algorithm : Addition Of Offset.
                    //----------------------------------------------------------------------------------------------------
                    {
                        Item01_ProcessAngle = m_DefectValue[0];
                        if (TriggerCount == 1)
                            Item01_AllAngle = 0;

                        if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] >= 0 && m_DefectValue[0] <= 29)//All Degree : ±(0 ~ 29); New Degree : 0 ~ 29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][0][0];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] >= 30 && m_DefectValue[0] <= 59)//All Degree : ±(0 ~ 29); New Degree : 30 ~ 59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][0][1];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] >= 60 && m_DefectValue[0] <= 89)//All Degree : ±(0 ~ 29); New Degree : 60 ~ 89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][0][2];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] >= 90 && m_DefectValue[0] <= 119)//All Degree : ±(0 ~ 29); New Degree : 90 ~ 119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][0][3];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] >= 120 && m_DefectValue[0] <= 149)//All Degree : ±(0 ~ 29); New Degree : 120 ~ 149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][0][4];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] >= 150 && m_DefectValue[0] <= 180)//All Degree : ±(0 ~ 29); New Degree : 150 ~ 180.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][0][5];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] >= 0 && m_DefectValue[0] <= 29)//All Degree : ±(30 ~ 59); New Degree :  0 ~ 29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][1][0];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] >= 30 && m_DefectValue[0] <= 59)//All Degree : ±(30 ~ 59); New Degree : 30 ~ 59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][1][1];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] >= 60 && m_DefectValue[0] <= 89)//All Degree : ±(30 ~ 59); New Degree : 60 ~ 89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][1][2];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] >= 90 && m_DefectValue[0] <= 119)//All Degree : ±(30 ~ 59); New Degree : 90 ~ 119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][1][3];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] >= 120 && m_DefectValue[0] <= 149)//All Degree : ±(30 ~ 59); New Degree : 120 ~ 149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][1][4];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] >= 150 && m_DefectValue[0] <= 180)//All Degree : ±(30 ~ 59); New Degree : 150 ~ 180.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][1][5];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] >= 0 && m_DefectValue[0] <= 29)//All Degree : ±(60 ~ 89); New Degree : 0 ~ 29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][2][0];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] >= 30 && m_DefectValue[0] <= 59)//All Degree : ±(60 ~ 89); New Degree : 30 ~ 59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][2][1];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] >= 60 && m_DefectValue[0] <= 89)//All Degree : ±(60 ~ 89); New Degree : 60 ~ 89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][2][2];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] >= 90 && m_DefectValue[0] <= 119)//All Degree : ±(60 ~ 89); New Degree : 90 ~ 119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][2][3];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] >= 120 && m_DefectValue[0] <= 149)//All Degree : ±(60 ~ 89); New Degree : 120 ~ 149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][2][4];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] >= 150 && m_DefectValue[0] <= 180)//All Degree : ±(60 ~ 89); New Degree : 150 ~ 180.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][2][5];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] >= 0 && m_DefectValue[0] <= 29)//All Degree :  ±(90 ~ 119); New Degree : 0 ~ 29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][3][0];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] >= 30 && m_DefectValue[0] <= 59)//All Degree : ±(90 ~ 119); New Degree : 30 ~ 59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][3][1];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] >= 60 && m_DefectValue[0] <= 89)//All Degree : ±(90 ~ 119); New Degree : 60 ~ 89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][3][2];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] >= 90 && m_DefectValue[0] <= 119)//All Degree : ±(90 ~ 119); New Degree : 90 ~ 119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][3][3];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] >= 120 && m_DefectValue[0] <= 149)//All Degree : ±(90 ~ 119); New Degree : 120 ~ 149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][3][4];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] >= 150 && m_DefectValue[0] <= 180)//All Degree : ±(90 ~ 119); New Degree : 150 ~ 180.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][3][5];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] >= 0 && m_DefectValue[0] <= 29)//All Degree : ±(120 ~ 149); New Degree : 0 ~ 29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][4][0];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] >= 30 && m_DefectValue[0] <= 59)//All Degree : ±(120 ~ 149); New Degree : 30 ~ 59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][4][1];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] >= 60 && m_DefectValue[0] <= 89)//All Degree : ±(120 ~ 149); New Degree : 60 ~ 89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][4][2];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] >= 90 && m_DefectValue[0] <= 119)//All Degree : ±(120 ~ 149); New Degree : 90 ~ 119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][4][3];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] >= 120 && m_DefectValue[0] <= 149)//All Degree : ±(120 ~ 149); New Degree : 120 ~ 149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][4][4];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] >= 150 && m_DefectValue[0] <= 180)//All Degree : ±(120 ~ 149); New Degree : 150 ~ 180.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][4][5];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] >= 0 && m_DefectValue[0] <= 29)//All Degree : ±(150 ~ ∞); New Degree : 0 ~ 29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][5][0];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] >= 30 && m_DefectValue[0] <= 59)//All Degree : ±(150 ~ ∞); New Degree : 30 ~ 59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][5][1];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] >= 60 && m_DefectValue[0] <= 89)//All Degree : ±(150 ~ ∞); New Degree : 60 ~ 89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][5][2];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] >= 90 && m_DefectValue[0] <= 119)//All Degree : ±(150 ~ ∞); New Degree : 90 ~ 119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][5][3];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] >= 120 && m_DefectValue[0] <= 149)//All Degree : ±(150 ~ ∞); New Degree : 120 ~ 149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][5][4];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] >= 150 && m_DefectValue[0] <= 180)//All Degree : ±(150 ~ ∞); New Degree : 150 ~ 180.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[0][5][5];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] <= -1 && m_DefectValue[0] >= -29)//All Degree : ±(0 ~ 29); New Degree : -1 ~ -29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][0][0];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] <= -30 && m_DefectValue[0] >= -59)//All Degree : ±(0 ~ 29); New Degree : -30 ~ -59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][0][1];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] <= -60 && m_DefectValue[0] >= -89)//All Degree : ±(0 ~ 29); New Degree : -60 ~ -89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][0][2];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] <= -90 && m_DefectValue[0] >= -119)//All Degree : ±(0 ~ 29); New Degree : -90 ~ -119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][0][3];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] <= -120 && m_DefectValue[0] >= -149)//All Degree : ±(0 ~ 29); New Degree : -120 ~ -149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][0][4];
                        else if (Item01_AllAngle >= -29 && Item01_AllAngle <= 29 && m_DefectValue[0] <= -150 && m_DefectValue[0] >= -179)//All Degree : ±(0 ~ 29); New Degree : -150 ~ -179.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][0][5];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] <= -1 && m_DefectValue[0] >= -29)//All Degree : ±(30 ~ 59); New Degree :  -1 ~ -29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][1][0];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] <= -30 && m_DefectValue[0] >= -59)//All Degree : ±(30 ~ 59); New Degree : -30 ~ -59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][1][1];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] <= -60 && m_DefectValue[0] >= -89)//All Degree : ±(30 ~ 59); New Degree : -60 ~ -89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][1][2];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] <= -90 && m_DefectValue[0] >= -119)//All Degree : ±(30 ~ 59); New Degree : -90 ~ -119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][1][3];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] <= -120 && m_DefectValue[0] >= -149)//All Degree : ±(30 ~ 59); New Degree : -120 ~ -149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][1][4];
                        else if (((Item01_AllAngle >= -59 && Item01_AllAngle <= -30) || (Item01_AllAngle >= 30 && Item01_AllAngle <= 59)) && m_DefectValue[0] <= -150 && m_DefectValue[0] >= -179)//All Degree : ±(30 ~ 59); New Degree : -150 ~ -179.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][1][5];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] <= -1 && m_DefectValue[0] >= -29)//All Degree : ±(60 ~ 89); New Degree : -1 ~ -29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][2][0];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] <= -30 && m_DefectValue[0] >= -59)//All Degree : ±(60 ~ 89); New Degree : -30 ~ -59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][2][1];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] <= -60 && m_DefectValue[0] >= -89)//All Degree : ±(60 ~ 89); New Degree : -60 ~ -89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][2][2];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] <= -90 && m_DefectValue[0] >= -119)//All Degree : ±(60 ~ 89); New Degree : -90 ~ -119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][2][3];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] <= -120 && m_DefectValue[0] >= -149)//All Degree : ±(60 ~ 89); New Degree : -120 ~ -149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][2][4];
                        else if (((Item01_AllAngle >= -89 && Item01_AllAngle <= -60) || (Item01_AllAngle >= 60 && Item01_AllAngle <= 89)) && m_DefectValue[0] <= -150 && m_DefectValue[0] >= -179)//All Degree : ±(60 ~ 89); New Degree : -150 ~ -179.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][2][5];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] <= -1 && m_DefectValue[0] >= -29)//All Degree :  ±(90 ~ 119); New Degree : -1 ~ -29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][3][0];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] <= -30 && m_DefectValue[0] >= -59)//All Degree : ±(90 ~ 119); New Degree : -30 ~ -59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][3][1];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] <= -60 && m_DefectValue[0] >= -89)//All Degree : ±(90 ~ 119); New Degree : -60 ~ -89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][3][2];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] <= -90 && m_DefectValue[0] >= -119)//All Degree : ±(90 ~ 119); New Degree : -90 ~ -119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][3][3];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] <= -120 && m_DefectValue[0] >= -149)//All Degree : ±(90 ~ 119); New Degree : -120 ~ -149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][3][4];
                        else if (((Item01_AllAngle >= -119 && Item01_AllAngle <= -90) || (Item01_AllAngle >= 90 && Item01_AllAngle <= 119)) && m_DefectValue[0] <= -150 && m_DefectValue[0] >= -179)//All Degree : ±(90 ~ 119); New Degree : -150 ~ -179.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][3][5];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] <= -1 && m_DefectValue[0] >= -29)//All Degree : ±(120 ~ 149); New Degree : -1 ~ -29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][4][0];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] <= -30 && m_DefectValue[0] >= -59)//All Degree : ±(120 ~ 149); New Degree : -30 ~ -59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][4][1];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] <= -60 && m_DefectValue[0] >= -89)//All Degree : ±(120 ~ 149); New Degree : -60 ~ -89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][4][2];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] <= -90 && m_DefectValue[0] >= -119)//All Degree : ±(120 ~ 149); New Degree : -90 ~ -119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][4][3];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] <= -120 && m_DefectValue[0] >= -149)//All Degree : ±(120 ~ 149); New Degree : -120 ~ -149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][4][4];
                        else if (((Item01_AllAngle >= -149 && Item01_AllAngle <= -120) || (Item01_AllAngle >= 120 && Item01_AllAngle <= 149)) && m_DefectValue[0] <= -150 && m_DefectValue[0] >= -179)//All Degree : ±(120 ~ 149); New Degree : -150 ~ -179.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][4][5];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] <= -1 && m_DefectValue[0] >= -29)//All Degree : ±(150 ~ ∞); New Degree : -1 ~ -29.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][5][0];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] <= -30 && m_DefectValue[0] >= -59)//All Degree : ±(150 ~ ∞); New Degree :-30 ~ -59.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][5][1];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] <= -60 && m_DefectValue[0] >= -89)//All Degree : ±(150 ~ ∞); New Degree : -60 ~ -89.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][5][2];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] <= -90 && m_DefectValue[0] >= -119)//All Degree : ±(150 ~ ∞); New Degree : -90 ~ -119.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][5][3];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] <= -120 && m_DefectValue[0] >= -149)//All Degree : ±(150 ~ ∞); New Degree : -120 ~ -149.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][5][4];
                        else if ((Item01_AllAngle <= -150 || Item01_AllAngle >= 150) && m_DefectValue[0] <= -150 && m_DefectValue[0] >= -179)//All Degree : ±(150 ~ ∞); New Degree : -150 ~ -179.
                            Item01_OffsetAngle = IP_Config.parameters.Offset01[1][5][5];

                        Item01_AllAngle += m_DefectValue[0];

                        m_DefectValue[0] += Item01_OffsetAngle;
                        if (m_DefectValue[0] > 180)
                            m_DefectValue[0] -= 360;
                        else if (m_DefectValue[0] < -180)
                            m_DefectValue[0] += 360;

                        Item01_NewAngle = m_DefectValue[0];
                    }
                    //----------------------------------------------------------------------------------------------------

                    //Step 6 Of Algorithm : Color Judgement And Address Judgement.
                    //----------------------------------------------------------------------------------------------------
                    {
                        m_RateOfArea = (Convert.ToDouble(m_ColorCount[1, 0] + m_ColorCount[1, 1] + m_ColorCount[1, 2]) / (Convert.ToDouble(Item02_LineHeight) * ImgROI.Width)) * 100.0;

                        if (m_RateOfArea > m_LimitOfRate[0] && m_RateOfArea < m_LimitOfRate[1])
                        {
                            m_DefectFlag[1] = false;
                            m_DefectFlag[2] = false;
                        }
                        else
                        {
                            m_DefectFlag[1] = true;
                            m_DefectFlag[2] = true;
                        }

                        Item02_AreaRate = Convert.ToSingle(m_RateOfArea);
                    }
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Return"
                    //Set Result Value.
                    //----------------------------------------------------------------------------------------------------
                    for (int i = 0; i < 3; i++)
                    {
                        if (InspectionItemFlag[i])
                            if (m_DefectFlag[i])
                                InspectionResultFlag[i] = 1;
                            else
                                InspectionResultFlag[i] = 2;
                        else
                            InspectionResultFlag[i] = 0;
                    }

                    InspectionResultValue[0] = m_DefectValue[0];
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Record"
                    //Record Image.
                    //----------------------------------------------------------------------------------------------------
                    CvInvoke.PutText(ImgResult, "Trigger : ", new Point(0, Convert.ToInt32(ImgResult.Height * 0.86)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(255, 255, 255), 3);
                    CvInvoke.PutText(ImgResult, TriggerCount.ToString(), new Point(Convert.ToInt32(ImgResult.Width * 0.2), Convert.ToInt32(ImgResult.Height * 0.86)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(255, 255, 255), 3);
                    CvInvoke.PutText(ImgResult, "Area    : ", new Point(0, Convert.ToInt32(ImgResult.Height * 0.92)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(255, 255, 255), 3);
                    CvInvoke.PutText(ImgResult, "Degree  : ", new Point(0, Convert.ToInt32(ImgResult.Height * 0.98)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(255, 255, 255), 3);

                    if (InspectionResultFlag[0] == 1 || InspectionResultFlag[1] == 1 || InspectionResultFlag[2] == 1)
                    {
                        CvInvoke.Rectangle(ImgResult, IP_Config.parameters.ROI01, new MCvScalar(0, 0, 255, 255), 3);
                    }
                    else
                    {
                        CvInvoke.Rectangle(ImgResult, IP_Config.parameters.ROI01, new MCvScalar(0, 255, 0, 255), 3); 
                    }

                    if (InspectionItemFlag[0])
                    {
                        if (InspectionResultFlag[0] == 1)
                        {
                            CvInvoke.PutText(ImgResult, Item01_ProcessAngle.ToString() + " + " + Item01_OffsetAngle.ToString(), new Point(Convert.ToInt32(ImgResult.Width * 0.2), Convert.ToInt32(ImgResult.Height * 0.98)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(0, 0, 255), 3);
                        }
                        else
                        {
                            CvInvoke.PutText(ImgResult, Item01_ProcessAngle.ToString() + " + " + Item01_OffsetAngle.ToString(), new Point(Convert.ToInt32(ImgResult.Width * 0.2), Convert.ToInt32(ImgResult.Height * 0.98)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(0, 255, 0), 3);
                        }
                    }
                    else
                    {
                        
                    }

                    if (InspectionItemFlag[1])
                    {
                        if (InspectionResultFlag[1] == 1)
                        {
                            CvInvoke.PutText(ImgResult, Item02_AreaRate.ToString("F2") + " % ", new Point(Convert.ToInt32(ImgResult.Width * 0.2), Convert.ToInt32(ImgResult.Height * 0.92)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(0, 0, 255), 3);
                        }
                        else
                        {
                            CvInvoke.PutText(ImgResult, Item02_AreaRate.ToString("F2") + " % ", new Point(Convert.ToInt32(ImgResult.Width * 0.2), Convert.ToInt32(ImgResult.Height * 0.92)), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 4, new MCvScalar(0, 255, 0), 3);
                        }
                    }
                    else
                    {

                    }
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Debug"
                    //Save Process Image In File And Print Out Imformation For Debug.
                    //----------------------------------------------------------------------------------------------------
                    if (DebugFlag)
                    {
                        if(m_ColorFlag[0])
                            CvInvoke.Rectangle(m_ImgCanvas, m_RectColor[0], new MCvScalar(255, 0, 0, 255), -1);
                        if (m_ColorFlag[1])
                            CvInvoke.Rectangle(m_ImgCanvas, m_RectColor[1], new MCvScalar(255, 255, 255, 255), -1);
                        if (m_ColorFlag[2])
                            CvInvoke.Rectangle(m_ImgCanvas, m_RectColor[2], new MCvScalar(0, 0, 255, 255), -1);
                        CvInvoke.HConcat(ImgROI, m_ImgCanvas, ImgCanvas);

                        ImgSource.Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_Origin.bmp");
                        ImgROI.Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_ROI.bmp");
                        ImgCanvas.Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_Canvas.bmp");

                        m_ImgColor[0].Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_ColorB.bmp");
                        m_ImgColor[1].Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_ColorW.bmp");
                        m_ImgColor[2].Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_ColorR.bmp");
                        m_ImgCanvas.Save(System.Environment.CurrentDirectory + "/Appendix/Image/Debug/K1/" + SerialNumber.ToString() + "_SmallCanvas.bmp");

                        Console.WriteLine("-------------------------第1站三芯線轉線角度判別之參數(如下)-------------------------");
                        Console.WriteLine("Trigger Of [Item01]                  : " + TriggerCount);
                        Console.WriteLine("Color Of [Blue, White, Red]          : " + m_ColorFlag[0] + " , " + m_ColorFlag[1] + " , " + m_ColorFlag[2]);
                        Console.WriteLine("Average Y Axis Of [Blue, White, Red] : " + m_ColorCount[0, 0] + " , " + m_ColorCount[0, 1] + " , " + m_ColorCount[0, 2]);
                        Console.WriteLine("Area Of [Blue, White, Red]           : " + m_ColorCount[1, 0] + " , " + m_ColorCount[1, 1] + " , " + m_ColorCount[1, 2]);
                        Console.WriteLine("Defect Of [Blue, White, Red]         : " + m_DefectFlag[0] + " , " + m_DefectFlag[1] + " , " + m_DefectFlag[2]);
                        Console.WriteLine("Angle Of [Item01]                    : " + InspectionResultValue[0]);
                        Console.WriteLine("Rate Of [Item02]                     : " + Item02_AreaRate);
                        Console.WriteLine("[Item01, Item02, Item03] Error       : " + m_ErrorString[0] + " ! " + m_ErrorString[1] + " ! " + m_ErrorString[2]);
                        Console.WriteLine("-------------------------第1站三芯線轉線角度判別之參數(如上)-------------------------");
                    }
                    //----------------------------------------------------------------------------------------------------
                    #endregion
                }
                #endregion
            }
            catch (Exception Ex)
            {
                InspectionResultFlag[0] = -1;
                InspectionResultFlag[1] = -1;
                InspectionResultFlag[2] = -1;
                InspectionResultValue[0] = 0;
                InspectionResultValue[1] = 0;
                InspectionResultValue[2] = 0;

                Console.WriteLine("Exception In A1 Algorithm(三芯線轉線角度判別&三芯線色差判別&芯線夾取位置判別). Message: " + Ex.Message);
            }
        }

        //Inspection Item "4" : "剝外被邊緣品質判別".
        public void Algorithm_AualityOfCLine_Check()
        {
            try
            {
                #region "Edition 01"
                if (true)
                {
                    #region "Parameter"
                    //Parameter
                    //----------------------------------------------------------------------------------------------------
                    bool m_DefectFlag = false;
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Image Processing Algorithm"
                    //Step 1 Of Algorithm.
                    //----------------------------------------------------------------------------------------------------

                    //----------------------------------------------------------------------------------------------------

                    //Step 2 Of Algorithm.
                    //----------------------------------------------------------------------------------------------------

                    //----------------------------------------------------------------------------------------------------

                    //Step 3 Of Algorithm.
                    //----------------------------------------------------------------------------------------------------

                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Return"
                    //Set Result Value.
                    //----------------------------------------------------------------------------------------------------
                    if (m_DefectFlag)
                        InspectionResultFlag[3] = 1;
                    else
                        InspectionResultFlag[3] = 2;
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Record"
                    //Write CSV.
                    //----------------------------------------------------------------------------------------------------
                    //CSV_Write(PresentDateTime + "," + "剝外被邊緣品質判別" + "," + SerialNumber.ToString() + "," + TriggerCount.ToString() + "," + (InspectionResultFlag[3] == 2 ? "Pass" : "Fail"));
                    //----------------------------------------------------------------------------------------------------
                    #endregion

                    #region "Debug"
                    //Save Process Image In File And Print Out Imformation For Debug.
                    //----------------------------------------------------------------------------------------------------
                    if (DebugFlag)
                    {
                        Console.WriteLine("-------------------------第1站剝外被邊緣品質判別之參數(如下)-------------------------");
                        Console.WriteLine("Defect In ROI : " + m_DefectFlag);
                        Console.WriteLine("-------------------------第1站剝外被邊緣品質判別之參數(如上)-------------------------");
                    }
                    //----------------------------------------------------------------------------------------------------
                    #endregion
                }
                #endregion
            }
            catch (Exception Ex)
            {
                InspectionResultFlag[3] = -1;
                InspectionResultValue[3] = 0;

                Console.WriteLine("Exception In A1 Algorithm(剝外被邊緣品質判別). Message: " + Ex.Message);
            }
        }

        //Adjust RGB.
        public void ColorAdjustment()
        {
            try
            {
                if (!DoInspectionFlag)
                {
                    //Get Parameter.
                    //----------------------------------------------------------------------------------------------------               
                    ImgROI = ImgSource.GetSubRect(IP_Config.parameters.ROI01).Clone();
                    ImgAdjust = new Image<Bgr, byte>(ImgROI.Width, ImgROI.Height);

                    int[] m_Parameter = new int[9];
                    for (int i = 0; i < 9; i++)
                        m_Parameter[i] = IP_Config.parameters.Parameter01[i];

                    int m_AllCount = 0;
                    int[] m_RowCount = new int[ImgROI.Height];
                    for (int i = 0; i < ImgROI.Height; i++)
                        m_RowCount[i] = 0;                       
                    //----------------------------------------------------------------------------------------------------

                    //Count Pixel.
                    //----------------------------------------------------------------------------------------------------
                    Parallel.For
                        (0, ImgROI.Height, i =>
                        {
                            for (int j = 0; j < ImgROI.Width; j++)
                            {
                                if (ImgROI[i, j].Blue > m_Parameter[3] && ImgROI[i, j].Red > m_Parameter[4] && ImgROI[i, j].Green > m_Parameter[5])
                                {
                                    ImgAdjust.Data[i, j, 0] = 255;
                                    ImgAdjust.Data[i, j, 1] = 255;
                                    ImgAdjust.Data[i, j, 2] = 255;
                                    m_RowCount[i]++;
                                }
                                else if (ImgROI[i, j].Blue > m_Parameter[0] && ImgROI[i, j].Blue - ImgAdjust[i, j].Red > m_Parameter[1] && ImgROI[i, j].Red < m_Parameter[2])
                                {
                                    ImgAdjust.Data[i, j, 0] = 255;
                                    ImgAdjust.Data[i, j, 1] = 0;
                                    ImgAdjust.Data[i, j, 2] = 0;
                                    m_RowCount[i]++;
                                }
                                else if (ImgROI[i, j].Red > m_Parameter[6] && ImgROI[i, j].Red - ImgAdjust[i, j].Blue > m_Parameter[7] && ImgROI[i, j].Blue < m_Parameter[8])
                                {
                                    ImgAdjust.Data[i, j, 0] = 0;
                                    ImgAdjust.Data[i, j, 1] = 0;
                                    ImgAdjust.Data[i, j, 2] = 255;
                                    m_RowCount[i]++;
                                }
                                else
                                {

                                }
                            }
                        }
                        );

                    for (int i = 0; i < ImgROI.Height; i++)
                    {
                        m_AllCount += m_RowCount[i];
                    }
                    //----------------------------------------------------------------------------------------------------

                    //Check Area.
                    //----------------------------------------------------------------------------------------------------
                    Item02_AreaRate = (Convert.ToSingle(m_AllCount) / (Convert.ToSingle(Item02_LineHeight) * ImgROI.Width)) * Convert.ToSingle(100.0);
                    if (Item02_AreaRate > 90.0 && Item02_AreaRate < 110.0)
                        AdjustmentFlag[0] = true;
                    else
                        AdjustmentFlag[0] = false;
                    //----------------------------------------------------------------------------------------------------
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception Occured When User Set Color Condition : " + Ex.Message);
            }
        }

        //Gray Image To RGB Image.
        public Image<Bgr, byte> ToBGR(Image<Gray, byte> OriginImage)
        {
            try
            {
                return OriginImage.Convert<Bgr, byte>();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //RGB Image To Gary Image.
        public Image<Gray, byte> ToGray(Image<Bgr, byte> OriginImage)
        {
            try
            {
                return OriginImage.Convert<Gray, byte>();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //Gary Image To Binary Image.
        public Image<Gray, byte> ToBinarization(Image<Gray, byte> OriginImage, Gray ThresholdValue)
        {
            //Gray ThresholdValue = new Gray(70);
            try
            {
                return OriginImage.ThresholdBinary(ThresholdValue, new Gray(255));
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //Rotate Image.
        public Image<Bgr, byte> RotateImage(Image<Bgr, byte> ModelImage, int Degree, Bgr FillColor)
        {
            Image<Bgr, byte> modelImage_Emgucv = ModelImage.Clone();
            double angle = Degree * Math.PI / 180; // 弧度  
            double a = Math.Sin(angle), b = Math.Cos(angle);
            int width = ModelImage.Width;
            int height = ModelImage.Height;
            int width_rotate = Convert.ToInt32(height * Math.Abs(a) + width * Math.Abs(b));
            int height_rotate = Convert.ToInt32(width * Math.Abs(a) + height * Math.Abs(b));
            //旋转数组map
            // [ m0  m1  m2 ] ===>  [ A11  A12   b1 ]
            // [ m3  m4  m5 ] ===>  [ A21  A22   b2 ]
            //float[] map = new float[6];
            //此处为修改点，opencv可以直接使用数组，但emgucv似乎不认，所以改为了Matrix。
            Matrix<float> map_matrix_temp = new Matrix<float>(2, 3);

            // 旋转中心
            PointF center = new PointF(width / 2, height / 2);
            CvInvoke.GetRotationMatrix2D(center, Degree, 1.0, map_matrix_temp);

            map_matrix_temp[0, 2] += (width_rotate - width) / 2;
            map_matrix_temp[1, 2] += (height_rotate - height) / 2;

            Image<Bgr, byte> img_rotate = new Image<Bgr, byte>(width_rotate, height_rotate, FillColor);

            //对图像做仿射变换
            //CV_WARP_FILL_OUTLIERS - 填充所有输出图像的象素。
            //如果部分象素落在输入图像的边界外，那么它们的值设定为 fillval.
            //CV_WARP_INVERSE_MAP - 指定 map_matrix 是输出图像到输入图像的反变换，
            CvInvoke.WarpAffine(modelImage_Emgucv, img_rotate, map_matrix_temp, new Size(width_rotate, height_rotate), Inter.Nearest, Warp.Default, BorderType.Transparent, new MCvScalar(0d, 0d, 0d, 0d));

            return img_rotate;
        }

        //Move Image.
        Image<Bgr, byte> MoveImage(Image<Bgr, byte> ModelImage, int X, int Y, Bgr FillColor)
        {
            Image<Bgr, byte> modelImage_Emgucv = ModelImage.Clone();
            int degree = 0;
            double angle = degree * Math.PI / 180; // 弧度  
            double a = Math.Sin(angle), b = Math.Cos(angle);
            int width = ModelImage.Width;
            int height = ModelImage.Height;
            int width_rotate = Convert.ToInt32(height * Math.Abs(a) + width * Math.Abs(b));
            int height_rotate = Convert.ToInt32(width * Math.Abs(a) + height * Math.Abs(b));
            //旋转数组map
            // [ m0  m1  m2 ] ===>  [ A11  A12   b1 ]
            // [ m3  m4  m5 ] ===>  [ A21  A22   b2 ]
            //float[] map = new float[6];
            //此处为修改点，opencv可以直接使用数组，但emgucv似乎不认，所以改为了Matrix。
            Matrix<float> map_matrix_temp = new Matrix<float>(2, 3);

            // 旋转中心
            PointF center = new PointF(width / 2, height / 2);
            CvInvoke.GetRotationMatrix2D(center, degree, 1.0, map_matrix_temp);

            map_matrix_temp[0, 2] += X; // (width_rotate - width) / 2;
            map_matrix_temp[1, 2] += Y; // (height_rotate - height) / 2;

            Image<Bgr, byte> img_rotate = new Image<Bgr, byte>(width_rotate, height_rotate, FillColor);

            //对图像做仿射变换
            //CV_WARP_FILL_OUTLIERS - 填充所有输出图像的象素。
            //如果部分象素落在输入图像的边界外，那么它们的值设定为 fillval.
            //CV_WARP_INVERSE_MAP - 指定 map_matrix 是输出图像到输入图像的反变换，
            CvInvoke.WarpAffine(modelImage_Emgucv, img_rotate, map_matrix_temp, new Size(width_rotate, height_rotate), Inter.Nearest, Warp.Default, BorderType.Transparent, new MCvScalar(0d, 0d, 0d, 0d));

            return img_rotate;
        }

        //Optic Adjustment.
        public void OpticAdjustment()
        {
            //Declare Parameter.
            //------------------------------------------------------------------------------------------------------------------------
            Image<Gray, byte> t_ImgGray = ImgSource.GetSubRect(RectAdjust).Convert<Gray, byte>().Clone();
            double[] t_LightColValue = new double[t_ImgGray.Width];
            double[] t_SharpColValue = new double[t_ImgGray.Width];

            ImgAdjust = ImgSource.Clone();
            CurrentLightValue = 0;
            CurrentSharpValue = 0;
            //------------------------------------------------------------------------------------------------------------------------

            // Calculate Light.
            //------------------------------------------------------------------------------------------------------------------------
            Parallel.For
               (0, t_ImgGray.Width, i =>
               {
                   t_LightColValue[i] = 0;

                   for (int j = 0; j < t_ImgGray.Height; j++)
                   {
                       t_LightColValue[i] += Convert.ToDouble(t_ImgGray.Data[j, i, 0]);
                   }
               }
               );

            for (int i = 0; i < t_ImgGray.Width; i++)
                CurrentLightValue += t_LightColValue[i];

            CurrentLightValue /= Convert.ToDouble(t_ImgGray.Width * t_ImgGray.Height);
            //------------------------------------------------------------------------------------------------------------------------

            //Calculate Sharp.
            //------------------------------------------------------------------------------------------------------------------------
            
            Parallel.For
               (0, t_ImgGray.Width, i =>
               {
                   t_SharpColValue[i] = 0;

                   for (int j = 0; j < t_ImgGray.Height; j++)
                   {
                       t_SharpColValue[i] += Math.Pow(Convert.ToDouble(t_ImgGray.Data[j, i, 0]) - CurrentLightValue, 2);
                   }
               }
               );

            for (int i = 0; i < t_ImgGray.Width; i++)
                CurrentSharpValue += t_SharpColValue[i];

            CurrentSharpValue /= (t_ImgGray.Width * t_ImgGray.Height);
            //------------------------------------------------------------------------------------------------------------------------

            //Draw ROI.
            //------------------------------------------------------------------------------------------------------------------------
            CvInvoke.Rectangle(ImgAdjust, RectAdjust, new MCvScalar(0, 0, 255, 255), 3); //Console.WriteLine("校正完成!");
            //------------------------------------------------------------------------------------------------------------------------
        }

        //Count Slight Angle Of Three Core Line Which Is Less Than 30 Degrees.
        public double CountSmallAngle(string m_Mode, double m_Parameter_A, double m_Parameter_B)
        {
            double m_ReturnValue = 0;

            switch (m_Mode)
            {
                case "SmallAngle":
                    if (m_Parameter_A < 0 || m_Parameter_B < 0)
                        m_ReturnValue = 0;
                    else
                        m_ReturnValue =  Math.Atan((Math.Sqrt(3) / 3.0) * ((m_Parameter_B - m_Parameter_A) / (m_Parameter_B + m_Parameter_A))) * 180.0 / Math.PI;
                    break;

                case "TwoLine":
                    if (m_Parameter_A < 0 || m_Parameter_B < 0)
                        m_ReturnValue = 0;
                    else
                        m_ReturnValue = Math.Abs(Math.Acos(m_Parameter_A / m_Parameter_B) * 180.0 / Math.PI - 60.0);
                    break;

                default:
                    m_ReturnValue = 0;
                    break;
            }

            return m_ReturnValue;
        }

        //Convert Time Format.
        private string TimeFormat(int mode, DateTime datetime)
        {
            string date;
            string time;
            string str;

            switch (mode)
            {
                case 1:
                    date = datetime.ToShortDateString();
                    time = datetime.ToString("hh:mm:ss.FFF", System.Globalization.CultureInfo.InvariantCulture).PadRight(12, '0');
                    str = " [ " + date + " " + time + " ] : ";
                    return str;

                case 2:
                    str = datetime.Year.ToString() + "年" + datetime.Month.ToString() + "月" + datetime.Day.ToString() + "日" + datetime.Hour.ToString() + "時" + datetime.Minute.ToString() + "分" + datetime.Second.ToString() + "秒";
                    return str;

                case 3:
                    str = datetime.Year.ToString() + "Y" + datetime.Month.ToString() + "M" + datetime.Day.ToString() + "D";
                    return str;

                case 4:
                    str = datetime.Hour.ToString() + "H" + datetime.Minute.ToString() + "M" + datetime.Second.ToString() + "S";
                    return str;

                case 5:
                    str = datetime.Year.ToString() + "." + datetime.Month.ToString() + "." + datetime.Day.ToString() + "_" + datetime.Hour.ToString() + "." + datetime.Minute.ToString() + "." + datetime.Second.ToString();
                    return str;

                case 6:
                    str = datetime.Year.ToString() + "." + datetime.Month.ToString() + "." + datetime.Day.ToString();
                    return str;

                case 7:
                    str = datetime.Hour.ToString() + "." + datetime.Minute.ToString() + "." + datetime.Second.ToString();
                    return str;

                default:
                    date = datetime.ToShortDateString();
                    time = datetime.ToString("hh:mm:ss.FFF", System.Globalization.CultureInfo.InvariantCulture).PadRight(12, '0');
                    str = " [ " + date + " " + time + " ] : ";
                    return str;
            }
        }

        //Count Use Time 
        public void UseTime(string title, TimeSpan ts)
        {
            if (PrintFlag[0])
            {
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                Console.WriteLine(title + elapsedTime);
            }
        }

        //(Copy From Web)
        private void SaveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality 

            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec 
            ImageCodecInfo jpegCodec = this.GetEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }

        //(Copy From Web)
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        //(Copy From Web)
        public static double DirectorySize(DirectoryInfo directoryInfo)
        {
            double Size = 0;
            FileInfo[] fi = directoryInfo.GetFiles();
            foreach (FileInfo f in fi)
            {
                Size += f.Length;
            }
            DirectoryInfo[] dis = directoryInfo.GetDirectories();
            foreach (DirectoryInfo d in dis)
            {
                if (d.Name != "System Volume Information" && d.Name.Substring(0, 1) != "$")//避開此類folder權限問題
                    Size += DirectorySize(d);
            }
            return (Size);
        }

        //(Copy From Web)
        public void DeleteSrcFolder_E01(string file)
        {
            //去除資料夾和子檔案的只讀屬性
            //去除資料夾的只讀屬性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除檔案的只讀屬性
            System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            //判斷資料夾是否還存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (File.Exists(f))
                    {
                        //如果有子檔案刪除檔案
                        File.Delete(f);
                    }
                    else
                    {
                        //迴圈遞迴刪除子資料夾 
                        DeleteSrcFolder_E02(f);
                    }
                }
                //刪除空資料夾
                //Directory.Delete(file);
            }
        }

        //(Copy From Web)
        private void DeleteSrcFolder_E02(string file)
        {
            //去除資料夾和子檔案的只讀屬性
            //去除資料夾的只讀屬性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除檔案的只讀屬性
            System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            //判斷資料夾是否還存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (File.Exists(f))
                    {
                        //如果有子檔案刪除檔案
                        File.Delete(f);
                    }
                    else
                    {
                        //迴圈遞迴刪除子資料夾 
                        DeleteSrcFolder_E02(f);
                    }
                }
                //刪除空資料夾
                Directory.Delete(file);
            }
        }

        //Test Many Image.
        public void TestManyImage()
        {
            string t_Station = "K1";
            string t_Path = System.Environment.CurrentDirectory + "/Appendix/Image/Test/K1/";
            string[] t_Files = Directory.GetFiles(t_Path, "*.bmp");
            int t_Number = 0;

            foreach (string t_File in t_Files)
            {
                Barcode = "Manual";
                SerialNumber = ++t_Number;
                TriggerCount = 1;

                InspectionItemFlag[0] = true;
                InspectionItemFlag[1] = true;
                InspectionItemFlag[2] = true;
                InspectionItemFlag[3] = false;

                SetImage(0, new Image<Bgr, byte>(t_File));

                if (DoInspectionFlag)
                {
                    Console.WriteLine("K1 IP Thread Was Busy When TestManyImage() Executed.");
                }
                else
                {
                    DoInspectionFlag = true;
                }

                while (DoInspectionFlag)
                {
                    Thread.Sleep(100);
                }

                Console.WriteLine("站名 : " + t_Station);
                Console.WriteLine("檔名 : " + t_File);
                Console.WriteLine("編號 : " + SerialNumber);
                Console.WriteLine("時間 : " + PresentTime);
                Console.WriteLine("結果 : " + InspectionResultFlag[0].ToString() + "," + InspectionResultFlag[1].ToString() + "," + InspectionResultFlag[2].ToString() + "," + InspectionResultFlag[3].ToString() + "," + InspectionResultValue[0].ToString() + "," + Item02_AreaRate + "\n\n\n");

                Thread.Sleep(1000);
            }
        }
    }
}
