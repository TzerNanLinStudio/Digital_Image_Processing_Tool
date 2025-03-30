using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class CustomConfig_UI_Parameters
    {
        #region "Image Recording Parameter Declare"
        private int m_Threshold;
        private float[][] m_Kernel;
        #endregion

        public CustomConfig_UI_Parameters()
        {
            #region "Image Recording Parameter Initialize"
            m_Threshold = 127;
            m_Kernel = new float[][]  
            {
                new float[] { 0, -1, 0 },
                new float[] { -1, 5, -1 },
                new float[] { 0, -1, 0 }
            };
            #endregion
        }

        #region "Image Recording Parameter Set Or Get"
        public int Threshold { get => m_Threshold; set => m_Threshold = value; }

        public float[][] Kernel {   get => m_Kernel; set => m_Kernel = value;  }
        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomConfig_UI : BaseConfig<CustomConfig_UI>
    {
        private string _version = "UIConfig_202503301630";

        public CustomConfig_UI_Parameters parameters;

        public CustomConfig_UI()
        {
            version = _version;

            parameters = new CustomConfig_UI_Parameters();
            
            RecipeFullPath = "./recipt.bat"; // Default Path
        }

        public CustomConfig_UI(string fileFullPath)
        {
            parameters = new CustomConfig_UI_Parameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_UI config)
        {
            parameters = config.parameters.Clone() as CustomConfig_UI_Parameters;

            return (this._version == config._version);
        }
    }
}

