using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config_sharp
{
    public class LightingParameters
    {
        public LightingParameters()
        {
            m_CH1_Value = -1;
            m_CH2_Value = -1;
            m_CH3_Value = -1;
            m_CH4_Value = -1;
        }

        private int m_CH1_Value;
        private int m_CH2_Value;
        private int m_CH3_Value;
        private int m_CH4_Value;

        public int CH1_Value
        {
            set
            {
                m_CH1_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH1_Value == -1) ? 0 : m_CH1_Value;
            }
        }
        public int CH2_Value
        {
            set
            {
                m_CH2_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH2_Value == -1) ? 0 : m_CH2_Value;
            }
        }
        public int CH3_Value
        {
            set
            {
                m_CH3_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH3_Value == -1) ? 0 : m_CH3_Value;
            }
        }

        public int CH4_Value
        {
            set
            {
                m_CH4_Value = (value < 0 || value > 100) ? -1 : value;
            }

            get
            {
                return (m_CH4_Value == -1) ? 0 : m_CH4_Value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    
    public class CustomConfig_Lighting : BaseConfig<CustomConfig_Lighting>
    {
        //private string _version = "FinaljudgeListConfig_201912031520";
        private string _version = "LightingConfig_202004081500";

        public LightingParameters parameters;

        public CustomConfig_Lighting()
        {
            version = _version;

            parameters = new LightingParameters();
            // Default Path
            RecipeFullPath = "./recipt.bat";

            ConfigName = "Lighting";
        }

        public CustomConfig_Lighting(string fileFullPath)
        {
            parameters = new LightingParameters();

            RecipeFullPath = fileFullPath;
        }

        protected override bool Write_Object(CustomConfig_Lighting config)
        {
            parameters = config.parameters.Clone() as LightingParameters;

            return (this._version == config._version);
        }
    }
}

