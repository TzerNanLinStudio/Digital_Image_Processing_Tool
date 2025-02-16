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
        private bool m_SaveOriginImage;
        private bool m_SaveResultImage;
        private int m_NumberOfImage;
        private int m_SizeOfImage;
        #endregion

        public CustomConfig_UI_Parameters()
        {
            #region "Image Recording Parameter Initialize"
            m_SaveOriginImage = true;
            m_SaveResultImage = true;
            m_NumberOfImage = 50;
            m_SizeOfImage = 50;
            #endregion
        }

        #region "Image Recording Parameter Set Or Get"
        public bool SaveOriginImage { get => m_SaveOriginImage; set => m_SaveOriginImage = value; }
        public bool SaveResultImage { get => m_SaveResultImage; set => m_SaveResultImage = value; }
        public int NumberOfImage { get => m_NumberOfImage; set => m_NumberOfImage = value; }
        public int SizeOfImage { get => m_SizeOfImage; set => m_SizeOfImage = value; }
        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomConfig_UI : BaseConfig<CustomConfig_UI>
    {
        private string _version = "UIConfig_202004081500";

        public CustomConfig_UI_Parameters parameters;

        public CustomConfig_UI()
        {
            version = _version;

            parameters = new CustomConfig_UI_Parameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";
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

