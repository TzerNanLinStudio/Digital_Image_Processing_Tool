using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_sharp
{
    public class CustomConfig_IP_Parameters
    {
        #region "1st Station Parameter Declare"
        private System.Drawing.Rectangle m_ROI01;
        private int[] m_Parameter01;
        private int[][][] m_Offset01;
        #endregion

        #region "2nd Station Parameter Declare"
        private System.Drawing.Rectangle[] m_ROI02 = new System.Drawing.Rectangle[3];
        private int[] m_Parameter02 = new int[30];
        #endregion

        #region "3rd Station Parameter Declare"
        private System.Drawing.Rectangle[] m_ROI03 = new System.Drawing.Rectangle[4];
        private System.Drawing.Point[] m_Center03 = new System.Drawing.Point[4];
        private double[] m_Parameter03 = new double[20];
        #endregion

        #region "4th Station Parameter Declare"
        private int m_SAT4_Value;
        #endregion

        #region "Other Parameter Declare"
        //Other.
        //----------------------------------------
        private int m_NumberOfImageInFile;
        //----------------------------------------
        #endregion

        public CustomConfig_IP_Parameters()
        {
            #region "1st Station Parameter Initialize"
            m_ROI01 = new System.Drawing.Rectangle(0, 0, 100, 100);

            m_Parameter01 = new int[12];
            for (int index = 0; index < 12; index++)
                m_Parameter01[index] = 0;

            m_Offset01 = new int[2][][];
            for (int i = 0; i < 2; i++)
            {
                m_Offset01[i] = new int[6][];
                for(int j = 0; j < 6; j++)
                {
                    m_Offset01[i][j] = new int[6];
                    for(int k = 0; k < 6; k++)
                    {
                        m_Offset01[i][j][k] = 0;
                    }
                }
            }               
            #endregion

            #region "2nd Station Parameter Initialize"
            for (int index = 0; index < 3; index++)
                m_ROI02[index] = new System.Drawing.Rectangle(0, 0, 100, 100);
            for (int index = 0; index < 30; index++)
                m_Parameter02[index] = 0;
            #endregion

            #region "3rd Station Parameter Initialize"
            for (int index = 0; index < 4; index++)
                m_ROI03[index] = new System.Drawing.Rectangle(0, 0, 100, 100);
            for (int index = 0; index < 4; index++)
                m_Center03[index] = new System.Drawing.Point(0, 0);
            for (int index = 0; index < 20; index++)
                m_Parameter03[index] = 0;
            #endregion

            #region "4th Station Parameter Initialize"
            m_SAT4_Value = -1;
            #endregion

            #region "Other Parameter Initialize"
            //Other.
            //----------------------------------------
            m_NumberOfImageInFile = 100;
            //----------------------------------------
            #endregion
        }

        #region "1st Station Parameter Set Or Get"
        public System.Drawing.Rectangle ROI01 { get => m_ROI01; set => m_ROI01 = value; }
        public int[] Parameter01 { get => m_Parameter01; set => m_Parameter01 = value; }
        public int[][][] Offset01 { get => m_Offset01; set => m_Offset01 = value; }
        #endregion

        #region "2nd Station Parameter Set Or Get"
        public System.Drawing.Rectangle[] ROI02 { get => m_ROI02; set => m_ROI02 = value; }
        public int[] Parameter02 { get => m_Parameter02; set => m_Parameter02 = value; }
        #endregion

        #region "3rd Station Parameter Set Or Get"
        public System.Drawing.Rectangle[] ROI03 { get => m_ROI03; set => m_ROI03 = value; }
        public System.Drawing.Point[] Center03 { get => m_Center03; set => m_Center03 = value; }
        public double[] Parameter03 { get => m_Parameter03; set => m_Parameter03 = value; }
        #endregion

        #region "4th Station Parameter Set Or Get"
        public int SAT4_Value
        {
            set
            {
                m_SAT4_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_SAT4_Value == -1) ? 0 : m_SAT4_Value;
            }
        }
        #endregion

        #region "Other Parameter Set Or Get"
        //Other.
        //----------------------------------------
        public int NumberOfImageInFile { get => m_NumberOfImageInFile; set => m_NumberOfImageInFile = value; }
        //----------------------------------------
        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomConfig_IP : BaseConfig<CustomConfig_IP>
    {
        //private string _version = "FinaljudgeListConfig_201912031520";
        private string _version = "IPConfig_202004081500";

        public CustomConfig_IP_Parameters parameters;

        public CustomConfig_IP()
        {
            version = _version;

            parameters = new CustomConfig_IP_Parameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";

            ConfigName = "IP";
        }

        public CustomConfig_IP(string fileFullPath)
        {
            parameters = new CustomConfig_IP_Parameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_IP config)
        {
            parameters = config.parameters.Clone() as CustomConfig_IP_Parameters;

            return (this._version == config._version);
        }
    }
}

